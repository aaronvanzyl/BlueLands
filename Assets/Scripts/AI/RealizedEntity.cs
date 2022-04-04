using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealizedEntity : IEntity
{
    public RealizedEntity() {
        Inventory = new List<Item>();
    }

    public RealizedEntity(Vector2Int position) {
        this.Position = position;
        Inventory = new List<Item>();
    }

    public Vector2Int Position { get; set; }
    public List<Item> Inventory { get; set; }
    public int Id { get; set; }
    public float Money { get; set; }
}
