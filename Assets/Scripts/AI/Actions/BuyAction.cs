using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyAction : Action
{
    public int buyerId;
    public int itemType;
    public int merchantId;

    public BuyAction(IWorldState worldState, int buyerId, int itemType, int merchantId) {
        this.buyerId = buyerId;
        this.itemType = itemType;
        this.merchantId = merchantId;
        GenerateConditions(worldState);
    }

    public override float EstimateCost(IWorldState worldState)
    {
        Merchant merchant = worldState.GetMerchant(merchantId);
        float salePrice = merchant.SalePrice(itemType);
        return salePrice;
    }

    public override ActionOutcome Execute(IWorldState worldState, float duration)
    {
        ExecuteImmediate(worldState);
        return ActionOutcome.Complete;
    }

    public override void ExecuteImmediate(IWorldState worldState) 
    {
        IEntity buyer = worldState.GetEntity(buyerId);
        Merchant merchant = worldState.GetMerchant(merchantId);
        float salePrice = merchant.SalePrice(itemType);
        if (buyer.Money > salePrice)
        {
            Item item = ItemGenerator.GenerateType(itemType);
            buyer.Inventory.Add(item);
            buyer.Money = buyer.Money - salePrice;
        }
    }

    protected override void GenerateConditions(IWorldState worldState)
    {
        Merchant merchant = worldState.GetMerchant(merchantId);
        conditions = new List<Condition>();
        LocationCondition locationCond = new LocationCondition(buyerId, merchant.position);
        conditions.Add(locationCond);
        MoneyCondition moneyCond = new MoneyCondition(buyerId, merchant.SalePrice(itemType));
        conditions.Add(moneyCond);
    }

    public override string ToString()
    {
        return $"buyer:{buyerId}\nitem:{itemType}\nmerchant:{merchantId}";
    }
}
