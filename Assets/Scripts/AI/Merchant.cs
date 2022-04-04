using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant
{
    public int id;
    public Vector2Int position;
    public Dictionary<int, float> saleEntries;

    public Merchant(Vector2Int position)
    {
        this.position = position;
        saleEntries = new Dictionary<int, float>();
    }

    public Merchant(Vector2Int position, Dictionary<int, float> saleEntries) {
        this.position = position;
        this.saleEntries = saleEntries;
    }

    public bool SellsType(int type)
    {
        return saleEntries.ContainsKey(type);
    }

    public float SalePrice(int itemType) {
        if (saleEntries.ContainsKey(itemType)) {
            return saleEntries[itemType];
        }
        return -1;
    }

    public Merchant Clone() {
        Merchant clone = new Merchant(position, new Dictionary<int, float>(saleEntries));
        clone.id = id;
        return clone;
    }
}
