using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public TileGrid grid;
    public NoiseGen elevationAdd;
    public NoiseGen elevationMult;
    public NoiseGen humidityNoise;
    public NoiseGen temperatureNoise;
    public float waterLevel;
    public float maxStepsFromWater = 4;
    public int minRiverCount;   
    public int maxRiverCount;
    public int riverSearchDist;
    public AnimationCurve waterHumidityCurve;
    public float temperatureHumidityScaling;
    public float elevationTemperatureScaling;
    Biome[] biomeChart = {
        Biome.Tundra, Biome.Steppes, Biome.Desert, Biome.Desert,
        Biome.BorealForest, Biome.BorealForest, Biome.TemperateForest, Biome.Savanna,
        Biome.BorealForest, Biome.TemperateForest, Biome.TemperateForest, Biome.Savanna,
        Biome.Swamp, Biome.RainForest, Biome.RainForest, Biome.RainForest
    };
    float[] humidityRanges = {0.25f,0.5f,0.75f};
    float[] temperatureRanges = {0.25f, 0.5f, 0.75f};
    public float globalFrequency = 1;

    //public int riverSearchRange;
    public int seed;

    public void Generate2()
    {
        for (int i = 0; i < grid.tileCountX; i++)
        {
            for (int j = 0; j < grid.tileCountY; j++)
            {
                grid.tiles[i, j].elevation = Random.Range(0f, 1f);
            }
        }
    }

    private void ResetMap()
    {
        for (int i = 0; i < grid.tileCountX; i++) {
            for (int j = 0; j < grid.tileCountY; j++) {
                grid.tiles[i,j] = new Tile();
            }
        }
    }

    public void Generate()
    {
        ResetMap();
        /*for (int i = 0; i < grid.tileCountX; i++)
        {
            for (int j = 0; j < grid.tileCountY; j++)
            {
                grid.tiles[i, j].elevation = Random.Range(0f, 1f);
            }
        }*/
        Noise.SetSeed(seed);
        Random.InitState(seed);
        for (int i = 0; i < grid.tileCountX; i++)
        {
            for (int j = 0; j < grid.tileCountY; j++)
            {
                //ELEVATION
                Vector3 v = HexMetric.WorldCoords(i, j);
                Vector2 scaled = new Vector2(v.x / grid.tileCountX, v.z / grid.tileCountY) * globalFrequency;
                float add = elevationAdd.Evaluate(scaled);
                float mult = elevationMult.Evaluate(scaled);
                if (add > waterLevel)
                {
                    grid.tiles[i, j].elevation = (waterLevel) + (add - (waterLevel)) * mult;
                }
                else
                {
                    //grid.tiles[i, j].elevation = (waterLevel) + (add - (waterLevel)) * Mathf.Max(1f,mult);// (waterLevel) + (add - (waterLevel)) * mult;
                    grid.tiles[i, j].elevation = add;
                    //grid.tiles[i, j].elevation = (waterLevel) + (add - (waterLevel)) * mult + 0.1f;
                }
                grid.tiles[i, j].temperature = 1f - Mathf.Abs((float)j - grid.tileCountY / 2f) * 2f / grid.tileCountY;
                grid.tiles[i, j].temperature += temperatureNoise.Evaluate(scaled);
                grid.tiles[i, j].temperature -= (grid.tiles[i, j].elevation-0.5f) * elevationTemperatureScaling;
                //grid.tiles[i, j].elevation = mult;
                //grid.SetTextureData(i,j,colorGradient.Evaluate(grid.tiles[i,j].elevation));
                //grid.SetTextureData(i, j, Color.HSVToRGB(noise,1,1));

                //HUMIDITY
                grid.tiles[i, j].humidity = humidityNoise.Evaluate(scaled);
                //grid.tiles[i, j].humidity = grid.tiles[i, j].elevation > waterLevel ? 0f : 1f;
            }
        }
        int numRivers = Random.Range(minRiverCount, maxRiverCount);
        for (int i = 0, j = 0; i < numRivers && j<200; i++, j++)
        {
            int x = Random.Range(0, grid.tileCountX);
            int y = Random.Range(0, grid.tileCountY);
            Vector2Int peak = Elevate(x, y, 4);
            if (grid.tiles[peak.x, peak.y].elevation < waterLevel + 0.2f || grid.tiles[peak.x, peak.y].waterType != WaterType.None)
            {
                i--; 
                continue;
            }
            //Debug.Log(grid.tiles[peak.x, peak.y].elevation);
            //grid.tiles[peak.x, peak.y].elevation += 0.3f;
            //Debug.Log(peak);
            GenerateRiver(peak);
        }
        CalculateWater();
        CalculateHumidity();

        foreach (Tile t in grid.tiles) {
            t.biome = DetermineBiome(t);
        }
        //GenerateRiver(new Vector2Int(110, 50));
    }

    Biome DetermineBiome(Tile tile) {
        int tempIndex = 0;
        while (tempIndex < temperatureRanges.Length && tile.temperature > temperatureRanges[tempIndex]) {
            tempIndex++;
        }

        int humidityIndex = 0;
        while (humidityIndex < humidityRanges.Length && tile.humidity > humidityRanges[humidityIndex])
        {
            humidityIndex++;
        }

        return biomeChart[humidityIndex * (temperatureRanges.Length+1) + tempIndex];
    }

    void CalculateWater() {
        for (int i = 0; i < grid.tileCountX; i++) {
            for (int j = 0; j < grid.tileCountY; j++) {
                if (grid.tiles[i, j].elevation <= waterLevel) {
                    grid.tiles[i, j].waterType = WaterType.Ocean;
                }
            }
        }
    }

    Vector2Int Elevate(int x, int y, int maxSteps)
    {
        for (int s = 0; s < maxSteps; s++)
        {
            Vector2Int maxPos = MaxElevation(x, y, 2);
            if (grid.tiles[maxPos.x, maxPos.y].elevation > grid.tiles[x, y].elevation)
            {
                x = maxPos.x;
                y = maxPos.y;
            }
            else
            {
                break;
            }
            //Vector2Int loc = new Vector2Int(x, y);
            //float highestE = 0;
            //if (x > 0 && grid.tiles[x - 1, y].elevation > highestE)
            //{
            //    highestE = grid.tiles[x - 1, y].elevation;
            //    loc = new Vector2Int(x - 1, y);
            //}
            //if (y > 0 && grid.tiles[x, y - 1].elevation > highestE)
            //{
            //    highestE = grid.tiles[x, y - 1].elevation;
            //    loc = new Vector2Int(x, y - 1);
            //}
            //if (x < grid.tileCountX - 1 && grid.tiles[x + 1, y].elevation > highestE)
            //{
            //    highestE = grid.tiles[x + 1, y].elevation;
            //    loc = new Vector2Int(x + 1, y);
            //}
            //if (y < grid.tileCountY - 1 && grid.tiles[x, y + 1].elevation > highestE)
            //{
            //    highestE = grid.tiles[x, y + 1].elevation;
            //    loc = new Vector2Int(x, y + 1);
            //}
            //if (highestE <= grid.tiles[x, y].elevation)
            //{
            //    return loc;
            //}
            //x = loc.x;
            //y = loc.y;
        }
        return new Vector2Int(x, y);
    }

    void CalculateHumidity()
    {
        int[,] stepsFromWater = new int[grid.tileCountX, grid.tileCountY];
        Queue<int> tiles = new Queue<int>();
        for (int i = 0; i < grid.tileCountX; i++)
        {
            for (int j = 0; j < grid.tileCountY; j++)
            {
                if (grid.tiles[i, j].waterType==WaterType.River)
                {
                    stepsFromWater[i, j] = 0;

                    tiles.Enqueue(j * grid.tileCountX + i);
                }
                else
                {
                    stepsFromWater[i, j] = -1;
                }
            }
        }
        while (tiles.Count > 0)
        {
            int t = tiles.Dequeue();
            int y = t / grid.tileCountX;
            int x = t - y * grid.tileCountX;
            if (x > 0 && stepsFromWater[x - 1, y] == -1)
            {
                stepsFromWater[x - 1, y] = stepsFromWater[x, y] + 1;
                tiles.Enqueue(y * grid.tileCountX + (x - 1));
            }
            if (x < grid.tileCountX - 1 && stepsFromWater[x + 1, y] == -1)
            {
                stepsFromWater[x + 1, y] = stepsFromWater[x, y] + 1;
                tiles.Enqueue(y * grid.tileCountX + (x + 1));
            }
            if (y > 0 && stepsFromWater[x, y - 1] == -1)
            {
                stepsFromWater[x, y - 1] = stepsFromWater[x, y] + 1;
                tiles.Enqueue((y - 1) * grid.tileCountX + x);
            }
            if (y < grid.tileCountY - 1 && stepsFromWater[x, y + 1] == -1)
            {
                stepsFromWater[x, y + 1] = stepsFromWater[x, y] + 1;
                tiles.Enqueue((y + 1) * grid.tileCountX + x);
            }
        }
        for (int i = 0; i < grid.tileCountX; i++)
        {
            for (int j = 0; j < grid.tileCountY; j++)
            {
                if (grid.tiles[i, j].waterType == WaterType.Ocean)
                {
                    grid.tiles[i, j].humidity = 1;
                }
                else
                {
                    grid.tiles[i, j].humidity += waterHumidityCurve.Evaluate(stepsFromWater[i, j] / maxStepsFromWater);
                    grid.tiles[i, j].humidity = Mathf.Lerp(grid.tiles[i, j].humidity, grid.tiles[i, j].humidity * (grid.tiles[i, j].temperature*2f), temperatureHumidityScaling);
                }

                //grid.tiles[i, j].humidity += Mathf.Clamp01(1f - humidityDecay * stepsFromWater[i, j]);
            }
        }
    }

    /*void SimulationStep()
    {
        float[,] humidity = new float[grid.tileCountX, grid.tileCountY];
        for (int i = 0; i < grid.tileCountX; i++)
        {
            for (int j = 0; j < grid.tileCountY; j++)
            {
                float remain = 1f;
                if (i > 0)
                {
                    humidity[i - 1, j] += grid.tiles[i, j].humidity * humiditySpreadRate * 0.25f;
                    remain -= humiditySpreadRate * 0.25f;
                }
                if (i < grid.tileCountX - 1)
                {
                    humidity[i + 1, j] += grid.tiles[i, j].humidity * humiditySpreadRate * 0.25f;
                    remain -= humiditySpreadRate * 0.25f;
                }
                if (j > 0)
                {
                    humidity[i, j - 1] += grid.tiles[i, j].humidity * humiditySpreadRate * 0.25f;
                    remain -= humiditySpreadRate * 0.25f;
                }
                if (j < grid.tileCountY - 1)
                {
                    humidity[i, j + 1] += grid.tiles[i, j].humidity * humiditySpreadRate * 0.25f;
                    remain -= humiditySpreadRate * 0.25f;
                }
                humidity[i, j] += grid.tiles[i, j].humidity * remain;
            }
        }
        for (int i = 0; i < grid.tileCountX; i++)
        {
            for (int j = 0; j < grid.tileCountY; j++)
            {
                grid.tiles[i, j].humidity = humidity[i, j];
            }
        }

    }*/

    void GenerateRiver(Vector2Int pos)
    {
        //List<Vector2Int> tiles = HexSearch(pos, 2, true);
        ////Debug.Log("found " + tiles.Count);
        //foreach (Vector2Int t in tiles)
        //{
        //	grid.tiles[t.x, t.y].waterType = WaterType.River;
        //}
        //Direction direction = (Direction)Random.Range(0, 6);
        ////Direction direction = Direction.NE;
        //while (true)
        //{
        //    if (grid.tiles[pos.x, pos.y].elevation < waterLevel)
        //    {
        //        break;
        //    }
        //    grid.tiles[pos.x, pos.y].waterType = WaterType.River;
        //    Vector2Int newPos = pos + HexMetric.Step(pos, direction);
        //    if (newPos.x < 0 || newPos.y < 0 || newPos.x >= grid.tileCountX || newPos.y >= grid.tileCountY)
        //    {
        //        break;
        //    }
        //    if (grid.tiles[pos.x, pos.y].elevation < grid.tiles[newPos.x, newPos.y].elevation)
        //    {
        //        grid.tiles[newPos.x, newPos.y].elevation = grid.tiles[pos.x, pos.y].elevation;
        //    }
        //    pos = newPos;
        //    //Vector2Int minE = MinElevation(x, y, 1);
        //    //if (grid.tiles[minE.x, minE.y].elevation >= grid.tiles[x, y].elevation)
        //    //{
        //    //    break;
        //    //}
        //    //x = minE.x;
        //    //y = minE.y;
        //}
        for (int test = 0; test < 100; test++)
        {
            grid.tiles[pos.x, pos.y].waterType = WaterType.River;
            List<Vector2Int> adj = HexSearch(pos, riverSearchDist, true);
            Vector2Int minE = adj[0];
            foreach (Vector2Int v in adj)
            {
                if (grid.tiles[v.x, v.y].elevation < grid.tiles[minE.x, minE.y].elevation)
                {
                    minE = v;
                }
            }
            if (minE == pos)
            {
                break;
            }

            //grid.tiles[minE.x, minE.y].elevation += 0.1f;
            int testC2 = 0;
            while (testC2 < riverSearchDist && pos != minE)
            {
                testC2++;
                pos += HexMetric.Step(pos, HexMetric.DirectionBetween(pos, minE));
                if (!InBounds(pos) || grid.tiles[pos.x, pos.y].elevation < waterLevel)
                {
                    return;
                }
                grid.tiles[pos.x, pos.y].waterType = WaterType.River;

            }
            pos = minE;

        }
    }
    bool InBounds(Vector2Int v)
    {
        return v.x >= 0 && v.y >= 0 && v.x < grid.tileCountX && v.y < grid.tileCountY;
    }

    Vector2Int MinElevation(int x, int y, int range)
    {
        Vector2Int minPos = new Vector2Int(-1, -1);
        float minVal = float.MaxValue;
        for (int i = Mathf.Max(0, x - range); i <= Mathf.Min(grid.tileCountX - 1, x + range); i++)
        {
            for (int j = Mathf.Max(0, y - range); j <= Mathf.Min(grid.tileCountY - 1, y + range); j++)
            {
                if (grid.tiles[i, j].elevation < minVal)
                {
                    minPos = new Vector2Int(i, j);
                    minVal = grid.tiles[i, j].elevation;
                }
            }
        }
        return minPos;
    }

    Vector2Int MaxElevation(int x, int y, int range)
    {
        Vector2Int maxPos = new Vector2Int(-1, -1);
        float maxVal = float.MinValue;
        for (int i = Mathf.Max(0, x - range); i <= Mathf.Min(grid.tileCountX - 1, x + range); i++)
        {
            for (int j = Mathf.Max(0, y - range); j <= Mathf.Min(grid.tileCountY - 1, y + range); j++)
            {
                if (grid.tiles[i, j].elevation > maxVal)
                {
                    maxPos = new Vector2Int(i, j);
                    maxVal = grid.tiles[i, j].elevation;
                }
            }
        }
        return maxPos;
    }

    List<Vector2Int> HexSearch(Vector2Int center, int radius, bool includeCenter)
    {
        Vector2Int hexC = HexMetric.XYToHex(center);
        List<Vector2Int> tiles = new List<Vector2Int>();
        for (int y = hexC.y; y <= hexC.y + radius; y++)
        {
            for (int x = hexC.x - radius; x <= hexC.x + radius - (y - hexC.y); x++)
            {
                if (!includeCenter && x == hexC.x && y == hexC.y)
                {
                    continue;
                }
                Vector2Int coords = HexMetric.HexToXY(new Vector2Int(x, y));
                if (coords.x >= 0 && coords.y >= 0 && coords.x < grid.tileCountX && coords.y < grid.tileCountY)
                {
                    tiles.Add(coords);
                }
            }
        }
        for (int y = hexC.y - 1; y >= hexC.y - radius; y--)
        {
            for (int x = hexC.x - radius + Mathf.Abs(y - hexC.y); x <= hexC.x + radius; x++)
            {
                Vector2Int coords = HexMetric.HexToXY(new Vector2Int(x, y));
                if (coords.x >= 0 && coords.y >= 0 && coords.x < grid.tileCountX && coords.y < grid.tileCountY)
                {
                    tiles.Add(coords);
                }
            }
        }
        return tiles;
    }
}