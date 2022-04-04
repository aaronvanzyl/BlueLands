using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMeshChunk : MonoBehaviour
{
    GridMesh gMesh;
    public int size;
    int offsetX;
    int offsetY;

    public bool needUpdate;

    public GenericMeshChunk terrainMesh;
    public GenericMeshChunk channelMesh;
    public GenericMeshChunk wallMesh;

    public void Initialize(GridMesh gMesh, int size, int offsetX, int offsetY)
    {
        this.size = size;
        this.offsetX = offsetX;
        this.offsetY = offsetY;
        this.gMesh = gMesh;
        for (int i = offsetX; i < offsetX + size; i++)
        {
            for (int j = offsetY; j < offsetY + size; j++)
            {
                gMesh.tileHeights[i, j] = 0;
                gMesh.tileColors[i, j] = Color.HSVToRGB(Random.Range(0f, 1f), 1, 1);
            }
        }
        Triangulate();
    }

    private void LateUpdate()
    {
        if (needUpdate)
        {
            Triangulate();
            needUpdate = false;
        }
    }

    void Triangulate()
    {
        terrainMesh.Clear();
        channelMesh.Clear();
        wallMesh.Clear();
        for (int i = offsetX; i < offsetX + size; i++)
        {
            for (int j = offsetY; j < offsetY + size; j++)
            {
                TriangulateHex(i, j);
                TriangulateChannels(i, j);
                TriangulateWalls(i, j);
            }
        }
        terrainMesh.UpdateMesh();
        channelMesh.UpdateMesh();
        wallMesh.UpdateMesh();
    }

    void TriangulateHex(int x, int y)
    {
        /*Vector3 v00 = new Vector3(i, tileHeights[i, j], j);
                Vector3 v10 = new Vector3(i + 1, tileHeights[i, j], j);
                Vector3 v01 = new Vector3(i, tileHeights[i, j], j + 1);
                Vector3 v11 = new Vector3(i + 1, tileHeights[i, j], j + 1);*/

        //TriangulateRectangle(v00, v10, v01, v11, tileColors[i, j]);

        Vector3 hexCenter = HexMetric.XYToWorldCoords(x, y, gMesh.tileHeights[x, y]);
        Color col = gMesh.tileColors[x, y];
        terrainMesh.AddHex(hexCenter, 0.5f * HexMetric.hexInnerToOuterRatio, gMesh.tileColors[x, y]);
        //AddHexConnection(i, j, i + 1, j);

        if (x < gMesh.tileCountX - 1)
        {
            terrainMesh.AddHexConnection(hexCenter, col,
                HexMetric.XYToWorldCoords(x + 1, y, gMesh.tileHeights[x + 1, y]), gMesh.tileColors[x + 1, y]);
        }
        if (y < gMesh.tileCountY - 1)
        {
            terrainMesh.AddHexConnection(hexCenter, col,
                HexMetric.XYToWorldCoords(x, y + 1, gMesh.tileHeights[x, y + 1]), gMesh.tileColors[x, y + 1]);
            if (y % 2 == 0 && x > 0)
            {
                terrainMesh.AddHexConnection(hexCenter, col,
                HexMetric.XYToWorldCoords(x - 1, y + 1, gMesh.tileHeights[x - 1, y + 1]), gMesh.tileColors[x - 1, y + 1]);
            }
            else if (y % 2 == 1 && x < gMesh.tileCountX - 1)
            {
                terrainMesh.AddHexConnection(hexCenter, col,
                HexMetric.XYToWorldCoords(x + 1, y + 1, gMesh.tileHeights[x + 1, y + 1]), gMesh.tileColors[x + 1, y + 1]);
            }
        }
        /*if (i < size - 1)
        {
            Vector3 vr0 = new Vector3(i + 1, tileHeights[i + 1, j], j);
            Vector3 vr1 = new Vector3(i + 1, tileHeights[i + 1, j], j+1);
            Color c = tileHeights[i, j] > tileHeights[i + 1, j] ? tileColors[i,j] : tileColors[i + 1, j];
            TriangulateRectangle(v10, vr0, v11, vr1, c);
        }

        if (j < size - 1)
        {
            Vector3 vt0 = new Vector3(i, tileHeights[i, j+1], j+1);
            Vector3 vt1 = new Vector3(i + 1, tileHeights[i, j+1], j + 1);
            Color c = tileHeights[i, j] > tileHeights[i, j+1] ? tileColors[i, j] : tileColors[i, j+1];
            TriangulateRectangle(v01, v11, vt0, vt1, c);
        }*/
    }

    void TriangulateWalls(int x, int y)
    {
        for (int i = 0; i < 3; i++)
        {
            int wallIdx = gMesh.WallIndex(x, y, (HexDirection)i);
            if (wallIdx != -1)
            {
                WallInfo wall = gMesh.wallInfo[wallIdx];
                if (wall.active)
                {
                    Vector3 center1 = HexMetric.XYToWorldCoords(x, y, gMesh.tileHeights[x, y]);
                    Vector2Int pos2 = new Vector2Int(x, y) + HexMetric.Step(new Vector2Int(x, y), (HexDirection)i);
                    Vector3 center2 = HexMetric.XYToWorldCoords(pos2.x, pos2.y, gMesh.tileHeights[pos2.x, pos2.y]);
                    Vector2Int posl = new Vector2Int(x, y) + HexMetric.Step(new Vector2Int(x, y), ((HexDirection)i).Next());
                    Vector2Int posr = new Vector2Int(x, y) + HexMetric.Step(new Vector2Int(x, y), ((HexDirection)i).Prev());
                    if (gMesh.InBounds(posl) && gMesh.InBounds(posr) && gMesh.InBounds(pos2))
                    {
                        float lElev = gMesh.tileHeights[posl.x, posl.y];
                        float rElev = gMesh.tileHeights[posr.x, posr.y];
                        //Debug.Log($"{lElev},{center2.y},{rElev}");
                        bool wallNearL = gMesh.wallInfo[gMesh.WallIndex(x, y, ((HexDirection)i).Next())].active;
                        bool wallNearR = gMesh.wallInfo[gMesh.WallIndex(x, y, ((HexDirection)i).Prev())].active;

                        bool wallFarL = gMesh.wallInfo[gMesh.WallIndex(pos2.x, pos2.y, ((HexDirection)i).Opposite().Prev())].active;
                        bool wallFarR = gMesh.wallInfo[gMesh.WallIndex(pos2.x, pos2.y, ((HexDirection)i).Opposite().Next())].active;
                        wallMesh.AddHexWall(center1, center2, lElev, rElev, wall.color, wall.height, wall.thickness, wallNearL, wallFarL, wallNearR, wallFarR);
                        //Debug.Log($"Active wall at {x},{y} - {i}");
                        //Debug.DrawRay(HexMetric.XYToWorldCoords(x, y, gMesh.tileHeights[x, y]), Vector3.up * 100, Color.red, 1000);
                    }
                }
            }
        }
    }

    void TriangulateChannels(int x, int y)
    {
        ChannelInfo info = gMesh.channelInfo[x, y];
        Vector3 hexCenter = HexMetric.XYToWorldCoords(x, y, gMesh.tileHeights[x, y]);
        Vector2Int selfXY = new Vector2Int(x, y);
        for (int i = 0; i < 6; i++)
        {
            if (info.active[i])
            {

                //channelMesh.AddHexSide(hexCenter + Vector3.up*0.0001f, 0.5f * HexMetric.hexInnerToOuterRatio, info.color[i], (HexDirection)i);32
                channelMesh.AddHexChannel(hexCenter, 0.001f + info.layer[i] * 0.001f, 0.55f, info.color[i], (HexDirection)i);
                Vector2Int otherXY = selfXY + HexMetric.Step(new Vector2Int(x, y), (HexDirection)i);
                if (otherXY.x >= 0 && otherXY.x < gMesh.tileCountX && otherXY.y >= 0 && otherXY.y < gMesh.tileCountY)
                {

                    Vector3 otherCenter = HexMetric.XYToWorldCoords(otherXY, gMesh.tileHeights[otherXY.x, otherXY.y]);
                    channelMesh.AddHexConnection(hexCenter, info.color[i], otherCenter, info.color[i], 0.55f, 0.0001f);
                }
            }
        }
    }
}
