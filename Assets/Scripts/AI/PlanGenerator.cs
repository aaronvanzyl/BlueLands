using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlanGenerator
{
    public static void FillTree (ConditionNode conditionNode, IWorldState worldState, int depth, int width) 
    { 
        Stack<ConditionNode> stack = new Stack<ConditionNode>();
        stack.Push(conditionNode);
        FillTree(conditionNode, stack, worldState, depth, width);
    }

    public static void FillTree(Node outNode, Stack<ConditionNode> unsatisfied, IWorldState worldState, int depth, int width)
    {
        Debug.Log($"Filling Tree: {outNode}, depth: {depth}, unsatisfied: {unsatisfied.Count}");
        if (depth == 0)
        {
            return;
        }
        ConditionNode nextCond;
        do
        {
            if (unsatisfied.Count == 0)
            {
                return;
            }
            nextCond = unsatisfied.Pop();
            outNode.inNodes.Add(nextCond);
            nextCond.outNode = outNode;
            outNode = nextCond;
        }
        // Needs to full calc
        while (nextCond.condition.Satisfied(worldState));

        Debug.Log($"Condition of type {nextCond.condition.GetType()}");

        List<Action> actions = ActionGenerator.Satisfy(nextCond.condition, worldState, width);
        Debug.Log($"Evaluating {actions.Count} possible actions");
        foreach (Action a in actions)
        {
            Debug.Log($"Action of type {a.GetType()} with {a.conditions.Count} conditions");
            ActionNode aNode = new ActionNode(a);
            aNode.outNode = outNode;
            outNode.inNodes.Add(aNode);

            if (a.conditions!=null && a.conditions.Count > 0)
            {
                Stack<ConditionNode> newConds = new Stack<ConditionNode>(unsatisfied);
                ConditionNode[] tempNodes = new ConditionNode[a.conditions.Count];
                for (int i = 0; i < a.conditions.Count; i++)
                {
                    tempNodes[i] = new ConditionNode(a.conditions[i]);
                }
                for (int i = 0; i < a.conditions.Count; i++)
                {
                    newConds.Push(tempNodes[i]);
                    if (i == a.conditions.Count - 1)
                    {
                        tempNodes[i].outNode = aNode;
                        aNode.inNodes.Add(tempNodes[i]);
                    }
                    else
                    {
                        tempNodes[i].outNode = tempNodes[i + 1];
                        tempNodes[i + 1].inNodes.Add(tempNodes[i]);
                    }
                }
                FillTree(tempNodes[0], newConds, worldState, depth - 1, width);
            }
            else if (unsatisfied.Count > 0)
            {
                FillTree(aNode, new Stack<ConditionNode>(unsatisfied), worldState, depth - 1, width);
            }
        }



    }

    //List<ActionChain> SatisfyCondition(Condition condition, AbstractWorldState worldState, int depth)
    //{
    //    if (depth == 0)
    //    {
    //        return null;
    //    }
    //    List<Action> actions = actionGenerator.Satisfy(condition, worldState);
    //    //List<List<Tuple<List<Action>, AbstractWorldState>>> actionChains = new List<List<Tuple<List<Action>, AbstractWorldState>>>();
    //    foreach (Action action in actions)
    //    {
    //        List<ActionChain> actionChains = new List<ActionChain>();
    //        ActionChain start = new ActionChain();
    //        start.endWorldState = worldState;

    //        foreach (Condition cond in action.conditions)
    //        {
    //            foreach (ActionChain chain in actionChains)
    //            {
    //                if (!cond.Satisfied(chain.endWorldState))
    //                {
    //                    List<ActionChain> options = SatisfyCondition(cond, chain.endWorldState, depth - 1);
    //                    if (options != null && options.Count > 0)
    //                    {
    //                        foreach (ActionChain option in options) {

    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    return null;
    //}



    //public void GenerateTree(List<NodeAction> tree, AbstractWorldState worldState, Condition condition, int depth)
    //{
    //    List<Action> actions = actionGenerator.Satisfy(condition, worldState);

    //}
}
