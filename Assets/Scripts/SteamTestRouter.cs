using FishNet;
using FishNet.Transporting.Tugboat;
using UnityEngine;

public class SteamTestRouter : MonoBehaviour
{
    [SerializeField] private GameObject _networkManagerObject;
    [SerializeField] private LocalNetworkTester _localTester;

    public void OnHostGameClicked()
    {
        if (InstanceFinder.TransportManager.Transport is Tugboat)
            _localTester.TestHost();
        else
            SteamLobby.Instance.HostLobby();
    }

    public void OnJoinGameClicked()
    {
        if (InstanceFinder.TransportManager.Transport is Tugboat)
        {
            _localTester.TestJoin();
        }
        else
        {    
            LobbiesListManager.Instance.GetListOfLobbies();
        }
    }
}