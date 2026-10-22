using Core;
using FishNet;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LobbyController : Singleton<LobbyController>
{
    [SerializeField] private TextMeshProUGUI _lobbyNameText;
    [SerializeField] private Transform _playerListViewContentHolder;
    [SerializeField] private GameObject _playerListItemPrefab;
    [SerializeField] private GameObject _startGameButton;
    private GameObject _localPlayerObject;
    private ulong _currentLobbyID;
    private bool _playerItemCreated = false;
    private List<PlayerListItem> _playerListItems = new();

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
        if(!_playerItemCreated)
        {
            CreateHostPlayerItem();
        }
        
        if(_playerListItems.Count < PlayerSpawnManager.Instance.GamePlayers.Count)
        {
            CreateClientPlayerItem();
        }

        if(_playerListItems.Count == PlayerSpawnManager.Instance.GamePlayers.Count)
        {
            UpdatePlayerItem();
        }
    }

    public void FindLocalPlayer()
    {
        _localPlayerObject = GameObject.Find("LocalGamePlayer");
        _localPlayerObject = _localPlayerObject.GetComponent<PlayerLobbyController>().gameObject;
    }

    public void CreateHostPlayerItem()
    {
        foreach (var player in PlayerSpawnManager.Instance.GamePlayers)
        {
            var newPlayerItem = Instantiate(_playerListItemPrefab);
            var newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();

            newPlayerItemScript.PlayerName = player.PlayerName.Value;
            newPlayerItemScript.ConnectionID = player.ConnectionID.Value;
            newPlayerItemScript.PlayerSteamID = player.PlayerSteamID.Value;
            newPlayerItemScript.SetPlayerValues();

            newPlayerItem.transform.SetParent(_playerListViewContentHolder.transform);
            newPlayerItem.transform.localScale = Vector3.one;
            _playerListItems.Add(newPlayerItemScript);
        }

        _playerItemCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        foreach (var player in PlayerSpawnManager.Instance.GamePlayers)
        {
            if(!_playerListItems.Any(b => b.ConnectionID == player.ConnectionID.Value))
            {
                var newPlayerItem = Instantiate(_playerListItemPrefab) as GameObject;
                var newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();
                newPlayerItemScript.PlayerName = player.PlayerName.Value;
                newPlayerItemScript.ConnectionID = player.ConnectionID.Value;
                newPlayerItemScript.PlayerSteamID = player.PlayerSteamID.Value;
                newPlayerItemScript.SetPlayerValues();

                newPlayerItem.transform.SetParent(_playerListViewContentHolder.transform);
                newPlayerItem.transform.localScale = Vector3.one;
                _playerListItems.Add(newPlayerItemScript);
            }
        }
    }

    public void UpdatePlayerItem()
    {
        foreach(var player in PlayerSpawnManager.Instance.GamePlayers)
        {
            foreach(var playerListItemScript in _playerListItems)
            {
                if(playerListItemScript.ConnectionID == player.ConnectionID.Value)
                {
                    playerListItemScript.PlayerName = player.PlayerName.Value;
                    playerListItemScript.SetPlayerValues();
                }
            }
        }
    }

    public void RemovePlayerItem()
    {
        var playerListItemsToRemove = new List<PlayerListItem>();

        foreach (var playerListItem in _playerListItems)
        {
            if(!PlayerSpawnManager.Instance.GamePlayers.Any(p => p.ConnectionID.Value == playerListItem.ConnectionID))
            {
                playerListItemsToRemove.Add(playerListItem);
            }
        }

        if(playerListItemsToRemove.Count > 0)
        {
            foreach (var playerListItem in playerListItemsToRemove)
            {
                _playerListItems.Remove(playerListItem);
                Destroy(playerListItem.gameObject);
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

    public void ClearPlayerList()
    {
        foreach (var item in _playerListItems)
        {
            if (item != null)
                Destroy(item.gameObject);
        }

        _playerListItems.Clear();
        _playerItemCreated = false;
    }
}