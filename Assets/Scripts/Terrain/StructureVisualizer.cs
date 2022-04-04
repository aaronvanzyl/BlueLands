using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureVisualizer : MonoBehaviour
{
    public TileGrid grid;
    public GameObject buildingPrefab;
    public Vector3 treeMinScale;
    public Vector3 treeMaxScale;
    public Vector3 cityBuildingMinScale;
    public Vector3 cityBuildingMaxScale;
    List<GameObject>[,] tileStructures;
    GameObject structureParent;
    


    public void Initialize(int tileCountX, int tileCountY)
    {
        structureParent = new GameObject("Structures");
        tileStructures = new List<GameObject>[tileCountX, tileCountY];
        for (int i = 0; i < tileCountX; i++)
        {
            for (int j = 0; j < tileCountY; j++)
            {
                tileStructures[i, j] = new List<GameObject>();
            }
        }
    }

    public void UpdateTile(Tile tile, int x, int y, float height, bool enableClutter)
    {
        foreach (GameObject g in tileStructures[x, y])
        {
            Destroy(g);
        }
        tileStructures[x, y].Clear();
        if (enableClutter)
        {
            if (tile.biome!=null && tile.biome.treePrefab != null)
            {
                SurfaceClutter(x, y, height, tile.biome.treePrefab, tile.trees, treeMinScale, treeMaxScale, tile.seed);
            }
        }
        if (tile.city != null) {
            CreateCity(x, y, height, tile.seed, enableClutter);
        }
    }

    void CreateCity(int x, int y, float height, int seed, bool enableClutter) {
        Random.InitState(seed);
        int count = Random.Range(2, 4);
        if (enableClutter)
        {
            SurfaceClutter(x, y, height, buildingPrefab, count, cityBuildingMinScale, cityBuildingMaxScale, seed);
        }
        //Random.InitState(seed);
        //Vector3 pos = HexMetric.XYToWorldCoords(new Vector2Int(x,y), height);
        //pos += Vector3.up * cityPrefab.transform.localScale.y * 0.5f;
        //Quaternion rot = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
        //GameObject g = Instantiate(cityPrefab, pos, rot);
        //tileStructures[x, y].Add(g);
    }

    void SurfaceClutter(int x, int y, float height, GameObject prefab, int count, Vector3 minScale, Vector3 maxScale, int seed)
    {
        Random.InitState(seed);
        float rotation = Random.value * 2 * Mathf.PI;
        float angleDif = (2 * Mathf.PI) / count;
        for (int i = 0; i < count; i++)
        {
            float a = rotation + (i * angleDif) + angleDif * Random.value * 0.8f;
            Vector3 offset = new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a));
            offset = offset.normalized * 0.5f * Random.Range(0.4f, 0.8f);
            Vector3 pos = HexMetric.XYToWorldCoords(x, y, height) + offset;
            Vector3 localScale = Vector3.Scale(prefab.transform.localScale, MathUtility.RandVector3(minScale, maxScale));
            pos += Vector3.up * localScale.y * 0.5f;
            Quaternion rot = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
            GameObject g = Instantiate(prefab, pos, rot, structureParent.transform);
            g.transform.localScale = localScale;
            tileStructures[x, y].Add(g);
        }
    }
}
