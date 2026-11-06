using Core;
using Global;
using UnityEngine;

namespace Game
{
    public class GameInventoryManager : Singleton<GameInventoryManager>
    { 
        [SerializeField] private GameInventorySlot[] _slots;
        private int _activeSlotIndex = -1;

        public void UpdateSlot(int slotId, Sprite itemSprite)
        {
            _slots[slotId].SetItem(itemSprite);
        }

        public void OnSlotActive(int slotIndex)
        {
            if(_activeSlotIndex != -1)
                _slots[_activeSlotIndex].UnhighlightSlot();

            _activeSlotIndex = slotIndex;
            _slots[_activeSlotIndex].HighlightSlot();
        }

        public void OnScrollSlot(int direction)
        {
            if (_activeSlotIndex != -1)
                _slots[_activeSlotIndex].UnhighlightSlot();

            _activeSlotIndex += direction;
            
            if (_activeSlotIndex < 0)
                _activeSlotIndex = _slots.Length - 1;
            else if (_activeSlotIndex >= _slots.Length)
                _activeSlotIndex = 0;
            
            _slots[_activeSlotIndex].HighlightSlot();
        }
    }
}