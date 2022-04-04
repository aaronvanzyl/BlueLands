using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Condition
{
    public abstract bool Satisfied(IWorldState worldState);

    public bool CanMerge(Condition other) {
        return false;
    }

    public bool Conflicts(Condition other)
    {
        return false;
    }
}
