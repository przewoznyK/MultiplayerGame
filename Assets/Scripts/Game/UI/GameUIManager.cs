using Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameUIManager : Singleton<GameUIManager>
    {
        public InteractorUI InteractorUI;
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