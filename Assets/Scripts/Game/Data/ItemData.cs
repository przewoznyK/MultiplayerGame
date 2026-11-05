using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item Data")]
    public class ItemData : ScriptableObject
    {
        public int ItemID;
        public string ItemName;
        public Sprite ItemSprite;
        public WeaponController WeaponVisualPrefab;
        public GameObject DropPrefab;
        public int Damage;
    }
}