using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GamePlayerSpawner : NetworkBehaviour
    {
        public static GamePlayerSpawner Instance; 
        [SerializeField] private GameObject _gamePlayerPrefab;
        [SerializeField] private GameObject _zombiePrefab;
        [SerializeField] private Transform[] _spawnPoints;
        private int _nextSpawnIndex = -1;
        public List<GameObject> Players;
        private bool _zombieSpawned;

        private void Awake()
        {
            Instance = this;
        }

        public override void OnSpawnServer(NetworkConnection conn)
        {
            Transform spawnPoint = GetSpawnPoint();
            GameObject player = Instantiate(_gamePlayerPrefab, spawnPoint.position, spawnPoint.rotation);

            Spawn(player, conn, gameObject.scene);
            Players.Add(player);
            if (!_zombieSpawned)
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