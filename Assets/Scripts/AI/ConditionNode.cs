using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionNode : Node
{
    public Condition condition;
    public ConditionNode(Condition condition) {
        this.condition = condition;
    }

    public override string ToString()
    {
        return condition.GetType().ToString() + "\n" + condition.ToString();
    }
}
