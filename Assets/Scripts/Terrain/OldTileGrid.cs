using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public enum DataType {Elevation, Slope };
public class OldTileGrid : MonoBehaviour
{
    public TileChunk chunkPrefab;
    [Header("Set in inspector")]
    public int chunkSize = 64;
    public int chunkCountX;
    public int chunkCountY;
    public TileGraphics tileGraphics;
    [Header("View only")]
    public int tileCountX;
    public int tileCountY;

    public Tile[,] tiles;
    TileChunk[,] chunks;
    public DataType RenderType {
        get { return RenderType; }
        private set {
            if (value != renderType) {
                renderType = value;
                //UpdateRenderType();
            }
        }
    }
    DataType renderType = DataType.Elevation;


    void Start()
    {
        tileCountX = chunkCountX * chunkSize;
        tileCountY = chunkCountY * chunkSize;
        tiles = new Tile[tileCountX, tileCountY];
        chunks = new TileChunk[chunkCountX, chunkCountY];
        CreateChunks();
        //CreateTiles();

        /*SetTextureMode(true, DataType.Noise);
        for (int i = 0; i < tileCountX; i++) {
            for (int j = 0; j < tileCountY; j++) {
                SetTextureData(i,j,Color.HSVToRGB(Noise.Perlin2D(new Vector2(i,j),1f/chunkSize) * 0.5f + 0.5f, 1,1));
            }
        }*/

    }

    /*void UpdateDataType() {
        for (int i = 1; i < grid.tileCountX - 1; i++)
        {
            for (int j = 1; j < grid.tileCountY - 1; j++)
            {
                float val = 0;
                switch (RenderType) {
                    case RenderType.Texture:

                }
                //float noise = Noise.Layered2D(new Vector2(i, j), frequency, layers, lacunarity, persistence) * 0.5f + 0.5f;
                float slope = Mathf.Abs(grid.tiles[i, j].elevation - grid.tiles[i - 1, j].elevation);
                slope += Mathf.Abs(grid.tiles[i, j].elevation - grid.tiles[i + 1, j].elevation);
                slope += Mathf.Abs(grid.tiles[i, j].elevation - grid.tiles[i, j - 1].elevation);
                slope += Mathf.Abs(grid.tiles[i, j].elevation - grid.tiles[i, j + 1].elevation);
                grid.SetTextureData(i, j, colorGradient.Evaluate(slope * mult));
                //grid.SetTextureData(i, j, Color.HSVToRGB(noise,1,1));
            }
        }
    }*/


    public void SetTextureMode(bool dataMode)
    {
        foreach (TileChunk c in chunks)
        {
            c.SetTextureMode(dataMode);
        }
    }

    public void SetTextureData(int x, int y, Color col)
    {
        int chunkX = x / chunkSize;//Mathf.CeilToInt((float)x / chunkSize) - 1;
        int chunkY = y / chunkSize;//Mathf.CeilToInt((float)y / chunkSize) - 1;
        //Debug.Log(chunkX + " " + chunkY + " " + (x - chunkX * chunkSize) + " " + (y - chunkY * chunkSize));
        chunks[chunkX, chunkY].SetDataTexture(x - chunkX * chunkSize, y - chunkY * chunkSize, col);

    }

    void CreateChunks()
    {
        for (int i = 0; i < chunkCountX; i++)
        {
            for (int j = 0; j < chunkCountY; j++)
            {
                chunks[i, j] = Instantiate(chunkPrefab, new Vector2(i, j), Quaternion.identity, transform);
                chunks[i, j].Initialize(chunkSize, tileGraphics);
                chunks[i, j].name = i + ", " + j;
            }
        }
    }

    /*void CreateTiles()
    {
        for (int i = 0; i < tileCountX; i++)
        {
            for (int j = 0; j < tileCountY; j++)
            {
                Tile tile = tiles[i, j] = new Tile();
                int chunkX = i / chunkSize;
                int chunkY = j / chunkSize;
                tile.localX = i - chunkX * chunkSize;
                tile.localY = j - chunkY * chunkSize;
                tile.chunk = chunks[chunkX, chunkY];
                chunks[chunkX, chunkY].SetTile(tile, tile.localX, tile.localY);

                tile.Type = (TileType)Random.Range(0, 3);
            }
        }
    }*/
}
