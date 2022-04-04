using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathFinder
{
    public static float EstimateDistance(Vector2Int pos1, Vector2Int pos2)
	{
		return (pos2-pos1).magnitude;
	}
}
