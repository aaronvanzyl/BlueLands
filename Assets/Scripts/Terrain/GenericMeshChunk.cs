using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericMeshChunk : MonoBehaviour
{
    MeshFilter meshFilter;
    Mesh mesh;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Color> colors = new List<Color>();
    List<Vector3> normals = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh = new Mesh();
    }

    public void Clear()
    {
        vertices.Clear();
        colors.Clear();
        triangles.Clear();
        normals.Clear();
        uvs.Clear();
    }

    public void UpdateMesh()
    {
        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetColors(colors);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    void AddTriangle(Vector3 v0, Vector3 v1, Vector3 v2, Color color)
    {
        int i = vertices.Count;
        vertices.Add(v0);
        vertices.Add(v1);
        vertices.Add(v2);

        triangles.Add(i);
        triangles.Add(i + 1);
        triangles.Add(i + 2);

        colors.Add(color);
        colors.Add(color);
        colors.Add(color);

        uvs.Add(new Vector2(v0.x, v0.z + v0.y));
        uvs.Add(new Vector2(v1.x, v1.z + v1.y));
        uvs.Add(new Vector2(v2.x, v2.z + v2.y));
    }

    public void AddHexSide(Vector3 center, float outerRadius, Color color, HexDirection direction)
    {
        int i = (int)direction - 1;
        Vector3 v1 = new Vector3(center.x + Mathf.Cos((i + 0.5f) / 6f * 2 * Mathf.PI) * outerRadius,
                center.y,
                center.z + Mathf.Sin((i + 0.5f) / 6f * 2 * Mathf.PI) * outerRadius);
        Vector3 v2 = new Vector3(center.x + Mathf.Cos((i + 1.5f) / 6f * 2 * Mathf.PI) * outerRadius,
            center.y,
            center.z + Mathf.Sin((i + 1.5f) / 6f * 2 * Mathf.PI) * outerRadius);
        AddTriangle(center, v2, v1, color);
    }

    public void AddHexChannel(Vector3 center, float offset, float width, Color color, HexDirection direction)
    {
        float a = HexMetric.DirectionToAngle(direction);
        center += offset * Vector3.up;
        Vector3 forward = new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * 0.5f;
        Vector3 orthogonal = new Vector3(Mathf.Cos(a - Mathf.PI / 2), 0, Mathf.Sin(a - Mathf.PI / 2)) * 0.25f * HexMetric.hexInnerToOuterRatio * width;
        Vector3 v00 = center - orthogonal;
        Vector3 v10 = center + orthogonal;
        Vector3 v01 = center + forward - orthogonal;
        Vector3 v11 = center + forward + orthogonal;
        TriangulateRectangle(v00, v10, v01, v11, color);

        AddHex(center, width * 0.25f * HexMetric.hexInnerToOuterRatio, color);
    }

    public void AddHexWall(Vector3 center1, Vector3 center2, float lElevation, float rElevation, Color color, float height, float thickness, bool wallNearL, bool wallFarL, bool wallNearR, bool wallFarR)
    {
        float a = Mathf.Atan2(center2.z - center1.z, center2.x - center1.x);
        if (a < 0)
        {
            a += 2 * Mathf.PI;
        }
        Vector3 forward = new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a));
        Vector3 orthogonal = new Vector3(Mathf.Cos(a - Mathf.PI / 2), 0, Mathf.Sin(a - Mathf.PI / 2)) * 0.25f * HexMetric.hexInnerToOuterRatio;
        Vector3 lcenter = center1 + forward * 0.5f - orthogonal;
        Vector3 rcenter = center1 + forward * 0.5f + orthogonal;
        Vector3 upl = Vector3.up * (Mathf.Max(center1.y, center2.y, lElevation) - Mathf.Min(center1.y, center2.y, lElevation) + height);
        Vector3 upr = Vector3.up * (Mathf.Max(center1.y, center2.y, rElevation) - Mathf.Min(center1.y, center2.y, rElevation) + height);
        lcenter.y = Mathf.Min(center1.y, center2.y, lElevation);
        rcenter.y = Mathf.Min(center1.y, center2.y, rElevation);
        Vector3 v00 = lcenter - forward * thickness * 0.5f + orthogonal * thickness;
        Vector3 v10 = rcenter - forward * thickness * 0.5f - orthogonal * thickness;
        Vector3 v01 = lcenter + forward * thickness * 0.5f + orthogonal * thickness;
        Vector3 v11 = rcenter + forward * thickness * 0.5f - orthogonal * thickness;
        if (wallNearL && !wallFarL) {
            lcenter += forward * thickness * 0.5f;
            lcenter -= orthogonal*thickness;
        }
        else if (wallFarL && !wallNearL) {
            lcenter -= forward * thickness * 0.5f;
            lcenter -= orthogonal * thickness;
        }
        if (wallNearR && !wallFarR)
        {
            rcenter += forward * thickness * 0.5f;
            rcenter += orthogonal * thickness;
        }
        else if (wallFarR && !wallNearR)
        {
            rcenter -= forward * thickness * 0.5f;
            rcenter += orthogonal * thickness;
        }

        //left
        TriangulateRectangle(v01, lcenter, v01 + upl, lcenter + upl, color);
        TriangulateRectangle(lcenter, v00, lcenter + upl, v00 + upl, color);
        //right
        TriangulateRectangle(v10, rcenter, v10 + upr, rcenter + upr, color);
        TriangulateRectangle(rcenter, v11, rcenter + upr, v11 + upr, color);
        //front
        TriangulateRectangle(v00, v10, v00 + upl, v10 + upr, color);
        //back
        TriangulateRectangle(v11, v01, v11 + upr, v01 + upl, color);
        //top
        //TriangulateRectangle(v00 + upl, v10 + upr, lcenter + upl, rcenter + upr, color);
        //TriangulateRectangle(lcenter + upl, rcenter + upr, v01 + upl, v11 + upr, color);
        TriangulateRectangle(v00 + upl, v10 + upr, v01 + upl, v11 + upr, color);
        AddTriangle(v00 + upl, lcenter + upl, v01 + upl, color);
        AddTriangle(v11 + upr, rcenter + upr, v10 + upr, color);

        //TriangulateRectangle(v00, v10, v00 + up, v10 + up, color);
        //TriangulateRectangle(v11, v01, v11 + up, v01 + up, color);
        //TriangulateRectangle(v01, v00, v01 + up, v00 + up, color);
        //TriangulateRectangle(v10, v11, v10 + up, v11 + up, color);
        //TriangulateRectangle(v00 + up, v10 + up, v01 + up, v11 + up, color);
    }

    public void AddHex(Vector3 center, float outerRadius, Color color)
    {
        for (int i = 0; i < 6; i++)
        {
            AddHexSide(center, outerRadius, color, (HexDirection)i);
        }
    }

    public void AddHexConnection(Vector3 center1, Color c1, Vector3 center2, Color c2, float width = 1, float offset = 0)
    {
        //float a1, a2;
        //if (h1.x == h2.x)
        //{
        //    a1 = Mathf.PI - 1 / 12f * Mathf.PI * 2;
        //    a2 = Mathf.PI + 1 / 12f * Mathf.PI * 2;
        //}
        //else
        //{
        float a = Mathf.Atan2(center2.z - center1.z, center2.x - center1.x);
        if (a < 0)
        {
            a += 2 * Mathf.PI;
        }
        //a1 = Mathf.Atan((center2.z - center1.z) / (center2.x - center1.x)) - 1 / 12f * Mathf.PI * 2;
        //a2 = Mathf.Atan((center2.z - center1.z) / (center2.x - center1.x)) + 1 / 12f * Mathf.PI * 2;
        //if (center1.x > center2.x)
        //{
        //	a1 += Mathf.PI;
        //	a2 += Mathf.PI;
        //}
        //}
        //Debug.Log(x1 + "," + y1 + " " + x2 + "," + y2 + " : " + Mathf.Atan((h2.z - h1.z) / (h2.x - h1.x)));
        //Vector3 v1 = center1 + new Vector3(Mathf.Cos(a1), 0, Mathf.Sin(a1)) * 0.5f * HexMetric.hexOuterRatio;
        //Vector3 v2 = center1 + new Vector3(Mathf.Cos(a2), 0, Mathf.Sin(a2)) * 0.5f * HexMetric.hexOuterRatio;
        Vector3 forward = new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * 0.5f;
        Vector3 orthogonal = new Vector3(Mathf.Cos(a + Mathf.PI / 2), 0, Mathf.Sin(a + Mathf.PI / 2)) * 0.25f * HexMetric.hexInnerToOuterRatio * width;
        Vector3 v1 = center1 + forward - orthogonal;
        Vector3 v2 = center1 + forward + orthogonal;
        //bool invert = center2.y > center1.y;
        //invert = inver
        //if (gMesh.tileHeights[x2, y2] > gMesh.tileHeights[x1, y1])
        //{
        //    TriangulateRectangle(v1, v2,
        //    v1 + Vector3.up * (gMesh.tileHeights[x2, y2] - gMesh.tileHeights[x1, y1]),
        //    v2 + Vector3.up * (gMesh.tileHeights[x2, y2] - gMesh.tileHeights[x1, y1]),
        //    gMesh.tileColors[x1, y1]);
        //}
        //else
        //{
        Vector3 dir = center2.y < center1.y ? center2 - center1 : center1 - center2;
        dir.y = 0;
        dir.Normalize();
        v1 += dir * offset;
        v2 += dir * offset;

        Color c = center1.y > center2.y ? c1 : c2;
        TriangulateRectangle(
        v1 + Vector3.up * (center2.y - center1.y),
        v2 + Vector3.up * (center2.y - center1.y),
        v1,
        v2,
        c);
        //}
        //TriangulateRectangle(v1, v2,
        //    v1 + Vector3.up ,
        //    v2 + Vector3.up ,
        //    gMesh.tileColors[x1, y1]);

    }

    void TriangulateRectangle(Vector3 v00, Vector3 v10, Vector3 v01, Vector3 v11, Color col)
    {
        int i = vertices.Count;
        vertices.Add(v00);
        vertices.Add(v10);
        vertices.Add(v01);
        vertices.Add(v11);

        triangles.Add(i);
        triangles.Add(i + 2);
        triangles.Add(i + 1);

        triangles.Add(i + 1);
        triangles.Add(i + 2);
        triangles.Add(i + 3);

        colors.Add(col);
        colors.Add(col);
        colors.Add(col);
        colors.Add(col);

        uvs.Add(new Vector2(v00.x, v00.z + v00.y));
        uvs.Add(new Vector2(v01.x, v01.z + v01.y));
        uvs.Add(new Vector2(v10.x, v10.z + v10.y));
        uvs.Add(new Vector2(v11.x, v11.z + v11.y));
    }

    //public void SetTile(int x, int y, float height, Color color)
    //{
    //    gMesh.tileHeights[x, y] = height;
    //    gMesh.tileColors[x, y] = color;
    //    needUpdate = true;
    //}




    //public void Initialize(int size)
    //{
    //    //Debug.Log("Generating: " + size);
    //    this.size = size;
    //    vertices = new Vector3[(size * 2) * (size * 2)];
    //    normals = new Vector3[(size * 2) * (size * 2)];
    //    colors = new Color[(size * 2) * (size * 2)];
    //    triangles = new int[(size * 2 - 1) * (size * 2 - 1) * 6];


    //    for (int i = 0; i < size * 2 - 1; i++)
    //    {
    //        //normals[i] = Vector3.up;
    //        for (int j = 0; j < size * 2 - 1; j++)
    //        {
    //            triangles[(i + j * (size * 2 - 1)) * 6] = i + j * size * 2;
    //            triangles[(i + j * (size * 2 - 1)) * 6 + 2] = triangles[(i + j * (size * 2 - 1)) * 6 + 4] = i + (j + 1) * size * 2;
    //            triangles[(i + j * (size * 2 - 1)) * 6 + 1] = triangles[(i + j * (size * 2 - 1)) * 6 + 3] = (i + 1) + j * size * 2;
    //            triangles[(i + j * (size * 2 - 1)) * 6 + 5] = (i + 1) + (j + 1) * size * 2;
    //        }
    //    }
    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        normals[i] = Vector3.up;
    //        colors[i] = Color.blue;
    //    }
    //    for (int i = 0; i < size*2; i++)
    //    {
    //        for (int j = 0; j < size*2; j++)
    //        {
    //            vertices[i + j * size * 2] = new Vector3(i,0,j*size*2);
    //            //SetTile(i, j, i + j, Color.HSVToRGB(Random.Range(0f, 1f), 1, 1));
    //        }
    //    }
    //    mesh.vertices = vertices;
    //    mesh.normals = normals;
    //    mesh.triangles = triangles;
    //    mesh.colors = colors;
    //    //mesh.Optimize();
    //    //mesh.RecalculateNormals();
    //}



    //// Update is called once per frame
    //void Update()
    //{

    //}

    //public void SetTile(int x, int y, float height, Color color)
    //{
    //    vertices[x * 2 + y * 2 * size] = new Vector3(x * 2, height, y * 2);
    //    vertices[x * 2 + 1 + y * 2 * size] = new Vector3(x * 2 + 1, height, y * 2);
    //    vertices[x * 2 + (y * 2 + 1) * size] = new Vector3(x * 2, height, y * 2 + 1);
    //    vertices[x * 2 + 1 + (y * 2 + 1) * size] = new Vector3(x * 2 + 1, height, y * 2 + 1);
    //    colors[x * 2 + y * 2 * size] = colors[x * 2 + 1 + y * 2 * size] = colors[x * 2 + (y * 2 + 1) * size] = colors[x * 2 + 1 + (y * 2 + 1) * size] = color;
    //    /*vertices[(x + y * size) * 6] = new Vector3(x, height, y);
    //    vertices[(x + y * size) * 6 + 1] = new Vector3(x, height, y+1);
    //    vertices[(x + y * size) * 6 + 2] = new Vector3(x+1, height, y);
    //    vertices[(x + y * size) * 6 + 3] = new Vector3(x, height, y+1);
    //    vertices[(x + y * size) * 6 + 4] = new Vector3(x+1, height, y+1);
    //    vertices[(x + y * size) * 6 + 5] = new Vector3(x+1, height, y);
    //    colors[(x + y * size) * 6] = colors[(x + y * size) * 6 + 1] = colors[(x + y * size) * 6 + 2] =
    //        colors[(x + y * size) * 6 + 3] = colors[(x + y * size) * 6 + 4] = colors[(x + y * size) * 6 + 5] = color;*/
    //    mesh.vertices = vertices;
    //    mesh.colors = colors;
    //}

}
