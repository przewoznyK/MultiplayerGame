using Core;
using FishNet;
using FishNet.Connection;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnManager : PersistentSingleton<PlayerSpawnManager>
{
    public List<PlayerLobbyController> GamePlayers { get; private set; } = new();
    [SerializeField] private PlayerLobbyController _playerLobbyPrefab;
    [SerializeField] private GameObject _gmePlayerPrefab;

    private void Start()
    {
        InstanceFinder.SceneManager.OnClientLoadedStartScenes += SpawnLobbyPlayer;
    }

    private void OnDestroy()
    {
        if (InstanceFinder.SceneManager != null)
        {
            InstanceFinder.SceneManager.OnClientLoadedStartScenes -= SpawnLobbyPlayer;
        }
    }

    private void SpawnLobbyPlayer(NetworkConnection conn, bool asServer)
    {
        if (!asServer) 
            return;

        PlayerLobbyController gamePlayerInstance = Instantiate(_playerLobbyPrefab);

        gamePlayerInstance.ConnectionID.Value = conn.ClientId;
        gamePlayerInstance.PlayerIdNumber.Value = GamePlayers.Count + 1;
        gamePlayerInstance.PlayerSteamID.Value = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyID, GamePlayers.Count);

        GamePlayers.Add(gamePlayerInstance);

        InstanceFinder.ServerManager.Spawn(gamePlayerInstance.gameObject, conn);
    }

    public void SpawnGamePlayer(NetworkConnection conn)
    {
        if (!InstanceFinder.IsServerStarted)
            return;

        GameObject player = Instantiate(_gmePlayerPrefab, Vector3.zero, Quaternion.identity);
        InstanceFinder.ServerManager.Spawn(player, conn);
    }
}