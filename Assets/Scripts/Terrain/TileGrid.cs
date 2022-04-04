using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public Tile[,] tiles;
    public TileWall[] tileWalls;
    //public GridRenderer gridRenderer;
    //public TileGraphics graphics;
    public int chunkSize;
    public int chunkCountX;
    public int chunkCountY;
    public int tileCountX;
    public int tileCountY;
    public int wallCount;

    void Awake()
    {
        tileCountX = chunkCountX * chunkSize;
        tileCountY = chunkCountY * chunkSize;
        wallCount = tileCountX * tileCountY * 3;
        tiles = new Tile[chunkCountX * chunkSize, chunkCountY * chunkSize];
        tileWalls = new TileWall[wallCount];
        CreateTiles();
    }

    private void Start()
    {

        //gridRenderer.Initialize(chunkSize, chunkCountX, chunkCountY, graphics.tileResolution);
    }

    public void CreateTiles()
    {
        for (int i = 0; i < tileCountX; i++)
        {
            for (int j = 0; j < tileCountY; j++)
            {
                tiles[i, j] = new Tile(new Vector2Int(i,j));
            }
        }
    }

    public Tile GetTile(Vector2Int pos) {
        if (InBounds(pos))
        {
            return tiles[pos.x, pos.y];
        }
        return null;
    }

    public List<Vector2Int> HexSearch(Vector2Int center, int radius, bool includeCenter)
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
                if (coords.x >= 0 && coords.y >= 0 && coords.x < tileCountX && coords.y < tileCountY)
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
                if (coords.x >= 0 && coords.y >= 0 && coords.x < tileCountX && coords.y < tileCountY)
                {
                    tiles.Add(coords);
                }
            }
        }
        return tiles;
    }

    public bool InBounds(Vector2Int pos) {
        return pos.x >= 0 && pos.y >= 0 && pos.x < tileCountX && pos.y < tileCountY;
    }

    public int WallIndex(Vector2Int pos, HexDirection direction) {
        if ((int)direction > 2) {
            pos += HexMetric.Step(pos, direction);
            direction -= 3;
        }
        if (!InBounds(pos)) {
            return -1;
        }
        if (direction == HexDirection.E && pos.x == tileCountX - 1)
        {
            return -1;
        }
        if (direction == HexDirection.NE && (pos.y == tileCountY - 1 || (pos.x == tileCountX - 1 && pos.y % 2 == 1))) {
            return -1;
        }
        if (direction == HexDirection.NW && (pos.y == tileCountY - 1 || (pos.x == 0 && pos.y % 2 == 0)))
        {
            return -1;
        }
        return (pos.y * tileCountX + pos.x) * 3 + (int)direction;
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
