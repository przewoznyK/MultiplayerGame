using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Steamworks;

public class PlayerLobbyController : NetworkBehaviour
{
    public readonly SyncVar<int> ConnectionID = new SyncVar<int>();
    public readonly SyncVar<int> PlayerIdNumber = new SyncVar<int>();
    public readonly SyncVar<ulong> PlayerSteamID = new SyncVar<ulong>();
    public readonly SyncVar<string> PlayerName = new SyncVar<string>();

    private void Awake()
    {
        PlayerName.OnChange += PlayerNameUpdate;
    }

    [ServerRpc]
    public void ServerSetPlayerName(string newName)
    {
        PlayerName.Value = newName;
    }

    private void PlayerNameUpdate(string oldValue, string newValue, bool asServer)
    {
        if (!asServer)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        PlayerSpawnManager.Instance.GamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!PlayerSpawnManager.Instance.GamePlayers.Contains(this))
        {
            PlayerSpawnManager.Instance.GamePlayers.Add(this);
        }
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();

        if (base.IsOwner)
        {
            ServerSetPlayerName(SteamFriends.GetPersonaName());

            gameObject.name = "LocalGamePlayer";
            LobbyController.Instance.FindLocalPlayer();
            LobbyController.Instance.UpdateLobbyName();
        }

        if(InstanceFinder.IsHostStarted)
        {
            LobbyController.Instance.SetIsOwner();
        }
    }
}