using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public Tile[,] tiles;
    public GridRenderer gridRenderer;
    public int chunkSize;
    public int chunkCountX;
    public int chunkCountY;
    public int tileCountX;
    public int tileCountY;
    public TileGraphics graphics;

    void Awake()
    {
        tileCountX = chunkCountX * chunkSize;
        tileCountY = chunkCountY * chunkSize;
        tiles = new Tile[chunkCountX * chunkSize, chunkCountY * chunkSize];
        CreateTiles();
    }

    private void Start()
    {
        gridRenderer.Initialize(chunkSize, chunkCountX, chunkCountY, graphics.tileResolution);
    }

    public void CreateTiles()
    {
        for (int i = 0; i < tileCountX; i++)
        {
            for (int j = 0; j < tileCountY; j++)
            {
                tiles[i, j] = new Tile();
            }
        }
    }

    //public void SetTileType(int x, int y, TileType type)
    //{
    //    if (type != tiles[x, y].type)
    //    {
    //        tiles[x, y].type = type;
    //        gridRenderer.SetTile(x, y, graphics.GetTexture(tiles[x, y]));
            
    //    }
    //}

}
