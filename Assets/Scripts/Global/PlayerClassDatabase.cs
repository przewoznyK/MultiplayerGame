using UnityEngine;

namespace Global.Data
{
    [CreateAssetMenu(menuName = "Data/Player Class Database")]
    public class PlayerClassDatabase : ScriptableObject
    {
        public PlayerClassData[] AvailableClasses;
    }
}