using Core;
using FishNet;
using FishNet.Managing.Scened;

public class SceneTransitionManager : PersistentSingleton<SceneTransitionManager>
{
    public string MainMenuSceneName { get; private set; } = "MainMenuScene";
    public string LobbySceneName { get; private set; } = "LobbyScene";
    public string GameScene { get; private set; } = "GameScene";

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