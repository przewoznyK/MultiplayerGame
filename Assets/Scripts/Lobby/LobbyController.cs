using Core;
using FishNet;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Lobby
{
    public class LobbyController : Singleton<LobbyController>
    {
        [SerializeField] private TextMeshProUGUI _lobbyNameText;
        [SerializeField] private Transform _playerListViewContentHolder;
        [SerializeField] private GameObject _playerListItemPrefab;
        [SerializeField] private GameObject _startGameButton;
        private ulong _currentLobbyID;

        private List<PlayerLobbySlot> _playerListItems = new();

        public void SetIsOwner()
        {
            _startGameButton.SetActive(true);
        }

        public void StartGame()
        {
            SceneTransitionManager.Instance.LoadOnlineScene(SceneTransitionManager.Instance.GameScene);
        }

        public void UpdateLobbyName()
        {
            _currentLobbyID = SteamLobby.Instance.CurrentLobbyID;
            _lobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(_currentLobbyID), "name");
        }

        public void UpdatePlayerList()
        {
            RemoveStalePlayerItems();
            CreateMissingPlayerItems();
            UpdateExistingItems();
        }

        private void RemoveStalePlayerItems()
        {
            for (int i = _playerListItems.Count - 1; i >= 0; i--)
            {
                var uiSlot = _playerListItems[i];

                bool playerStillExists = LobbyPlayerSpawnManager.Instance.GamePlayers.Any(p => p.ConnectionID.Value == uiSlot.ConnectionID);

                if (playerStillExists == false)
                {
                    Destroy(uiSlot.gameObject);
                    _playerListItems.RemoveAt(i);
                }
            }
        }

        private void CreateMissingPlayerItems()
        {
            foreach (var player in LobbyPlayerSpawnManager.Instance.GamePlayers)
            {
                if (_playerListItems.Any(b => b.ConnectionID == player.ConnectionID.Value))
                    continue;

                var newPlayerItem = Instantiate(_playerListItemPrefab, _playerListViewContentHolder);
                var newPlayerItemScript = newPlayerItem.GetComponent<PlayerLobbySlot>();

                newPlayerItemScript.PlayerName = player.PlayerName.Value;
                newPlayerItemScript.ConnectionID = player.ConnectionID.Value;
                newPlayerItemScript.PlayerSteamID = player.PlayerSteamID.Value;
                newPlayerItemScript.SetPlayerValues();

                var classSelector = newPlayerItem.GetComponentInChildren<LobbyClassSelector>();
                if (classSelector != null)
                    classSelector.Setup(player);

                newPlayerItem.transform.localScale = Vector3.one;
                _playerListItems.Add(newPlayerItemScript);
            }
        }

        private void UpdateExistingItems()
        {
            foreach (var player in LobbyPlayerSpawnManager.Instance.GamePlayers)
            {
                foreach (var playerListItemScript in _playerListItems)
                {
                    if (playerListItemScript.ConnectionID == player.ConnectionID.Value)
                    {
                        playerListItemScript.PlayerName = player.PlayerName.Value;
                        playerListItemScript.SetPlayerValues();
                    }
                }
            }
        }

        public void ExitLobby()
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
    }
}