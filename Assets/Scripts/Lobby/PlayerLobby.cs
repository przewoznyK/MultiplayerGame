using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Steamworks;

namespace Lobby
{
    public class PlayerLobby : NetworkBehaviour
    {
        public readonly SyncVar<int> ConnectionID = new SyncVar<int>();
        public readonly SyncVar<int> PlayerIdNumber = new SyncVar<int>();
        public readonly SyncVar<ulong> PlayerSteamID = new SyncVar<ulong>();
        public readonly SyncVar<string> PlayerName = new SyncVar<string>();
        public readonly SyncVar<int> ClassIndex = new SyncVar<int>();

        private void Awake()
        {
            PlayerName.OnChange += PlayerNameUpdate;
        }

        [ServerRpc]
        public void CmdChangeClass(int direction, int maxClasses)
        {
            int newIndex = ClassIndex.Value + direction;

            if (newIndex < 0)
                newIndex = maxClasses - 1;
            else if (newIndex >= maxClasses)
                newIndex = 0;

            ClassIndex.Value = newIndex;
        }

        [ServerRpc]
        public void ServerSetPlayerName(string newName)
        {
            PlayerName.Value = newName;
        }

        private void PlayerNameUpdate(string oldValue, string newValue, bool asServer)
        {
            if (!asServer)
                LobbyController.Instance.UpdatePlayerList();
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            if (LobbyPlayerSpawnManager.Instance != null)
                LobbyPlayerSpawnManager.Instance.GamePlayers.Remove(this);

            if (LobbyController.Instance != null)
                LobbyController.Instance.UpdatePlayerList();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!LobbyPlayerSpawnManager.Instance.GamePlayers.Contains(this))
                LobbyPlayerSpawnManager.Instance.GamePlayers.Add(this);

            LobbyController.Instance.UpdateLobbyName();
            LobbyController.Instance.UpdatePlayerList();

            if (base.IsOwner)
            {
                ServerSetPlayerName(SteamFriends.GetPersonaName());
                gameObject.name = "LocalGamePlayer";
                LobbyController.Instance.UpdateLobbyName();
            }

            if (InstanceFinder.IsHostStarted)
                LobbyController.Instance.SetIsOwner();
        }
    }
}