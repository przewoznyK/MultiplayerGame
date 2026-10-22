using Core;
using FishNet;
using FishNet.Managing.Scened;

public class SceneTransitionManager : PersistentSingleton<SceneTransitionManager>
{
    public string MainMenuSceneName = "MainMenuScene";
    public string LobbySceneName = "Lobby";
    public string GameScene = "GameScene";

    public void LoadOnlineScene(string sceneName)
    {
        SceneLoadData sceneData = new SceneLoadData(sceneName);
        sceneData.ReplaceScenes = ReplaceOption.All;

        InstanceFinder.SceneManager.LoadGlobalScenes(sceneData);
    }

    public void LoadMainMenuScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(MainMenuSceneName);
    }
}