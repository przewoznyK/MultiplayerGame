using Steamworks;
using System.Collections.Generic;
using FishNet.Transporting;
using Core;
using UnityEngine;

public class SteamLobby : PersistentSingleton<SteamLobby>
{
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    protected Callback<LobbyMatchList_t> LobbyList;
    protected Callback<LobbyDataUpdate_t> LobbyDataUpdated;

    public List<CSteamID> lobbyIDs = new();

    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";

    private void Start()
    {
        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        LobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
        LobbyDataUpdated = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
    }

    private bool _isHost = false;

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
            return;

        _isHost = true;

        string hostSteamId = SteamUser.GetSteamID().ToString();
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, hostSteamId);
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s lobby");

        FishNet.InstanceFinder.ServerManager.OnServerConnectionState += OnServerStarted;
        FishNet.InstanceFinder.ServerManager.StartConnection();
    }
    private void OnServerStarted(ServerConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            FishNet.InstanceFinder.ServerManager.OnServerConnectionState -= OnServerStarted;
            FishNet.InstanceFinder.ClientManager.StartConnection();
            SceneTransitionManager.Instance.LoadOnlineScene(SceneTransitionManager.Instance.LobbySceneName);
        }
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        CurrentLobbyID = callback.m_ulSteamIDLobby;

        if (_isHost)
            return;

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        FishNet.InstanceFinder.ClientManager.StartConnection(hostAddress);
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
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
        if (lobbyIDs.Count > 0)
            lobbyIDs.Clear();

        SteamMatchmaking.AddRequestLobbyListResultCountFilter(60);
        SteamMatchmaking.RequestLobbyList();
    }

    public void OnGetLobbyList(LobbyMatchList_t result)
    {
        if (LobbiesListManager.Instance.ListOfLobbies.Count > 0)
            LobbiesListManager.Instance.DestroyLobbies();

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

    public void ClearCurrentLobby()
    {
        CurrentLobbyID = 0;
    }
}