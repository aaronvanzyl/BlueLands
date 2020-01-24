using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Biome { Tundra, Steppes, Desert, TemperateForest, Savanna, BorealForest, RainForest, Swamp};
public enum WaterType { None, River, Lake, Ocean}
public class Tile
{
    public Biome biome;
    public float elevation;
    public float waterLevel;
    public WaterType waterType;
    public float humidity;
    public float temperature;
}