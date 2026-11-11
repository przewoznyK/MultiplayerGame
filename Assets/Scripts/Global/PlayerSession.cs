using FishNet;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Lobby;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;

namespace Global
{
    public class PlayerSession : NetworkBehaviour
    {
        public readonly SyncVar<int> ConnectionID = new SyncVar<int>();
        public readonly SyncVar<int> PlayerIdNumber = new SyncVar<int>();
        public readonly SyncVar<ulong> PlayerSteamID = new SyncVar<ulong>();
        public readonly SyncVar<string> PlayerName = new SyncVar<string>();
        public readonly SyncVar<int> ClassIndex = new SyncVar<int>();
        public static Dictionary<int, PlayerSession> AllSessions = new Dictionary<int, PlayerSession>();
        public override void OnStartClient()
        {
            base.OnStartClient();

            AllSessions[OwnerId] = this;

            if (IsOwner)
            {
                ServerSetPlayerName(SteamFriends.GetPersonaName());
                gameObject.name = $"PlayerSession_{PlayerName.Value}";

                InstanceFinder.SceneManager.OnLoadEnd += OnSceneLoaded;

                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == SceneTransitionManager.Instance.LobbySceneName)
                    CmdRequestLobbyPawn();
            }
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            if (AllSessions.ContainsKey(OwnerId))
                AllSessions.Remove(OwnerId);

            if (IsOwner && InstanceFinder.SceneManager != null)
                InstanceFinder.SceneManager.OnLoadEnd -= OnSceneLoaded;
        }

        private void OnSceneLoaded(SceneLoadEndEventArgs args)
        {
            if (args.QueueData.AsServer)
                return;

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == SceneTransitionManager.Instance.LobbySceneName)
                CmdRequestLobbyPawn();
        }

        [ServerRpc]
        public void CmdRequestLobbyPawn()
        {
            if (LobbyPlayerSpawnManager.Instance != null)
                LobbyPlayerSpawnManager.Instance.SpawnPawn(Owner);
        }

        [ServerRpc]
        public void ServerSetPlayerName(string newName)
        {
            PlayerName.Value = newName;
        }

        [ServerRpc]
        public void ServerSetClassIndex(int newIndex)
        {
            ClassIndex.Value = newIndex;
        }
    }
}