using System;
using Player;
using UnityEngine;

namespace Consumables
{
    public class Key : ConsumableBase
    {
        private int myLevel;

        private void Awake()
        {
            myLevel = GetComponentInParent<KeySpawner>().Level;
        }

        private void Start()
        {
            GetComponent<SpriteRenderer>().color = GameManager.Instance.GetLevelColor(myLevel);
        }

        protected override void OnPickup()
        {
            GameManager.Instance.AddKey(myLevel);
        }
    }
}
