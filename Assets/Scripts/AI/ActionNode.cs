using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : Node
{
    public Action action;
    public ActionNode(Action action) {
        this.action = action;
    }

    public override string ToString() {
        return action.GetType().ToString() + "\n" + action.ToString();
    }
}
