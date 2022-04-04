using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealizedWorldState : IWorldState
{
    Dictionary<int, Merchant> merchantDict = new Dictionary<int, Merchant>();
    Dictionary<int, IEntity> entityDict = new Dictionary<int, IEntity>();
    public int NextEntityId { get; set; }
    public int NextMerchantId { get; set; }

    public void AddEntity(IEntity entity)
    {
        entity.Id = NextEntityId;
        entityDict.Add(NextEntityId, entity);
        NextEntityId++;
    }

    public void AddMerchant(Merchant merchant)
    {
        merchant.id = NextMerchantId;
        merchantDict.Add(NextMerchantId, merchant);
        NextMerchantId++;
    }

    public IEntity GetEntity(int id)
    {
        return entityDict[id];
    }

    public Merchant GetMerchant(int id)
    {
        return merchantDict[id];
    }

    public ICollection<Merchant> GetMerchants()
    {
        return merchantDict.Values;
    }
}
