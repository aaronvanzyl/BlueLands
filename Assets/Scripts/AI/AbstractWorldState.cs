using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractWorldState : IWorldState
{
    public IWorldState parentWorldState;
    bool clonedParentMerchants = false;
    Dictionary<int, Merchant> merchantDict = new Dictionary<int, Merchant>();
    Dictionary<int, IEntity> entityDict = new Dictionary<int, IEntity>();
    public int NextEntityId { get; set; }
    public int NextMerchantId { get; set; }

    public AbstractWorldState(IWorldState parentWorldState) {
        this.parentWorldState = parentWorldState;
        NextEntityId = parentWorldState.NextEntityId;
        NextMerchantId = parentWorldState.NextMerchantId;
    }

    public IEntity GetEntity(int id)
    {
        if (entityDict.TryGetValue(id, out IEntity entity))
        {
            return entity;
        }
        else
        {
            IEntity parentEntity = parentWorldState.GetEntity(id);
            if (parentEntity != null)
            {
                AbstractEntity e = new AbstractEntity(parentEntity);
                entityDict.Add(parentEntity.Id, e);
                return e;
            }
            else
            {
                return null;
            }
        }
    }

    public ICollection<Merchant> GetMerchants() {
        if (!clonedParentMerchants) {
            CloneParentMerchants();
            clonedParentMerchants = true;
        }
        return merchantDict.Values;
    }

    void CloneParentMerchants() {
        merchantDict = new Dictionary<int, Merchant>();
        ICollection<Merchant> parentMerchants = parentWorldState.GetMerchants();
        foreach (Merchant m in parentMerchants)
        {
            if (!merchantDict.ContainsKey(m.id))
            {
                merchantDict.Add(m.id, m.Clone());
            }
        }
    }

    public Merchant GetMerchant(int id) {
        if (merchantDict.TryGetValue(id, out Merchant merchant))
        {
            return merchant;
        }
        else
        {
            Merchant parentMerchant = parentWorldState.GetMerchant(id);
            if (parentMerchant != null)
            {
                Merchant m = parentMerchant.Clone();
                merchantDict.Add(m.id, m);
                return m;
            }
            else
            {
                return null;
            }
        }
    }

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


    //public AbstractWorldState Clone() {
    //    AbstractWorldState clone = new AbstractWorldState(parentWorldState);

    //    foreach (KeyValuePair<int, AbstractEntity> pair in entityDict) {
    //        clone.entityDict.Add(pair.Key, pair.Value.Clone());
    //    }
    //    return clone;
    //}
}
