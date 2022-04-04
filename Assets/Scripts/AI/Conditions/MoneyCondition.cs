using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyCondition : Condition
{
    int entityID;
    float money;

    public MoneyCondition(int entityID, float money) 
    {
        this.entityID = entityID;
        this.money = money;
    }

    public override bool Satisfied(IWorldState worldState)
    {
        IEntity entity = worldState.GetEntity(entityID);
        return entity.Money > money;
    }

    public override string ToString()
    {
        return $"entity:{entityID}\nmoney:{money}";
    }
}
