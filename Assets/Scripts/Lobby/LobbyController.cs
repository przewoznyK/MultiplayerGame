using Core;
using FishNet;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyController : Singleton<LobbyController>
    {
        [SerializeField] private TextMeshProUGUI _lobbyNameText;
        [SerializeField] private Transform _playerListViewContentHolder;
        [SerializeField] private GameObject _playerListItemPrefab;
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _readyButton;
        [SerializeField] private TextMeshProUGUI _readyButtonText;
        private ulong _currentLobbyID;
        private List<PlayerLobbySlot> _playerListItems = new();

        public void Initialize()
        {
            if (InstanceFinder.IsServerStarted)
                _startGameButton.gameObject.SetActive(true);
        }

        public void UpdateLobbyName()
        {
            _currentLobbyID = SteamLobby.Instance.CurrentLobbyID;
            _lobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(_currentLobbyID), "name");
        }

        public void EvaluateReadyState()
        {
            var players = LobbyPlayerSpawnManager.Instance.LobbyPlayerPawns;
            bool allReady = players.Count > 0;

            foreach (LobbyPlayerPawn p in players)
            {
                if (p.IsReady.Value == false)
                {
                    allReady = false;
                    break;
                }
            }

            _startGameButton.interactable = allReady;
        }

        public void UpdateLocalReadyButtonAppearance(bool isReady)
        {
            _readyButtonText.text = isReady ? "CANCEL READY" : "READY";
        }

        public void UpdatePlayerList()
        {
            RemoveStalePlayerItems();
            CreateMissingPlayerItems();
        }

        private void RemoveStalePlayerItems()
        {
            for (int i = _playerListItems.Count - 1; i >= 0; i--)
            {
                var uiSlot = _playerListItems[i];
                bool playerStillExists = LobbyPlayerSpawnManager.Instance.LobbyPlayerPawns.Contains(uiSlot.PlayerData);

                if (playerStillExists == false)
                {
                    Destroy(uiSlot.gameObject);
                    _playerListItems.RemoveAt(i);
                }
            }
        }

        private void CreateMissingPlayerItems()
        {
            foreach (var player in LobbyPlayerSpawnManager.Instance.LobbyPlayerPawns)
            {
                if (_playerListItems.Any(slot => slot.PlayerData == player))
                    continue;

                var newPlayerItem = Instantiate(_playerListItemPrefab, _playerListViewContentHolder);
                var newPlayerItemScript = newPlayerItem.GetComponent<PlayerLobbySlot>();

                newPlayerItemScript.Setup(player);

                var classSelector = newPlayerItem.GetComponentInChildren<LobbyClassSelector>();
                if (classSelector != null)
                    classSelector.Setup(player);

                newPlayerItem.transform.localScale = Vector3.one;
                _playerListItems.Add(newPlayerItemScript);
            }
        }

        #region Button Callbacks
        public void OnClickStartGameButton()
        {
            SceneTransitionManager.Instance.LoadOnlineScene(SceneTransitionManager.Instance.GameScene);
        }

        public void OnClickReadyButton()
        {
            foreach (var player in LobbyPlayerSpawnManager.Instance.LobbyPlayerPawns)
            {
                if (player.IsOwner)
                {
                    player.ToggleReadyServerRpc();
                    break;
                }
            }
        }

        public void OnClickExitLobbyButton()
        {
            if (InstanceFinder.IsClientStarted)
                InstanceFinder.ClientManager.StopConnection();

            if (InstanceFinder.IsServerStarted)
                InstanceFinder.ServerManager.StopConnection(true);

            if (SteamManager.Initialized && SteamLobby.Instance.CurrentLobbyID != 0)
            {
                CSteamID lobbyID = new CSteamID(SteamLobby.Instance.CurrentLobbyID);
                SteamMatchmaking.LeaveLobby(lobbyID);
                SteamLobby.Instance.ClearCurrentLobby();
            }

            SceneTransitionManager.Instance.LoadMainMenuScene();
        }
        #endregion
    }
}