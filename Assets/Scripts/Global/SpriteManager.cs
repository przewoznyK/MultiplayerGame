using Core;
using System.Collections.Generic;
using UnityEngine;

namespace Global
{
    public class SpriteManager : PersistentSingleton<SpriteManager>
    {
        [System.Serializable]
        public struct SpriteData
        {
            public string Name;
            public Sprite Sprite;
        }

        [SerializeField] private SpriteData[] _spriteDatabase;
        [SerializeField] private Sprite[] _classSprites;

        private readonly Dictionary<string, Sprite> _spriteDictionary = new Dictionary<string, Sprite>();

        private void Start()
        {
            foreach (SpriteData data in _spriteDatabase)
            {
                if (_spriteDictionary.ContainsKey(data.Name) == false)
                    _spriteDictionary.Add(data.Name, data.Sprite);
                else
                    Debug.LogWarning($"SpriteManager: Duplicate sprite name '{data.Name}'!");
            }
        }

        public Sprite GetSprite(string spriteName)
        {
            if (_spriteDictionary.TryGetValue(spriteName, out Sprite foundSprite))
                return foundSprite;

            Debug.LogError($"SpriteManager: Sprite name doesn't exist '{spriteName}'!");
            return null;
        }

        public Sprite GetClassSpriteById(int classId)
        {
            if (classId >= 0 && classId < _classSprites.Length)
                return _classSprites[classId];

            Debug.LogError($"SpriteManager: Invalid class ID '{classId}'!");
            return null;
        }

        public int GetClassCount()
        {
            return _classSprites.Length;
        }
    }
}