using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class LobbiesListManager : Singleton<LobbiesListManager>
{
    [SerializeField] private GameObject _lobbiesMenu;
    [SerializeField] private LobbyDataEntry _lobbyDataItemPrefab;
    [SerializeField] private GameObject _lobbyListContentHolder;
    [SerializeField] private GameObject _mainMenuHolder;
    public List<GameObject> ListOfLobbies = new();

    public void GetListOfLobbies()
    {
        _mainMenuHolder.SetActive(false);
        _lobbiesMenu.SetActive(true);

        SteamLobby.Instance.GetLobbiesList();
    }

    public void DisplayLobbies(List<CSteamID> lobbyIDs, LobbyDataUpdate_t result)
    {
        for (int i = 0; i < lobbyIDs.Count; i++)
        {
            if (lobbyIDs[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                LobbyDataEntry createdItem = Instantiate(_lobbyDataItemPrefab);
                createdItem.lobbyID = (CSteamID)lobbyIDs[i].m_SteamID;
                createdItem.lobbyName = SteamMatchmaking.GetLobbyData(lobbyIDs[i], "name");
                createdItem.SetLobbyData();
                createdItem.transform.SetParent(_lobbyListContentHolder.transform);
                createdItem.transform.localScale = Vector3.one;
                ListOfLobbies.Add(createdItem.gameObject);
            }
        }
    }

    public void DestroyLobbies()
    {
        foreach (var item in ListOfLobbies)
        {
            Destroy(item);
        }

        ListOfLobbies.Clear();
    }

    public void ExitLobbies()
    {
        _lobbiesMenu.SetActive(false);
        _mainMenuHolder.SetActive(true);
    }
}