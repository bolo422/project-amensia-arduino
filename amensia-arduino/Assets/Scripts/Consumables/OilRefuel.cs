using Player;
using UnityEngine;

namespace Consumables
{
    public class OilRefuel : ConsumableBase
    {

        protected override void OnPickup()
        {
            PlayerLamp.Instance.AddOil(GameManager.Instance.OilRefuelQuanity);
        }
    }
}
