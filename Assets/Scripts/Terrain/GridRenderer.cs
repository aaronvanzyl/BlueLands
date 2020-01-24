using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    public GridChunk chunkPrefab;
    GridChunk[,] chunks;
    int chunkSize;

    public void Initialize(int chunkSize, int chunkCountX, int chunkCountY, int resolution)
    {
        this.chunkSize = chunkSize;
        chunks = new GridChunk[chunkCountX, chunkCountY];
        for (int i = 0; i < chunkCountX; i++)
        {
            for (int j = 0; j < chunkCountY; j++)
            {
                chunks[i, j] = Instantiate(chunkPrefab, new Vector3(i, j), Quaternion.identity, transform);
                chunks[i, j].CreateTexture(chunkSize, resolution);
            }
        }
    }

    public void SetTile(int x, int y, Color color)
    {
        int chunkX = (int)((float)x / chunkSize);
        int chunkY = (int)((float)y / chunkSize);
        chunks[chunkX, chunkY].SetTile(x - chunkX * chunkSize, y - chunkY * chunkSize, color);
    }

    public void SetTile(int x, int y, Texture2D t)
    {
        int chunkX = (int)((float)x / chunkSize);
        int chunkY = (int)((float)y / chunkSize);
        chunks[chunkX, chunkY].SetTile(x - chunkX * chunkSize, y - chunkY * chunkSize, t);
    }
}
