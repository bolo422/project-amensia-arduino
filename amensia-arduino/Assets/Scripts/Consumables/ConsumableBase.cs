using UnityEngine;

namespace Consumables
{
    public abstract class ConsumableBase : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.CompareTag("Player"))
            {
                OnPickup();
                Destroy(gameObject);
            }
        }

        protected abstract void OnPickup();
    }
}
