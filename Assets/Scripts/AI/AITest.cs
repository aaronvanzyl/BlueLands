using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITest : MonoBehaviour
{
    public NodeTreeRenderer treeRenderer;
    

    private void Start()
    {
        RealizedWorldState worldState = new RealizedWorldState();

        RealizedEntity entity = new RealizedEntity();
        entity.Money = 1000; 
        worldState.AddEntity(entity);

        Merchant merchant = new Merchant(new Vector2Int(20,30));
        merchant.saleEntries.Add(0, 23);
        worldState.AddMerchant(merchant);

        Condition possessCond = new PossessCondition(entity.Id, 0);
        ConditionNode condNode = new ConditionNode(possessCond);
         
        PlanGenerator.FillTree(condNode, worldState, 5, 5);

        treeRenderer.RenderTree(condNode);
    }
}
    