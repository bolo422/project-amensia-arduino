using Player;
using UnityEngine;

namespace Consumables
{
    public class Key : ConsumableBase
    {

        
        protected override void OnPickup()
        {
            GameManager.Instance.AddKey();
        }
    }
}
