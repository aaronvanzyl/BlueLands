using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action
{
    public enum ActionOutcome { InProgress, Complete };

    public List<Condition> conditions;
    public abstract ActionOutcome Execute(IWorldState worldState, float duration);
    public abstract void ExecuteImmediate(IWorldState worldState);
    public abstract float EstimateCost(IWorldState worldState);
    protected abstract void GenerateConditions(IWorldState worldState);
}
