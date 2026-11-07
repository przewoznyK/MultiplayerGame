using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "New Range Weapon Data", menuName = "Items/RangeWeapon")]
    public class RangeWeaponData : WeaponData
    {
        [Header("RANGE WEAPON STATS")]
        public float MinProjectileSpeed;
        public float MaxProjectileSpeed;
    }
}