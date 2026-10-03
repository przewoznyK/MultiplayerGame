using NUnit.Framework;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;

public class LobbiesListManager : MonoBehaviour
{
    public static LobbiesListManager Instance;
    public GameObject lobbiesMenu;
    public GameObject lobbyDataItemPrefab;
    public GameObject lobbyListContent;
    public GameObject lobbiesButton;
    public GameObject hostButton;
    public List<GameObject> listOfLobbies = new();

    private void Awake()
    {
        Instance = this;
    }

    public void GetListOfLobbies()
    {
        lobbiesButton.SetActive(false);
        hostButton.SetActive(false);
        lobbiesMenu.SetActive(true);

        SteamLobby.Instance.GetLobbiesList();
    }

    public void DisplayLobbies(List<CSteamID> lobbyIDs, LobbyDataUpdate_t result)
    {
        for (int i = 0; i < lobbyIDs.Count; i++)
        {
            if (lobbyIDs[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                GameObject createdItem = Instantiate(lobbyDataItemPrefab);
                createdItem.GetComponent<LobbyDataEntry>().lobbyID = (CSteamID)lobbyIDs[i].m_SteamID;
                createdItem.GetComponent<LobbyDataEntry>().lobbyName = SteamMatchmaking.GetLobbyData(lobbyIDs[i], "name");
                createdItem.GetComponent<LobbyDataEntry>().SetLobbyData();
                createdItem.transform.SetParent(lobbyListContent.transform);
                createdItem.transform.localScale = Vector3.one;
                listOfLobbies.Add(createdItem);
            }
        }
    }

    public void DestroyLobbies()
    {
        foreach (var item in listOfLobbies)
        {
            Destroy(item);
        }

        listOfLobbies.Clear();
    }
}
