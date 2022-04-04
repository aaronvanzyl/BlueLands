using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWorldState
{
    int NextEntityId { get; set; }
    int NextMerchantId { get; set; }
    void AddEntity(IEntity entity);
    IEntity GetEntity(int id);
    void AddMerchant(Merchant merchant);
    Merchant GetMerchant(int id);
    ICollection<Merchant> GetMerchants();
}
