using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelInfo
{
    public Color[] color;
    public int[] layer;
    public bool[] active;

    public ChannelInfo() {
        color = new Color[6];
        layer = new int[6];
        active = new bool[6];
    }

    public ChannelInfo(Color[] color, int[] layer, bool[] active)
    {
        this.color = color;
        this.layer = layer;
        this.active = active;
    }
}

public class WallInfo
{
    public bool active;
    public float height;
    public Color color;
    public float thickness;

    public WallInfo()
    {
        active = false;
        color = Color.magenta;
    }

    public WallInfo(Color color, bool active, float height, float thickness)
    {
        this.color = color;
        this.active = active;
        this.height = height;
        this.thickness = thickness;
    }
}

public class GridMesh : MonoBehaviour
{
    public GridMeshChunk chunkPrefab;
    GridMeshChunk[,] chunks;
    public float[,] tileHeights;
    public Color[,] tileColors;
    public WallInfo[] wallInfo;
    public ChannelInfo[,] channelInfo;
    int chunkSize;
    public int tileCountX;
    public int tileCountY;
    public bool enableBorders;
    public float borderWidth;
    public Color borderColor;

    public void Initialize(int chunkSize, int chunkCountX, int chunkCountY)
    {
        tileCountX = chunkSize * chunkCountX;
        tileCountY = chunkSize * chunkCountY;
        tileHeights = new float[chunkSize * chunkCountX, chunkSize * chunkCountY];
        tileColors = new Color[chunkSize * chunkCountX, chunkSize * chunkCountY];
        channelInfo = new ChannelInfo[chunkSize * chunkCountX, chunkSize * chunkCountY];
        wallInfo = new WallInfo[tileCountX * tileCountY * 3];

        for (int i = 0; i < wallInfo.Length; i++) {
            wallInfo[i] = new WallInfo();
        }

        for (int i = 0; i < channelInfo.GetLength(0); i++) {
            for (int j = 0; j < channelInfo.GetLength(1); j++) {
                channelInfo[i, j] = new ChannelInfo();
            }
        }
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

    public void SetTile(int x, int y, float height, Color color, ChannelInfo channelInfo)
    {
        tileColors[x, y] = color;
        tileHeights[x, y] = height;
        this.channelInfo[x, y] = channelInfo;

        int chunkX = (int)((float)x / chunkSize);
        int chunkY = (int)((float)y / chunkSize);

        chunks[chunkX, chunkY].needUpdate = true;
        //chunks[chunkX, chunkY].SetTile(x - chunkX * chunkSize, y - chunkY * chunkSize, height, color);
    }

    public void SetWallInfo(WallInfo info, int index) {
        wallInfo[index] = info;
        Vector2Int chunkIdx = WallToChunkIndex(index);
        chunks[chunkIdx.x, chunkIdx.y].needUpdate = true;
    }

    public bool InBounds(Vector2Int v)
    {
        return v.x >= 0 && v.y >= 0 && v.x < tileCountX && v.y < tileCountY;
    }

    public Vector2Int WallToChunkIndex(int wallIndex)
    {
        wallIndex /= 3;
        int x = wallIndex % tileCountX;
        int y = wallIndex / tileCountX;
        return new Vector2Int(x/chunkSize, y/chunkSize);
    }

    public int WallIndex(int x, int y, HexDirection direction) {
        Vector2Int pos = new Vector2Int(x, y);
		if ((int)direction > 2)
		{
			pos += HexMetric.Step(pos, direction);
			direction -= 3;
		}
		if (!InBounds(pos))
		{
			return -1;
		}
		if (direction == HexDirection.E && pos.x == tileCountX - 1)
		{
			return -1;
		}
		if (direction == HexDirection.NE && (pos.y == tileCountY - 1 || (pos.x == tileCountX - 1 && pos.y % 2 == 1)))
		{
			return -1;
		}
		if (direction == HexDirection.NW && (pos.y == tileCountY - 1 || (pos.x == 0 && pos.y % 2 == 0)))
		{
			return -1;
		}
		return (pos.y * tileCountX + pos.x) * 3 + (int)direction;
	}
}