using FishNet.Object;
using UnityEngine;

namespace Game
{
    public class ItemPickup : NetworkBehaviour, IInteractable
    {
        public ItemData ItemData;

        public string GetInteractionPrompt()
        {
            return $"Press [E] to pick up {ItemData.ItemName}";
        }

        public void OnServerInteract(GameObject interactor)
        {
            if (interactor.TryGetComponent(out GamePlayerInventory inventory))
            {
                bool success = inventory.TryAddItem(ItemData.ItemID);

                if (success)
                    ServerManager.Despawn(gameObject);
            }
        }
    }
}