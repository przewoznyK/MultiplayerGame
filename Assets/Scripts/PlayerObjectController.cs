using FishNet.Object;
using FishNet.Object.Synchronizing;
using Steamworks;
using UnityEngine;

public class PlayerObjectController : NetworkBehaviour
{
    public readonly SyncVar<int> ConnectionID = new SyncVar<int>();
    public readonly SyncVar<int> PlayerIdNumber = new SyncVar<int>();
    public readonly SyncVar<ulong> PlayerSteamID = new SyncVar<ulong>();
    public readonly SyncVar<string> PlayerName = new SyncVar<string>();

    private PlayerSpawnManager _manager;
    private PlayerSpawnManager Manager
    {
        get
        {
            if(_manager != null)
            {
                return _manager;
            }
            return _manager = PlayerSpawnManager.Instance;
        }
    }

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

        Manager.GamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!Manager.GamePlayers.Contains(this))
        {
            Manager.GamePlayers.Add(this);
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
    }
}