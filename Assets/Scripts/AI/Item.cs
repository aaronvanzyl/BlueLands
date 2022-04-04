using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public int type;

    public Item(int type) {
        this.type = type;
    }

    public Item Clone() {
        Item i = new Item(type);
        return i;
    }
}
