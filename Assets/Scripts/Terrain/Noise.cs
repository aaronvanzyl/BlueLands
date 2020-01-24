using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    private static int[] hash = {
        151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
        140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
        247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
         57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
         74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
         60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
         65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
        200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
         52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
        207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
        119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
        129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
        218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
         81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
        184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
        222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180,

        151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
        140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
        247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
         57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
         74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
         60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
         65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
        200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
         52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
        207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
        119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
        129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
        218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
         81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
        184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
        222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180
    };
    const int hashMask = 255;

    static Vector2[] gradients2D = {
        RadianToVector2(0/16f*2*Mathf.PI),
        RadianToVector2(1/16f*2*Mathf.PI),
        RadianToVector2(2/16f*2*Mathf.PI),
        RadianToVector2(3/16f*2*Mathf.PI),
        RadianToVector2(4/16f*2*Mathf.PI),
        RadianToVector2(5/16f*2*Mathf.PI),
        RadianToVector2(6/16f*2*Mathf.PI),
        RadianToVector2(7/16f*2*Mathf.PI),
        RadianToVector2(8/16f*2*Mathf.PI),
        RadianToVector2(9/16f*2*Mathf.PI),
        RadianToVector2(10/16f*2*Mathf.PI),
        RadianToVector2(11/16f*2*Mathf.PI),
        RadianToVector2(12/16f*2*Mathf.PI),
        RadianToVector2(13/16f*2*Mathf.PI),
        RadianToVector2(14/16f*2*Mathf.PI),
        RadianToVector2(15/16f*2*Mathf.PI),

    };
    const int gradientMask = 15;
    static Vector2 offset = new Vector2(17f, 120f);

    static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static void SetSeed(int seed)
    {
        offset.x = seed * 54684;
        offset.x %= hashMask;
        offset.y = seed * 31781;
        offset.y %= hashMask;
    }

    public static float Value2D(Vector2 point, float frequency)
    {
        point += offset;
        point *= frequency;
        int ix0 = Mathf.FloorToInt(point.x);
        int iy0 = Mathf.FloorToInt(point.y);
        float tx = Smooth(point.x - ix0);
        float ty = Smooth(point.y - iy0);
        ix0 &= hashMask;
        iy0 &= hashMask;
        int ix1 = ix0 + 1;
        int iy1 = iy0 + 1;

        int h00 = hash[hash[ix0] + iy0];
        int h01 = hash[hash[ix0] + iy1];
        int h10 = hash[hash[ix1] + iy0];
        int h11 = hash[hash[ix1] + iy1];

        float hx0 = Mathf.Lerp(h00, h10, tx);
        float hx1 = Mathf.Lerp(h01, h11, tx);
        float hxy = Mathf.Lerp(hx0, hx1, ty);

        return hxy / hashMask;
    }

    public static float Layered2D(Vector2 point, Vector2 frequency, int layers, float lacunarity, float persistence)
    {
        float range = 1f;
        float amplitude = 1f;
        float sum = Perlin2D(point, frequency);
        for (int i = 1; i < layers; i++)
        {
            frequency *= lacunarity;
            amplitude *= persistence;
            range += amplitude;
            sum += Perlin2D(point, frequency) * amplitude;
        }
        return sum / range;
    }

    public static float Perlin2D(Vector2 point, Vector2 frequency)
    {
        point *= frequency;
        point += offset;
        int ix0 = Mathf.FloorToInt(point.x);
        int iy0 = Mathf.FloorToInt(point.y);
        float tx = point.x - ix0;
        float ty = point.y - iy0;
        ix0 &= hashMask;
        iy0 &= hashMask;
        int ix1 = ix0 + 1;
        int iy1 = iy0 + 1;

        Vector2 g00 = gradients2D[hash[hash[ix0] + iy0] & gradientMask];
        Vector2 g01 = gradients2D[hash[hash[ix0] + iy1] & gradientMask];
        Vector2 g10 = gradients2D[hash[hash[ix1] + iy0] & gradientMask];
        Vector2 g11 = gradients2D[hash[hash[ix1] + iy1] & gradientMask];

        float v00 = Dot(g00, tx, ty);
        float v01 = Dot(g01, tx, ty - 1);
        float v10 = Dot(g10, tx - 1, ty);
        float v11 = Dot(g11, tx - 1, ty - 1);

        tx = Smooth(tx);
        ty = Smooth(ty);
        return Mathf.Lerp(
            Mathf.Lerp(v00, v10, tx),
            Mathf.Lerp(v01, v11, tx), ty) * 1.41f;
    }

    static float Dot(Vector2 v, float x, float y)
    {
        return v.x * x + v.y * y;
    }

    static float Smooth(float t)
    {
        return 6 * Mathf.Pow(t, 5) - 15 * Mathf.Pow(t, 4) + 10 * Mathf.Pow(t, 3);
    }

    //static Vector2[,] gradients;

    //public static float GetPerlin(Vector2 pos)
    //{
    //    //int i = (int)(pos.x * resolution) % vectors.GetLength(0);
    //    //int j = (int)(pos.y * resolution) % vectors.GetLength(1);
    //    ////Debug.Log(i + " " + j);
    //    ////float xLerp = pos.x - vectors[i, j].x;
    //    ////float yLerp = pos.y - vectors[i, j].y;
    //    //float xLerp = pos.x - i;
    //    //float yLerp = pos.y - j;
    //    //Vector2 vec2 = new Vector2(xLerp,yLerp);
    //    //Debug.Log(vec2);
    //    //float v00 = Cross(vectors[i, j], vec2 - new Vector2(0, 0));
    //    //float v01 = Cross(vectors[i, j + 1], vec2 - new Vector2(0, 1));
    //    //float v10 = Cross(vectors[i + 1, j], vec2 - new Vector2(1, 0));
    //    //float v11 = Cross(vectors[i + 1, j + 1], vec2 - new Vector2(1, 1));
    //    ///*float x = v00 + (v10 - v00) * xLerp;
    //    //float y = v01 + (v01 - v00) * xLerp;*/
    //    //float result = v00 * (1 - xLerp) * (1 - yLerp) +
    //    //    v10 * (xLerp) * (1 - yLerp) +
    //    //    v01 * (1 - xLerp) * (yLerp) +
    //    //    v11 * (xLerp) * (yLerp);
    //    //return result;
    //    ////return Random.Range(0f, 1f);
    //    int gx = (int)(pos.x * resolution) % gradients.GetLength(0);
    //    int gy = (int)(pos.y * resolution) % gradients.GetLength(1);
    //    float u = pos.x - gx;
    //    float v = pos.y - gy;

    //    float g1 = Cross(gradients[gx, gy], pos);
    //    float g2 = Cross(gradients[gx + 1, gy], pos);
    //    float g3 = Cross(gradients[gx, gy], pos);
    //    float g4 = Cross(gradients[gx, gy], pos);

    //    x1 = Vector2.Lerp(gradients[gx, gy], gradients[gx + 1, gy], u);
    //}

    //static float Cross(Vector2 vec, Vector2 relativePos)
    //{
    //    //Vector2 relativePos = pos - vec;
    //    float x = vec.x * relativePos.x;
    //    float y = vec.y * relativePos.y;
    //    //return new Vector2(x, y);
    //    return x + y;
    //}

    //public static void GenerateVectors(int x, int y)
    //{

    //    gradients = new Vector2[x, y];
    //    for (int i = 0; i < x; i++)
    //    {
    //        for (int j = 0; j < y; j++)
    //        {
    //            float a = Random.Range(0, Mathf.PI * 2);
    //            gradients[i, j] = new Vector2(Mathf.Cos(a), Mathf.Sin(a));
    //            Debug.DrawRay(new Vector3(i, j, 0), gradients[i, j] * 0.5f, Color.red, 100000);
    //        }
    //    }
    //}

}
