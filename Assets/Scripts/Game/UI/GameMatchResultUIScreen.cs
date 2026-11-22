using FishNet;
using Game.Managers;
using System;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class GameMatchResultUIScreen : MonoBehaviour
    {
        [SerializeField] private GameObject _spectatorButton;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _exitButtonText;
        [SerializeField] private TextMeshProUGUI _playerAliveText;
        [SerializeField] private TextMeshProUGUI _playerKillsText;
        [SerializeField] private TextMeshProUGUI _zombieKillsText;
        [SerializeField] private TextMeshProUGUI _timerText;
        private bool _isLocalWinner;

        public void ShowScreen(bool isVictory, int alivePlayers, int playerKills, int zombieKills, int matchTimeSeconds)
        {
            gameObject.SetActive(true);
            _isLocalWinner = isVictory;
            if (isVictory)
            {
                _titleText.text = "Victory";
                _titleText.color = Color.yellow;

                _exitButtonText.text = "Back to lobby";
                _exitButtonText.color = Color.green;

                _spectatorButton.SetActive(false);
            }
            else
            {
                _titleText.text = "Game over";
                _titleText.color = Color.red;

                if (InstanceFinder.ServerManager.Started)
                    _exitButtonText.text = "Back to lobby (all players)";
                else
                    _exitButtonText.text = "Exit";

                _exitButtonText.color = Color.orange;

                _spectatorButton.SetActive(alivePlayers > 1);
            }

            _playerAliveText.text = $"Alive Players: {alivePlayers}";
            _playerKillsText.text = $"Player Kills: {playerKills}";
            _zombieKillsText.text = $"Zombie Kills: {zombieKills}";

            TimeSpan time = TimeSpan.FromSeconds(matchTimeSeconds);
            _timerText.text = $"Time: {time.ToString(@"mm\:ss")}";
        }

        public void SpectatorModeButton()
        {
            gameObject.SetActive(false);
            Debug.Log("SpectatorMode");
        }

        public void ExitButton()
        {
            if (_isLocalWinner || InstanceFinder.ServerManager.Started)
                GameManager.Instance.ReturnToLobbyServerRpc();
            else
            {
                InstanceFinder.ClientManager.StopConnection();
                SceneTransitionManager.Instance.LoadMainMenuScene();
            }
        }
    }
}