using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class NoiseGen
{
    public Vector2 frequency;
    public int layers;
    public float lacunarity;
    public float persistence;
    public AnimationCurve remap;
    public int seed;


    public float Evaluate(Vector2 pos)
    {
        float noise = Noise.Layered2D(pos, frequency, layers, lacunarity, persistence, seed);
        noise = noise * 0.5f + 0.5f;
        return remap.Evaluate(noise);
    }

    public Vector2 Evaluate2D(Vector2 pos)
    {
        float noiseX = Noise.Layered2D(pos, frequency, layers, lacunarity, persistence, seed);
        float noiseY = Noise.Layered2D(pos, frequency, layers, lacunarity, persistence, seed+1);
        noiseX = remap.Evaluate(noiseX * 0.5f + 0.5f);
        noiseY = remap.Evaluate(noiseY * 0.5f + 0.5f);
        return new Vector2(noiseX, noiseY);
    }
}
