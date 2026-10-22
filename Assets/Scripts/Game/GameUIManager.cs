using Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GameUIManager : Singleton<GameUIManager>
    {
        [SerializeField] private Slider _playerHealthSlider;
        
        public void InitializePlayerHealthSlider(int maxHp)
        {
            _playerHealthSlider.maxValue = maxHp;
            UpdatePlayerHp(maxHp);
        }

        public void UpdatePlayerHp(int currentHp)
        {
            _playerHealthSlider.value = currentHp;
        }
    }
}