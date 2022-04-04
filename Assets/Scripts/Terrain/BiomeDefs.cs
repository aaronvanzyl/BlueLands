using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeDefs : MonoBehaviour
{
    public Biome tundra;
    public Biome borealForest;
    public Biome temperateForest;
    public Biome rainForest;
    public Biome swamp;
    public Biome savanna;
    public Biome steppes;
    public Biome desert;

    Biome[] biomeChart;
    public float[] humidityRanges = { 0.25f, 0.5f, 0.75f };
	public float[] temperatureRanges = { 0.25f, 0.5f, 0.75f };

    private void Awake()
    {
        biomeChart = new Biome[] {
                tundra, steppes, desert, desert,
            borealForest, borealForest, temperateForest, savanna,
            borealForest, temperateForest, temperateForest, savanna,
            swamp, rainForest, rainForest, rainForest
        };
    }

    public Biome DetermineBiome(Tile tile)
    {
        int tempIndex = 0;
        while (tempIndex < temperatureRanges.Length && tile.temperature > temperatureRanges[tempIndex])
        {
            tempIndex++;
        }

        int humidityIndex = 0;
        while (humidityIndex < humidityRanges.Length && tile.humidity > humidityRanges[humidityIndex])
        {
            humidityIndex++;
        }
        return biomeChart[humidityIndex * (temperatureRanges.Length + 1) + tempIndex];
    }
}
