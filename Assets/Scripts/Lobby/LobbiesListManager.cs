using Core;
using Steamworks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbiesListManager : Singleton<LobbiesListManager>
{
    [SerializeField] private GameObject _lobbiesMenu;
    [SerializeField] private LobbyDataEntry _lobbyDataItemPrefab;
    [SerializeField] private GameObject _lobbyListContentHolder;
    [SerializeField] private GameObject _mainMenuHolder;
    [SerializeField] private TMP_InputField _searchInputField;
    public List<GameObject> ListOfLobbies = new();

    public void GetListOfLobbies()
    {
        _mainMenuHolder.SetActive(false);
        _lobbiesMenu.SetActive(true);
        _searchInputField.text = "";

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
                ApplySearchFilter(createdItem.gameObject, createdItem.lobbyName, _searchInputField.text);
            }
        }
    }

    public void SearchLobbies(string searchTerm)
    {
        string lowerSearchTerm = searchTerm.ToLower();

        foreach (var lobbyObj in ListOfLobbies)
        {
            if (lobbyObj != null && lobbyObj.TryGetComponent(out LobbyDataEntry entry))
                ApplySearchFilter(lobbyObj, entry.lobbyName, lowerSearchTerm);
        }
    }

    private void ApplySearchFilter(GameObject lobbyObj, string lobbyName, string searchTerm)
    {
        bool isMatch = string.IsNullOrEmpty(searchTerm) || lobbyName.ToLower().Contains(searchTerm.ToLower());
        lobbyObj.SetActive(isMatch);
    }

    public void DestroyLobbies()
    {
        foreach (var item in ListOfLobbies)
            Destroy(item);

        ListOfLobbies.Clear();
    }

    public void ExitLobbies()
    {
        _lobbiesMenu.SetActive(false);
        _mainMenuHolder.SetActive(true);
    }
}