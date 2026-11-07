using Core;
using Global.Data;
using UnityEngine;

namespace Global.Managers
{
    public class PlayerClassManager : PersistentSingleton<PlayerClassManager>
    {
        [SerializeField] private PlayerClassDatabase _classDatabase;

        public PlayerClassData GetClassById(int classId)
        {
            PlayerClassData playerClassData = _classDatabase.AvailableClasses[classId];

            if(playerClassData != null)
                return playerClassData;
            
            Debug.LogError($"ClassManager: Invalid class ID '{classId}'!");
            return null;
        }

        public int GetClassCount()
        {
            return _classDatabase.AvailableClasses.Length;
        }
    }
}