using Steamworks;
using TMPro;
using UnityEngine;

public class LobbyDataEntry : MonoBehaviour
{
    public CSteamID lobbyID;
    public string lobbyName;
    public TextMeshProUGUI lobbyNameText;

    public void SetLobbyData()
    {
        if (lobbyNameText.text == "")
        {

            lobbyNameText.text = "Empty";
        }
        else
        {
            lobbyNameText.text = lobbyName;
        }
    }

    public void JoinLobby()
    {
        SteamLobby.Instance.JoinLobby(lobbyID);
    }
}
