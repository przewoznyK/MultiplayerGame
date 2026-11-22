using Core;
using FishNet.Connection;
using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

namespace Lobby
{
    public class LobbyPlayerSpawnManager : NetSingleton<LobbyPlayerSpawnManager>
    {
        public List<LobbyPlayerPawn> LobbyPlayerPawns { get; private set; } = new();
        [SerializeField] private LobbyPlayerPawn _lobbyPawnPrefab;

        public void SpawnPawn(NetworkConnection clientConn)
        {
            NetworkObject newPawn = Instantiate(_lobbyPawnPrefab);
            ServerManager.Spawn(newPawn, clientConn);
        }
    }
}