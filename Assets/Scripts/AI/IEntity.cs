using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity
{
    Vector2Int Position { get; set; }
    List<Item> Inventory { get; set; }
    int Id { get; set; }
    float Money { get; set; }
}