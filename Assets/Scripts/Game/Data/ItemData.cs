using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item Data")]
    public class ItemData : ScriptableObject
    {
        public int ItemID;
        public string ItemName;
        public Sprite ItemSprite;
        public EquippableController VisualPrefab;
        public GameObject DropPrefab;
    }
}