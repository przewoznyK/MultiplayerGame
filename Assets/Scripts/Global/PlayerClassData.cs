using Global.Enums;
using UnityEngine;

namespace Global
{
    [CreateAssetMenu(menuName = "Data/Player Class Data")]
    public class PlayerClassData : ScriptableObject
    {
        public Sprite ClassSprite;
        [TextArea(2, 4)]
        public string ClassBuffDescription;
        public PlayerClassType ClassType;
        public int BonusSwordDamage = 0;
        public float BonusMaxArrowSpeed = 0f;
    }
}