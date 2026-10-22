using FishNet;
using FishNet.Transporting;
using UnityEngine;

public class LocalNetworkTester : MonoBehaviour
{
    public void TestHost()
    {
        InstanceFinder.ServerManager.OnServerConnectionState -= OnServerStarted;
        InstanceFinder.ServerManager.OnServerConnectionState += OnServerStarted;
        InstanceFinder.ServerManager.StartConnection();
    }

    private void OnServerStarted(ServerConnectionStateArgs args)
    {
        if (args.ConnectionState != LocalConnectionState.Started)
            return;

        InstanceFinder.ServerManager.OnServerConnectionState -= OnServerStarted;
        SceneTransitionManager.Instance.LoadOnlineScene(SceneTransitionManager.Instance.LobbySceneName);
        InstanceFinder.ClientManager.StartConnection("127.0.0.1");
    }

    public void TestJoin()
    {
        InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnectionState;
        InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionState;
        InstanceFinder.ClientManager.StartConnection("127.0.0.1");
    }

    private void OnClientConnectionState(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Stopped)
        {
            InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnectionState;
            SceneTransitionManager.Instance.LoadMainMenuScene();
        }
    }
}