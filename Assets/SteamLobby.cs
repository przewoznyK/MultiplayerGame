using Steamworks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using FishNet.Managing;
using System.Collections.Generic;


public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance { get; private set; }
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    protected Callback<LobbyMatchList_t> LobbyList;
    protected Callback<LobbyDataUpdate_t> LobbyDataUpdated;
    public List<CSteamID> lobbyIDs = new();

    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";

    [SerializeField] private Button _hostButton;
    [SerializeField] private TextMeshProUGUI _lobbyNameText;
    [SerializeField] private NetworkManager _networkManager;

    private void Start()
    {
        Instance = this;

        if (!SteamManager.Initialized) return;

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        LobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
        LobbyDataUpdated = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            return;
        }

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s lobby");

        _networkManager.ServerManager.StartConnection();
        _networkManager.ClientManager.StartConnection();
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        _hostButton.gameObject.SetActive(false);
        CurrentLobbyID = callback.m_ulSteamIDLobby;
        _lobbyNameText.gameObject.SetActive(true);
        _lobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");

        if (_networkManager.IsServer)
        { 
            return;
        }

        _networkManager.ClientManager.StartConnection();
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 4);
    }

    public void JoinLobby(CSteamID lobbyID)
    {
        SteamMatchmaking.JoinLobby(lobbyID);
    }

    public void GetLobbiesList()
    {
        if(lobbyIDs.Count > 0)
        {
            lobbyIDs.Clear();
        }

        SteamMatchmaking.AddRequestLobbyListResultCountFilter(60);
        SteamMatchmaking.RequestLobbyList();
    }

    public void OnGetLobbyList(LobbyMatchList_t result)
    {
        if(LobbiesListManager.Instance.listOfLobbies.Count > 0)
        {
            LobbiesListManager.Instance.DestroyLobbies();
        }

        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIDs.Add(lobbyID);
            SteamMatchmaking.RequestLobbyData(lobbyID);
        }
    }

    public void OnGetLobbyData(LobbyDataUpdate_t result)
    {
        LobbiesListManager.Instance.DisplayLobbies(lobbyIDs, result);
    }
}