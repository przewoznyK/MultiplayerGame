using Core;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Game.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Managers
{
    public class GameManager : NetSingleton<GameManager>
    {
        public readonly SyncVar<int> AlivePlayersCount = new SyncVar<int>(0);
        public readonly SyncVar<int> MatchTimeSeconds = new SyncVar<int>(0);

        public override void OnStartServer()
        {
            base.OnStartServer();
            StartCoroutine(MatchTimerRoutine());
        }

        [Server]
        public void PlayerJoined()
        {
            AlivePlayersCount.Value++;
        }

        [Server]
        public void PlayerDied()
        {
            AlivePlayersCount.Value--;

            if (AlivePlayersCount.Value == 1)
                DeclareWinner();
        }

        [Server]
        private void DeclareWinner()
        {
            List<GamePlayer> activePlayers = GamePlayerSpawnManager.Instance.GamePlayers;

            foreach (GamePlayer player in activePlayers)
            {
                if (player.GamePlayerHealth.IsDead.Value == false)
                {
                    player.TargetMatchWon(player.Owner, AlivePlayersCount.Value, player.PlayerKills.Value, player.ZombieKills.Value, MatchTimeSeconds.Value);
                    break;
                }
            }
        }

        [Server]
        public void PlayerDisconnected(bool wasAlive)
        {
            if (wasAlive)
                AlivePlayersCount.Value--;
        }

        private IEnumerator MatchTimerRoutine()
        {
            while (IsServerInitialized)
            {
                yield return new WaitForSeconds(1f);
                MatchTimeSeconds.Value++;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void ReturnToLobbyServerRpc()
        {
            SceneTransitionManager.Instance.LoadOnlineScene(SceneTransitionManager.Instance.LobbySceneName);
        }

        [Server]
        public void AnnounceServerMessage(string message)
        {
            ObserversRpcShowServerMessage(message);
        }

        [ObserversRpc]
        private void ObserversRpcShowServerMessage(string message)
        {
            GameUIManager.Instance.ShowServerMessage(message);
        }
    }
}