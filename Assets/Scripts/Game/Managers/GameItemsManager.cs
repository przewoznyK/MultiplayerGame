using Core;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GameItemsManager : Singleton<GameItemsManager>
    {
        [SerializeField] private ItemData[] _allItemAssets;
        private readonly Dictionary<int, ItemData> _itemDictionary = new Dictionary<int, ItemData>();

        protected override void Awake()
        {
            base.Awake();
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            foreach (var item in _allItemAssets)
            {
                if (_itemDictionary.ContainsKey(item.ItemID) == false)
                    _itemDictionary.Add(item.ItemID, item);
                else
                    Debug.LogError($"ItemDatabase: Duplicate Item ID {item.ItemID} found on {item.ItemName}!");
            }
        }

        public ItemData GetItemById(int id)
        {
            if (_itemDictionary.TryGetValue(id, out ItemData itemData))
                return itemData;

            Debug.LogError($"ItemDatabase: Item with ID {id} does not exist!");
            return null;
        }
    }
}