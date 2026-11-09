using Core;
using Game.Managers;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameUIManager : Singleton<GameUIManager>
    {
        public InteractorUI InteractorUI;
        [SerializeField] private Slider _playerHealthSlider;
        [SerializeField] private TextMeshProUGUI _playerAliveText;
        [SerializeField] private TextMeshProUGUI _playerKillsText;
        [SerializeField] private TextMeshProUGUI _zombieKillsText;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private GameMatchResultUIScreen _gameOverUIScreen;
        [SerializeField] private ServerMessageItem _serverMessagePrefab;
        [SerializeField] private Transform _messagesHolder;

        private GamePlayerCameraController _playerCameraController;
        private void Start()
        {
            GameManager.Instance.AlivePlayersCount.OnChange += OnAlivePlayersChanged;
            GameManager.Instance.MatchTimeSeconds.OnChange += OnTimerChanged;
        }

        private void OnAlivePlayersChanged(int prev, int next, bool asServer)
        {
            if (asServer == false)
                _playerAliveText.text = $"Players alive: {next}";
        }

        public void InitializePlayerHealthSlider(int maxHp)
        {
            _playerHealthSlider.maxValue = maxHp;
            UpdatePlayerHp(maxHp);
        }

        public void UpdatePlayerHp(int currentHp)
        {
            _playerHealthSlider.value = currentHp;
        }

        public void AssignLocalPlayer(GamePlayer localPlayer, GamePlayerCameraController playerCameraController)
        {
            _playerCameraController = playerCameraController;
            UpdatePlayerKills(localPlayer.PlayerKills.Value);
            UpdateZombieKills(localPlayer.ZombieKills.Value);
        }

        private void OnTimerChanged(int prev, int next, bool asServer)
        {
            if (asServer == false)
            {
                TimeSpan time = TimeSpan.FromSeconds(next);
                _timerText.text = time.ToString(@"mm\:ss");
            }
        }

        public void UpdatePlayerKills(int kills)
        {
            _playerKillsText.text = $"Player kills: {kills}";
        }

        public void UpdateZombieKills(int kills)
        {
            _zombieKillsText.text = $"Zombie kills: {kills}";
        }

        public void ShowMatchEndScreen(bool isVictory, int alivePlayers, int playerKills, int zombieKills, int matchTime)
        {
            _gameOverUIScreen.ShowScreen(isVictory, alivePlayers, playerKills, zombieKills, matchTime);
        }

        public void EnableSpectatorMode()
        {
            _playerCameraController.StartSpectating();
            _gameOverUIScreen.gameObject.SetActive(false);
        }

        public void ShowServerMessage(string message)
        {
            ServerMessageItem newMessage = Instantiate(_serverMessagePrefab, _messagesHolder);
            newMessage.Setup(message);
        }
    }
}