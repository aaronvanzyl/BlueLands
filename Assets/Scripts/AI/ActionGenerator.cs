using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ActionGenerator
{

    public static List<Action> Satisfy(Condition condition, IWorldState worldState, int maxActions) {
        switch (condition) {
            case PossessCondition possess:
                return Satisfy(possess, worldState, maxActions);
            case LocationCondition location:
                return Satisfy(location, worldState, maxActions);
            default:
                Debug.LogWarning($"Action generator received unknown condition: {condition}");
                return new List<Action>();
        }
    }

    public static List<Action> Satisfy(PossessCondition condition, IWorldState worldState, int maxActions) {
        Debug.Log("Satisfying possessCond");
        List<Tuple<Merchant,float>> merchantDists = new List<Tuple<Merchant, float>>();
        IEntity owner = worldState.GetEntity(condition.ownerId);
        foreach (Merchant m in worldState.GetMerchants()) {
            if (m.SellsType(condition.itemType)) {
                float dist = PathFinder.EstimateDistance(owner.Position, m.position);
                merchantDists.Add(new Tuple<Merchant, float>(m,dist));
            }
        }
        Debug.Log($"Found {merchantDists.Count} merchants");

        merchantDists.Sort((x, y) => x.Item2.CompareTo(y.Item2));
        List<Action> actions = new List<Action>();
        for (int i = 0; i < Mathf.Min(maxActions,merchantDists.Count); i++) {
            BuyAction buyAction = new BuyAction(worldState, condition.ownerId, condition.itemType, merchantDists[i].Item1.id);
            actions.Add(buyAction);
        }
        return actions;
    }

    public static List<Action> Satisfy(LocationCondition condition, IWorldState worldState, int maxActions)
    {
        List<Action> actions = new List<Action>(); 
        MoveAction moveAction = new MoveAction(worldState, condition.entityID, condition.position);
        actions.Add(moveAction);
        return actions;
    }
}
