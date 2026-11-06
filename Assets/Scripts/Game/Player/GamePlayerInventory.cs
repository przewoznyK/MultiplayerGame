using FishNet.Object;
using FishNet.Object.Synchronizing;
using Game.Data;
using UnityEngine;

namespace Game
{
    public class GamePlayerInventory : NetworkBehaviour
    {
        [SerializeField] private GamePlayer _gamePlayer;
        public readonly SyncList<int> Items = new SyncList<int>();
        public readonly SyncVar<int> EquippedItemID = new SyncVar<int>(-1);
        public readonly SyncVar<int> ActiveSlotIndex = new SyncVar<int>(-1);

        public void OnScrollSlot(int direction)
        {
            if (IsOwner == false)
                return;

            int currentSlot = ActiveSlotIndex.Value;

            if (currentSlot == -1)
                currentSlot = 0;

            int newSlotIndex = currentSlot + direction;

            if (newSlotIndex < 0)
                newSlotIndex = Items.Count - 1;
            else if (newSlotIndex >= Items.Count)
                newSlotIndex = 0;

            RequestChangeSlot(newSlotIndex);
        }

        public void RequestChangeSlot(int newSlotIndex)
        {
            if (IsOwner && newSlotIndex != ActiveSlotIndex.Value)
                ServerChangeSlot(newSlotIndex);
        }

        public void RequestDropEquippedItem()
        {
            if (IsOwner)
                ServerDropEquippedItem();
        }

        [ServerRpc]
        private void ServerDropEquippedItem()
        {
            if (Items[ActiveSlotIndex.Value] <= 0)
                return;
            
            DropActiveItem();
        }

        [ServerRpc]
        private void ServerChangeSlot(int newSlotIndex)
        {
            ActiveSlotIndex.Value = newSlotIndex;

            if (newSlotIndex >= 0 && newSlotIndex < Items.Count)
                EquippedItemID.Value = Items[newSlotIndex];
            else
                EquippedItemID.Value = -1;
        }

        private void OnActiveSlotChanged(int oldIndex, int newIndex, bool asServer)
        {
            if (asServer) 
                return;

            if (IsOwner)
                GameInventoryManager.Instance.OnSlotActive(newIndex);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            EquippedItemID.OnChange += OnEquippedItemChanged;

            if (base.IsOwner)
            {
                Items.OnChange += OnInventoryChanged;
                ActiveSlotIndex.OnChange += OnActiveSlotChanged;
            }

            RequestChangeSlot(0);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            Items.Add(0);

            for (int i = 1; i < 6; i++)
                Items.Add(-1);
            
            AddItem(1);
            AddItem(3);
        }
        
        public override void OnStopClient()
        {
            base.OnStopClient();
            
            EquippedItemID.OnChange -= OnEquippedItemChanged;
            if (base.IsOwner)
            {
                Items.OnChange -= OnInventoryChanged;
                ActiveSlotIndex.OnChange -= OnActiveSlotChanged;
            }
        }
        
        private void OnEquippedItemChanged(int oldItemID, int newItemID, bool asServer)
        {
            if (asServer) 
                return;
            
            ItemData itemData = GameItemsManager.Instance.GetItemById(newItemID);
            _gamePlayer.UpdateEquippable(itemData);
        }

        [Server]
        public void AddItem(int itemId)
        {
            for (int i = 1; i < Items.Count; i++)
            {
                if (Items[i] == -1)
                {
                    Items[i] = itemId;
                    UpdateServerEquippedState();

                    return;
                }
            }
        }

        [Server]
        public void DropActiveItem()
        {
            int currentSlot = ActiveSlotIndex.Value;

            if (currentSlot < 0 || currentSlot >= Items.Count)
                return;

            int itemId = Items[currentSlot];
            Items[currentSlot] = -1;

            ItemData itemData = GameItemsManager.Instance.GetItemById(itemId);

            Vector3 dropPosition = transform.position;
            GameObject droppedItem = Instantiate(itemData.DropPrefab, dropPosition, Quaternion.identity);

            if (droppedItem.TryGetComponent(out ItemPickup pickupScript))
                pickupScript.ItemData.ItemID = itemId;

            Spawn(droppedItem);
            UpdateServerEquippedState();
        }

        [Server]
        private void UpdateServerEquippedState()
        {
            if (ActiveSlotIndex.Value >= 0 && ActiveSlotIndex.Value < Items.Count)
                EquippedItemID.Value = Items[ActiveSlotIndex.Value];
            else
                EquippedItemID.Value = -1;

            ItemData itemData = GameItemsManager.Instance.GetItemById(EquippedItemID.Value);
            _gamePlayer.UpdateEquippable(itemData);
        }

        [Server]
        public bool TryAddItem(int itemId)
        {
            for (int i = 1; i < Items.Count; i++)
            {
                if (Items[i] == -1)
                {
                    Items[i] = itemId;
                    UpdateServerEquippedState();
                    return true;
                }
            }

            return false;
        }

        [Server]
        public void ConsumeActiveItem()
        {
            int currentSlot = ActiveSlotIndex.Value;

            if (currentSlot >= 0 && currentSlot < Items.Count)
            {
                Items[currentSlot] = -1;
                UpdateServerEquippedState();
            }
        }

        private void OnInventoryChanged(SyncListOperation op, int index, int oldItem, int newItem, bool asServer)
        {
            if (asServer) 
                return;

            ItemData itemData = GameItemsManager.Instance.GetItemById(newItem);
            switch (op)
            {
                case SyncListOperation.Add:
                    GameInventoryManager.Instance.UpdateSlot(index, itemData.ItemSprite);
                    break;

                case SyncListOperation.Set:
                    GameInventoryManager.Instance.UpdateSlot(index, itemData.ItemSprite);
                    break;
            }
        }
    }
}