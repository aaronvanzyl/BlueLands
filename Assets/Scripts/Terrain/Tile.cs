using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//public enum Biome { Tundra, Steppes, Desert, TemperateForest, Savanna, BorealForest, RainForest, Swamp};
public enum WaterType { None, River, Lake, Ocean, DeepOcean}
public enum ConnectionType { None, River};
public class Tile
{
    public Vector2Int pos;
    public int seed;
    public ConnectionType[] connections;
    public Biome biome;
    public float elevation;
    public float waterLevel;
    public WaterType waterType;
    public float humidity;
    public float temperature;
    public int trees;
    public City city;
    public float cityAppeal;

    public Tile(Vector2Int pos) {
        this.pos = pos;
        connections = new ConnectionType[6];
    }
}