using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : Action
{
    int entityID;
    Vector2Int targetPos;

    public MoveAction(IWorldState worldState, int entityID, Vector2Int targetPos) {
        this.entityID = entityID;
        this.targetPos = targetPos;
        GenerateConditions(worldState);
    }

    public override float EstimateCost(IWorldState worldState)
    {
        IEntity entity = worldState.GetEntity(entityID);
        float dist = PathFinder.EstimateDistance(entity.Position, targetPos);
        return dist;
    }

    public override ActionOutcome Execute(IWorldState worldState, float duration)
    {
        ExecuteImmediate(worldState);
        return ActionOutcome.Complete;
    }

    public override void ExecuteImmediate(IWorldState worldState)
    {
        IEntity entity = worldState.GetEntity(entityID);
        entity.Position = targetPos;
    }

    protected override void GenerateConditions(IWorldState worldState)
    {
        conditions = new List<Condition>();
    }

    public override string ToString()
    {
        return $"entity:{entityID}\npos:{targetPos}";
    }
}
