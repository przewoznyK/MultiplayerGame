using Core;
using FishNet.Connection;
using Game.Managers;
using Lobby;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class GamePlayerSpawnManager : NetSingleton<GamePlayerSpawnManager>
    {
        public List<GamePlayer> GamePlayers;
        [SerializeField] private GamePlayer _gamePlayerPrefab;
        [SerializeField] private GameObject _zombiePrefab;
        [SerializeField] private Transform[] _spawnPoints;
        private int _nextSpawnIndex = -1;
        private bool _zombieSpawned;

        public override void OnSpawnServer(NetworkConnection conn)
        {
            Transform spawnPoint = GetSpawnPoint();
            int playerClassIndex = 0;

            PlayerLobby sessionData = LobbyPlayerSpawnManager.Instance.GamePlayers.FirstOrDefault(p => p.Owner == conn);

            if (sessionData != null)
                playerClassIndex = sessionData.ClassIndex.Value;
            else
                Debug.LogWarning($"Missing lobby data for connection {conn.ClientId}. Defaulting to class 0.");

            GamePlayer player = Instantiate(_gamePlayerPrefab, spawnPoint.position, spawnPoint.rotation);
            player.Initialize(playerClassIndex);

            Spawn(player.gameObject, conn, gameObject.scene);
            GameManager.Instance.PlayerJoined();
            GamePlayers.Add(player);

            if (_zombieSpawned == false)
                StartCoroutine(SpawnZombieAfterDelay());
        }

        private IEnumerator SpawnZombieAfterDelay()
        {
            _zombieSpawned = true;

            yield return new WaitForSeconds(2f);
          
            Transform spawnPoint = GetSpawnPoint();
            GameObject zombie = Instantiate(_zombiePrefab, spawnPoint.position, spawnPoint.rotation);

            Spawn(zombie);
        }

        private Transform GetSpawnPoint()
        {
            _nextSpawnIndex++;
            return _spawnPoints[_nextSpawnIndex];
        }
    }
}