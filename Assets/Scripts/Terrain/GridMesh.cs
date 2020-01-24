using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct ChannelInfo
{
    public static ChannelInfo Empty => new ChannelInfo(new Color[6], new bool[6]);
    public Color[] color;
    public bool[] active;

    public ChannelInfo(Color[] color, bool[] active)
    {
        this.color = color;
        this.active = active;
    }
}

public class GridMesh : MonoBehaviour
{
    public GridMeshChunk chunkPrefab;
    GridMeshChunk[,] chunks;
    public float[,] tileHeights;
    public Color[,] tileColors;
    public ChannelInfo[,] channelInfo;
    //public Color[,,] 
    int chunkSize;
    public int tileCountX;
    public int tileCountY;

    public void Initialize(int chunkSize, int chunkCountX, int chunkCountY)
    {
        tileCountX = chunkSize * chunkCountX;
        tileCountY = chunkSize * chunkCountY;
        tileHeights = new float[chunkSize * chunkCountX, chunkSize * chunkCountY];
        tileColors = new Color[chunkSize * chunkCountX, chunkSize * chunkCountY];
        this.chunkSize = chunkSize;
        chunks = new GridMeshChunk[chunkCountX, chunkCountY];
        for (int i = 0; i < chunkCountX; i++)
        {
            for (int j = 0; j < chunkCountY; j++)
            {

                //chunks[i, j] = Instantiate(chunkPrefab, new Vector3(i * chunkSize, 0, j * chunkSize / GridMeshChunk.hexOuterRatio), Quaternion.identity, transform);
                chunks[i, j] = Instantiate(chunkPrefab, Vector3.zero, Quaternion.identity, transform);
                chunks[i, j].Initialize(this, chunkSize, i * chunkSize, j * chunkSize);
            }
        }
    }

    public void SetTile(int x, int y, float height, Color color, ChannelInfo info)
    {
        tileColors[x, y] = color;
        tileHeights[x, y] = height;
        //channelInfo[x, y] = info;

        int chunkX = (int)((float)x / chunkSize);
        int chunkY = (int)((float)y / chunkSize);

        chunks[chunkX, chunkY].needUpdate = true;
        //chunks[chunkX, chunkY].SetTile(x - chunkX * chunkSize, y - chunkY * chunkSize, height, color);
    }
}