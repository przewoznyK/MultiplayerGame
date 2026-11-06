using FishNet.Object;
using Game.Data;
using UnityEngine;

namespace Game
{
    public class Chest : NetworkBehaviour, IInteractable
    {
        [SerializeField] private ItemData[] _itemsToDrop;

        public string GetInteractionPrompt()
        {
            return $"Press [E] to open the chest";
        }

        public void OnServerInteract(GameObject interactor)
        {
            SpawnRandomItem();
            Despawn();
        }

        private void SpawnRandomItem()
        {
            ItemData randomItemData = _itemsToDrop[Random.Range(0, _itemsToDrop.Length)];
            Vector3 dropPosition = transform.position;
            GameObject droppedItem = Instantiate(randomItemData.DropPrefab, dropPosition, Quaternion.identity);
            Spawn(droppedItem);
        }
    }
}