using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Chest : NetworkBehaviour, IInteractable
    {
        [SerializeField] private List<ItemData> _possibleItemIds = new();

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
            ItemData randomItemData = _possibleItemIds[Random.Range(0, _possibleItemIds.Count)];
            Vector3 dropPosition = transform.position;
            GameObject droppedItem = Instantiate(randomItemData.DropPrefab, dropPosition, Quaternion.identity);
            Spawn(droppedItem);
        }
    }
}