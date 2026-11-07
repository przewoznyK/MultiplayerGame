using Global;
using Global.Enums;
using UnityEngine;

namespace Game
{
    public class GamePlayerStats : MonoBehaviour
    {
        private PlayerClassData _classData;

        public void Initialize(PlayerClassData classData)
        {
            _classData = classData;
        }

        public int GetTotalDamage(int baseDamage, WeaponType category)
        {
            int totalDamage = baseDamage;

            switch (category)
            {
                case WeaponType.Sword:
                    totalDamage += _classData.BonusSwordDamage;
                    break;
            }

            return totalDamage;
        }

        public float GetTotalMaxArrowSpeed(float baseSpeed)
        {
            return baseSpeed + _classData.BonusMaxArrowSpeed;
        }
    }
}