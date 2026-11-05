using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GameInventorySlot : MonoBehaviour
    {
        [SerializeField] private Image _borderImage;
        [SerializeField] private Image _itemImage;
        [SerializeField] private Button _button;
        private int _slotIndex;
        
        private void Awake()
        {
            _slotIndex = transform.GetSiblingIndex();
            _button.onClick.AddListener(OnSlotClicked);
        }

        private void OnSlotClicked()
        {
            GameInventoryManager.Instance.OnSlotActive(_slotIndex);
        }

        public void HighlightSlot()
        {
            _borderImage.color = Color.yellow;
        }

        public void UnhighlightSlot()
        {
            _borderImage.color = Color.grey;
        }

        public void SetItem(Sprite itemSprite)
        {
            _itemImage.sprite = itemSprite;
        }
    }
}