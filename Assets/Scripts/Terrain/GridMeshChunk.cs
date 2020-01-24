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
    public GenericMeshChunk riverMesh;
    public GenericMeshChunk roadMesh; 
    
    
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
        for (int i = offsetX; i < offsetX + size; i++)
        {
            for (int j = offsetY; j < offsetY + size; j++)
            {
                TriangulateHex(i, j);
            }
        }
        terrainMesh.UpdateMesh();
    }

    void TriangulateHex(int x, int y) {
        /*Vector3 v00 = new Vector3(i, tileHeights[i, j], j);
                Vector3 v10 = new Vector3(i + 1, tileHeights[i, j], j);
                Vector3 v01 = new Vector3(i, tileHeights[i, j], j + 1);
                Vector3 v11 = new Vector3(i + 1, tileHeights[i, j], j + 1);*/

        //TriangulateRectangle(v00, v10, v01, v11, tileColors[i, j]);

        Vector3 hexCenter = HexMetric.WorldCoords(x, y);
        hexCenter.y = gMesh.tileHeights[x, y];
        Color col = gMesh.tileColors[x, y];
        terrainMesh.AddHex(hexCenter, 0.5f * HexMetric.hexOuterRatio, gMesh.tileColors[x, y]);
        //AddHexConnection(i, j, i + 1, j);

        if (x < gMesh.tileCountX - 1)
        {
            terrainMesh.AddHexConnection(hexCenter, col,
                HexMetric.WorldCoords(x + 1, y, gMesh.tileHeights[x + 1, y]), gMesh.tileColors[x + 1, y]);
        }
        if (y < gMesh.tileCountY - 1)
        {
            terrainMesh.AddHexConnection(hexCenter, col,
                HexMetric.WorldCoords(x, y + 1, gMesh.tileHeights[x, y + 1]), gMesh.tileColors[x, y + 1]);
            if (y % 2 == 0 && x > 0)
            {
                terrainMesh.AddHexConnection(hexCenter, col,
                HexMetric.WorldCoords(x - 1, y + 1, gMesh.tileHeights[x - 1, y + 1]), gMesh.tileColors[x - 1, y + 1]);
            }
            else if (y % 2 == 1 && x < gMesh.tileCountX - 1)
            {
                terrainMesh.AddHexConnection(hexCenter, col,
                HexMetric.WorldCoords(x + 1, y + 1, gMesh.tileHeights[x + 1, y + 1]), gMesh.tileColors[x + 1, y + 1]);
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

    void TriangulateChannels(int x, int y) {
        ChannelInfo info = gMesh.channelInfo[x, y];
        for (int i = 0; i < 6; i++) {
            
        }
    }
}
