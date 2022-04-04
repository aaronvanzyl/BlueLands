using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NodeRenderer : MonoBehaviour
{
    public Text label;
    public Image bgImage;
    public Color actionColor;
    public Color conditionColor;
    public Node node;

    public void SetNode(Node node) {
        this.node = node;
        label.text = node.ToString();
        if (node.GetType() == typeof(ConditionNode))
        {
            bgImage.color = conditionColor;
        }
        else { 
            bgImage.color = actionColor;
        }
    }
}
