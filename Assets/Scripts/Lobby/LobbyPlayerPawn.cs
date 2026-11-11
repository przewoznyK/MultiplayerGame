using FishNet.Object;
using FishNet.Object.Synchronizing;
using Global;
using System.Linq;
using UnityEngine;

namespace Lobby
{
    public class LobbyPlayerPawn : NetworkBehaviour
    {
        public PlayerSession SessionData { get; private set; }
        public readonly SyncVar<bool> IsReady = new SyncVar<bool>();
        private int _localClassIndex;

        private void Awake()
        {
            IsReady.OnChange += OnReadyChanged;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (PlayerSession.AllSessions.TryGetValue(this.OwnerId, out PlayerSession mySession))
                SessionData = mySession;
            else
                Debug.LogError($"PlayerSession not found for OwnerId: {this.OwnerId}!");

            if (LobbyPlayerSpawnManager.Instance.LobbyPlayerPawns.Contains(this) == false)
                LobbyPlayerSpawnManager.Instance.LobbyPlayerPawns.Add(this);

            LobbyController.Instance.UpdateLobbyName();
            LobbyController.Instance.UpdatePlayerList();

            if (IsOwner)
                LobbyController.Instance.Initialize();
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            if (LobbyPlayerSpawnManager.Instance != null)
                LobbyPlayerSpawnManager.Instance.LobbyPlayerPawns.Remove(this);

            if (LobbyController.Instance != null)
                LobbyController.Instance.UpdatePlayerList();
        }

        [ServerRpc]
        public void ToggleReadyServerRpc()
        {
            IsReady.Value = !IsReady.Value;
        }

        private void OnReadyChanged(bool oldValue, bool newValue, bool asServer)
        {
            if (asServer == false && LobbyController.Instance != null)
            {
                LobbyController.Instance.EvaluateReadyState();

                if (IsOwner)
                    LobbyController.Instance.UpdateLocalReadyButtonAppearance(newValue);
            }
        }

        public void ChangeClass(int direction, int maxClasses)
        {
            if (IsOwner == false)
                return;

            int newIndex = _localClassIndex + direction;

            if (newIndex < 0)
                newIndex = maxClasses - 1;
            else if (newIndex >= maxClasses)
                newIndex = 0;

            _localClassIndex = newIndex;

            SessionData.ServerSetClassIndex(newIndex);
        }
    }
}