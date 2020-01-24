using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public  class NoiseGen
{
    public Vector2 frequency;
    public int layers;
    public float lacunarity;
    public float persistence;
    public AnimationCurve remap;

    public float Evaluate(Vector2 pos)
    {
        float noise = Noise.Layered2D(pos, frequency, layers, lacunarity, persistence);
        noise = noise * 0.5f + 0.5f;
        return remap.Evaluate(noise);
    }
}
