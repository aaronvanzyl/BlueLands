using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractEntity : IEntity
{ 
    IEntity parentEntity;
    public int Id { get; set; }
    public float Money { get; set; }
    public Vector2Int Position { get; set; }
    List<Item> inventory;
    public List<Item> Inventory { 
        get 
        {
            if (inventory == null)
            {
                inventory = new List<Item>();
                List<Item> parentInventory = parentEntity.Inventory;
                foreach (Item i in parentInventory)
                {
                    inventory.Add(i.Clone());
                }
            }
            return inventory;
        } 
        set 
        {
            inventory = value;
        }
    }

    public AbstractEntity(IEntity parentEntity)
    {
        this.parentEntity = parentEntity;
        Position = parentEntity.Position;
        Id = parentEntity.Id;
        Money = parentEntity.Money;
    }

    public AbstractEntity Clone() {
        AbstractEntity clone = new AbstractEntity(this);
        //if (inventory != null) {
        //    List<Item> invClone = new List<Item>();
        //    foreach (Item i in inventory) {
        //        invClone.Add(i.Clone());
        //    }
        //    clone.SetInvetory(invClone);
        //}
        return clone;
    }
}
