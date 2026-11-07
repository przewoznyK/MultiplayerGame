using Global.Enums;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Weapon")]
    public class WeaponData : ItemData
    {
        [Header("WEAPON STATS")]
        public WeaponType WeaponType;
        public int Damage;
        public float AttackSpeed;
    }
}