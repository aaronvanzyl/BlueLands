using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataVisualizer : MonoBehaviour
{
    public enum DataType { mixed, slope, temperature, humidity, biome, mixed2, cityAppeal }

    [Header("References")]
    public StructureVisualizer structureVisualizer;
    public GridRenderer gridRenderer;
    public TileGrid grid;
    public GridMesh mesh;

    [Header("General")]
    public DataType dataType;
    DataType prevDataType;

    [Header("Elevation")]
    public bool enable3D;
    public float heightScale;
    public float slopeMult;
    public AnimationCurve elevationPreGradient;

    [Header("Color gradients")]
    public float elevationColorOffset;
    public Gradient elevationGradient;
    public Gradient slopeGradient;
    public Gradient temperatureGradient;
    public Gradient humidityGradient;
    public Gradient waterGradient;
    public Gradient cityAppealGradient;

    [Header("Water")]
    public bool enableRivers;
    public Color oceanColor;
    public Color deepOceanColor;
    public Color riverColor;

    [Header("Walls")]
    public bool enableWalls;
    public Color wallColor;
    public float wallHeight;
    public float wallThickness;

    [Header("Clutter")]
    public bool enableClutter;
    bool prevEnableClutter = false;

    void Start()
    {
        if (gridRenderer != null)
        {
            gridRenderer.Initialize(grid.chunkSize, grid.chunkCountX, grid.chunkCountY, 1);
        }
        if (enable3D)
        {
            mesh.Initialize(grid.chunkSize, grid.chunkCountX, grid.chunkCountY);
            structureVisualizer.Initialize(grid.tileCountX, grid.tileCountY);
        }
    }

    void Update()
    {
        if (prevDataType != dataType)
        {
            UpdateTexture();
            prevDataType = dataType;
        }
        if (prevEnableClutter != enableClutter)
        {
            UpdateStructures();
            prevEnableClutter = enableClutter;
        }
    }

    void UpdateStructures()
    {
        for (int i = 0; i < grid.tileCountX; i++)
        {
            for (int j = 0; j < grid.tileCountY; j++)
            {
                structureVisualizer.UpdateTile(grid.tiles[i, j], i, j, GetHeight(i, j) * heightScale, enableClutter);
            }
        }
    }

    public void UpdateAll()
    {
        UpdateTexture();
        UpdateStructures();
    }

    void UpdateTexture()
    {
        for (int i = 0; i < grid.tileCountX; i++)
        {
            for (int j = 0; j < grid.tileCountY; j++)
            {
                Color c = GetColor(i, j);
                if (gridRenderer != null)
                {
                    gridRenderer.SetTile(i, j, c);
                }
                if (enable3D)
                {
                    ChannelInfo info = GetChannelInfo(i, j);
                    float h = GetHeight(i, j);
                    mesh.SetTile(i, j, h * heightScale, c, info);
                }
            }
        }

        for (int i = 0; i < grid.wallCount; i++)
        {
            WallInfo info = GetWallInfo(i);
            mesh.SetWallInfo(info, i);
        }
    }

    WallInfo GetWallInfo(int index)
    {
        WallInfo info = new WallInfo();
        if (enableWalls)
        {
            info.active = grid.tileWalls[index].active;
            info.color = wallColor;
            info.thickness = wallThickness;
            info.height = wallHeight;
        }
        else {
            info.active = false;
        }
        return info;
    }

    ChannelInfo GetChannelInfo(int x, int y)
    {
        ChannelInfo inf = new ChannelInfo();
        if (enableRivers)
        {
            float val = elevationPreGradient.Evaluate(grid.tiles[x, y].elevation + elevationColorOffset);
            for (int i = 0; i < 6; i++)
            {
                inf.active[i] = grid.tiles[x, y].connections[i] != ConnectionType.None;
                //inf.color[i] = riverColor;
                inf.color[i] = waterGradient.Evaluate(val);
            }
        }
        return inf;
    }

    Color GetColor(int x, int y)
    {
        switch (dataType)
        {
            case DataType.biome:
                float val = elevationPreGradient.Evaluate(grid.tiles[x, y].elevation + elevationColorOffset);
                //if (grid.tiles[x, y].waterType == WaterType.River)
                //{
                //    return waterGradient.Evaluate(val);
                //}
                if (grid.tiles[x, y].waterType == WaterType.Ocean)
                {
                    return oceanColor;
                }
                if (grid.tiles[x, y].waterType == WaterType.DeepOcean)
                {
                    return deepOceanColor;
                }
                if (grid.tiles[x, y].biome != null)
                {
                    return grid.tiles[x, y].biome.color;
                }
                return Color.magenta;
            //return biomeColors[(int)grid.tiles[x, y].biome];
            case DataType.mixed:
                val = elevationPreGradient.Evaluate(grid.tiles[x, y].elevation + elevationColorOffset);
                if (enableRivers && grid.tiles[x, y].waterType == WaterType.River)
                {
                    return waterGradient.Evaluate(val);
                }
                return elevationGradient.Evaluate(val);
            case DataType.slope:
                float s = 0;
                for (int i = Mathf.Max(0, x - 1); i < Mathf.Min(grid.tileCountX, x + 2); i++)
                {
                    for (int j = Mathf.Max(0, y - 1); j < Mathf.Min(grid.tileCountY, y + 2); j++)
                    {
                        //if (i != x || j != y)
                        //{
                        s += Mathf.Abs(grid.tiles[x, y].elevation - grid.tiles[i, j].elevation);
                        //}
                    }
                }
                return slopeGradient.Evaluate(s * slopeMult);
            case DataType.temperature:
                return temperatureGradient.Evaluate(grid.tiles[x, y].temperature);
            case DataType.humidity:
                return humidityGradient.Evaluate(grid.tiles[x, y].humidity);
            case DataType.mixed2:
                return new Color(grid.tiles[x, y].temperature, 0, grid.tiles[x, y].humidity);
            case DataType.cityAppeal:
                return cityAppealGradient.Evaluate(grid.tiles[x, y].cityAppeal);
            default:
                Debug.LogWarning("couldnt render data type");
                break;
        }
        return Color.black;
    }

    float GetHeight(int x, int y)
    {
        switch (dataType)
        {
            case DataType.slope:
                float s = 0;
                for (int i = Mathf.Max(0, x - 1); i < Mathf.Min(grid.tileCountX, x + 2); i++)
                {
                    for (int j = Mathf.Max(0, y - 1); j < Mathf.Min(grid.tileCountY, y + 2); j++)
                    {
                        //if (i != x || j != y)
                        //{
                        s += Mathf.Abs(grid.tiles[x, y].elevation - grid.tiles[i, j].elevation);
                        //}
                    }
                }
                return s * slopeMult;
            default:
                return grid.tiles[x, y].elevation;
                //Debug.LogWarning("couldnt height map data type");
        }
    }
}