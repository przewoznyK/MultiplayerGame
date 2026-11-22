using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "New Consumable", menuName = "Items/Consumable")]
    public class ConsumableData : ItemData
    {
        [Header("CONSUMABLE STATS")]
        public int HealAmount;
        public float ConsumeTime;
    }
}