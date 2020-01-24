using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataVisualizer : MonoBehaviour
{
    public enum DataType { mixed, slope, temperature, humidity, biome, mixed2 }
    public TileGrid grid;
    public DataType dataType;
    public GridRenderer gridRenderer;
    DataType prevDataType;
    public float slopeMult;
    public AnimationCurve elevationPreGradient;
    public Gradient elevationGradient;
    public Gradient slopeGradient;
    public Gradient temperatureGradient;
    public Gradient humidityGradient;
    public GridMesh mesh;
    public Gradient waterGradient;
    public Color oceanColor;
    public float heightScale;
    public float elevationColorOffset;
    public bool enable3D;

    public Color[] biomeColors;

    void Start()
    {
        gridRenderer.Initialize(grid.chunkSize, grid.chunkCountX, grid.chunkCountY, 1);
        if (enable3D)
        {
            mesh.Initialize(grid.chunkSize, grid.chunkCountX, grid.chunkCountY);
        }
    }

    void Update()
    {
        if (prevDataType != dataType)
        {
            UpdateTexture();
            prevDataType = dataType;
        }
    }

    public void UpdateTexture()
    {
        for (int i = 0; i < grid.tileCountX; i++)
        {
            for (int j = 0; j < grid.tileCountY; j++)
            {
                Color c = GetColor(i, j);
                gridRenderer.SetTile(i, j, c);
                if (enable3D)
                {
                    ChannelInfo info = GetChannelInfo(i, j);
                    float h = GetHeight(i, j);
                    mesh.SetTile(i, j, h * heightScale, c, info);
                }
            }
        }
    }

    ChannelInfo GetChannelInfo(int x, int y) {
        ChannelInfo inf = ChannelInfo.Empty;
        inf.active[0] = true;
        inf.color[0] = Color.red;
        return inf;
    }

    Color GetColor(int x, int y)
    {
        switch (dataType)
        {
            case DataType.biome:
                float val = elevationPreGradient.Evaluate(grid.tiles[x, y].elevation + elevationColorOffset);
                if (grid.tiles[x, y].waterType == WaterType.River)
                {
                    return waterGradient.Evaluate(val);
                }
                if (grid.tiles[x, y].waterType == WaterType.Ocean)
                {
                    return oceanColor;
                }
                return biomeColors[(int)grid.tiles[x, y].biome];
            case DataType.mixed:
                val = elevationPreGradient.Evaluate(grid.tiles[x, y].elevation + elevationColorOffset);
                if (grid.tiles[x, y].waterType == WaterType.River)
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
                return new Color(grid.tiles[x,y].temperature,0,grid.tiles[x,y].humidity);

            default:
                //Debug.LogWarning("couldnt render data type");
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