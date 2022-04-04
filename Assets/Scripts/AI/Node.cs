using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int width;
    public List<Node> inNodes = new List<Node>();
    public Node outNode;

    public void CalculateWidth()
    {
        if (inNodes.Count == 0)
        {
            width = 1;
        }
        else {
            width = 0;
            foreach (Node n in inNodes) {
                n.CalculateWidth();
                width += n.width;
            }
        }
    }
}
