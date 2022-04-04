using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessCondition : Condition
{
    public int ownerId;
    public int itemType;

    public PossessCondition(int ownerId, int objectType)
    {
        this.ownerId = ownerId;
        this.itemType = objectType;
    }

    override public bool Satisfied(IWorldState worldState) {
        IEntity owner = worldState.GetEntity(ownerId);
        foreach (Item i in owner.Inventory) {
            if (i.type == itemType) {
                return true;
            }
        }
        return false;
    }

    public override string ToString()
    {
        return $"entity:{ownerId}\nitem:{itemType}";
    }
}