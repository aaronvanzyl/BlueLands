using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtility
{
    public static Vector3 RandVector3(Vector3 min, Vector3 max) {
        return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
    }
}
