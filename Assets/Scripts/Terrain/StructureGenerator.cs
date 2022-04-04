using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureGenerator : MonoBehaviour
{
    public TileGrid grid;
    public int maxCities;
    public int minCitySize;
    public int maxCitySize;
    public float adjRiverAdd;
    public float adjOceanAdd;
    public float cityAdjMod;
    public float cityAdjDecay;
    float[,] crowdingStatus;

    private void Start()
    {
        crowdingStatus = new float[grid.tileCountX, grid.tileCountY];
    }

    public void GenerateStructures()
    {
        GenerateCities();
    }

    public void CalculateCityAppeal()
    {
        for (int i = 0; i < grid.tileCountX; i++)
        {
            for (int j = 0; j < grid.tileCountY; j++)
            {
                grid.tiles[i, j].cityAppeal = BaseCityAppeal(grid.tiles[i, j]);
            }
        }
    }

    void GenerateCities()
    {
        float sumAppeal = SumAppeal();
        int citiesPlaced = 0;
        while (citiesPlaced < maxCities && sumAppeal > 0)
        {
            float rand = Random.Range(0, sumAppeal);
            for (int i = 0; i < grid.tileCountX; i++)
            {
                for (int j = 0; j < grid.tileCountY; j++)
                {
                    rand -= ActualAppeal(grid.tiles[i, j]);
                    if (rand <= 0)
                    {
                        citiesPlaced++;
                        CreateCity(new Vector2Int(i, j));
                        ModifyByDist(crowdingStatus, new Vector2Int(i, j), cityAdjMod, cityAdjDecay);
                        break;
                    }
                }
                if (rand <= 0)
                {
                    break;
                }
            }
            sumAppeal = SumAppeal();
        }
    }

    void CreateCity(Vector2Int pos)
    {
        //Debug.Log($"city at {pos.x},{pos.y}");
        City city = new City();

        List<Vector2Int> cityTiles = new List<Vector2Int>();
        List<Vector2Int> adjTiles = new List<Vector2Int>();
        adjTiles.Add(pos);
        int placed = 0;
        int count = Random.Range(minCitySize, maxCitySize);
        while (placed < count && adjTiles.Count > 0)
        {
            int idx = Random.Range(0, adjTiles.Count);

            Vector2Int tile = adjTiles[idx];
            bool rm = true;
            while (rm)
            {
                rm = adjTiles.Remove(tile);
            }
            grid.GetTile(tile).city = city;
            grid.GetTile(tile).trees = 0;
            cityTiles.Add(tile);
            for (int i = 0; i < 6; i++)
            {
                Vector2Int adj = tile + HexMetric.Step(tile, (HexDirection)i);
                if (grid.InBounds(adj) && !cityTiles.Contains(adj) && grid.GetTile(adj).waterType == WaterType.None)
                {
                    adjTiles.Add(adj);
                }
            }
            placed++;
        }
        foreach (Vector2Int v in cityTiles)
        {
            for (int i = 0; i < 6; i++)
            {
                Vector2Int adj = v + HexMetric.Step(v, (HexDirection)i);
                if (!cityTiles.Contains(adj))
                {
                    int idx = grid.WallIndex(v, (HexDirection)i);
                    if (idx >= 0 && idx < grid.tileWalls.Length) {
                        grid.tileWalls[idx].active = true;
                    }
                }
            }
        }
        //grid.tileWalls[grid.WallIndex(pos, HexDirection.W)].active = true;
    }

    void ModifyByDist(float[,] values, Vector2Int origin, float baseModify, float linearDecayPerTile)
    {
        for (int i = 0; i < grid.tileCountX; i++)
        {
            for (int j = 0; j < grid.tileCountY; j++)
            {
                values[i, j] += baseModify - baseModify * Mathf.Max(linearDecayPerTile * HexMetric.HexDist(origin, new Vector2Int(i, j)), 0);
            }
        }
    }

    float BaseCityAppeal(Tile t)
    {
        if (t.waterType != WaterType.None)
        {
            return 0;
        }
        float v = t.biome.baseAppeal;
        bool adjRiver = false;
        bool adjOcean = false;
        for (int i = 0; i < 6; i++)
        {
            Vector2Int adjPos = t.pos + HexMetric.Step(t.pos, (HexDirection)i);
            Tile adj = grid.GetTile(adjPos);

            if (adj != null)
            {
                if (adj.waterType == WaterType.River)
                {
                    adjRiver = true;
                }
                else if (adj.waterType == WaterType.Ocean)
                {
                    adjOcean = true;
                }
            }
        }
        if (adjRiver)
        {
            v += adjRiverAdd;
        }
        if (adjOcean)
        {
            v += adjOceanAdd;
        }
        return Mathf.Max(v, 0);
    }

    float ActualAppeal(Tile t)
    {
        return Mathf.Max(t.cityAppeal + crowdingStatus[t.pos.x, t.pos.y], 0);
    }

    float SumAppeal()
    {
        float sumAppeal = 0;
        for (int i = 0; i < grid.tileCountX; i++)
        {
            for (int j = 0; j < grid.tileCountY; j++)
            {
                sumAppeal += ActualAppeal(grid.tiles[i, j]);
            }
        }
        return sumAppeal;
    }
}
