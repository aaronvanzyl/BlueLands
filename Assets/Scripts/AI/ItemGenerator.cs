using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemGenerator
{
    public static Item GenerateType(int type) {
        return new Item(type);
    }
}
