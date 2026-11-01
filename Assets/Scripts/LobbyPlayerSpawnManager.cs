using Core;
using FishNet;
using FishNet.Connection;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;

namespace Lobby
{
    public class LobbyPlayerSpawnManager : PersistentSingleton<LobbyPlayerSpawnManager>
    {
        public List<PlayerLobby> GamePlayers { get; private set; } = new();
        [SerializeField] private PlayerLobby _playerLobbyPrefab;

        private void Start()
        {
            InstanceFinder.SceneManager.OnClientLoadedStartScenes += SpawnLobbyPlayer;
        }

        private void OnDestroy()
        {
            if (InstanceFinder.SceneManager != null)
                InstanceFinder.SceneManager.OnClientLoadedStartScenes -= SpawnLobbyPlayer;
        }

        private void SpawnLobbyPlayer(NetworkConnection conn, bool asServer)
        {
            if (!asServer)
                return;
            PlayerLobby gamePlayerInstance = Instantiate(_playerLobbyPrefab);

            gamePlayerInstance.ConnectionID.Value = conn.ClientId;
            gamePlayerInstance.PlayerIdNumber.Value = GamePlayers.Count + 1;
            gamePlayerInstance.PlayerSteamID.Value = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyID, GamePlayers.Count);

            GamePlayers.Add(gamePlayerInstance);

            InstanceFinder.ServerManager.Spawn(gamePlayerInstance.gameObject, conn);
        }
    }
}