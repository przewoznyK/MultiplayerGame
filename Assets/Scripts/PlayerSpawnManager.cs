using Core;
using FishNet;
using FishNet.Connection;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnManager : PersistentSingleton<PlayerSpawnManager>
{
    public List<PlayerObjectController> GamePlayers { get; private set; } = new();
    [SerializeField] private PlayerObjectController _gamePlayerPrefab;

    private void Start()
    {
        InstanceFinder.SceneManager.OnClientLoadedStartScenes += SpawnPlayer;
    }

    private void OnDestroy()
    {
        if (InstanceFinder.SceneManager != null)
        {
            InstanceFinder.SceneManager.OnClientLoadedStartScenes -= SpawnPlayer;
        }
    }

    private void SpawnPlayer(NetworkConnection conn, bool asServer)
    {
        if (!asServer) return;

        PlayerObjectController gamePlayerInstance = Instantiate(_gamePlayerPrefab);

        gamePlayerInstance.ConnectionID.Value = conn.ClientId;
        gamePlayerInstance.PlayerIdNumber.Value = GamePlayers.Count + 1;
        gamePlayerInstance.PlayerSteamID.Value = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyID, GamePlayers.Count);

        GamePlayers.Add(gamePlayerInstance);

        InstanceFinder.ServerManager.Spawn(gamePlayerInstance.gameObject, conn);
    }
}