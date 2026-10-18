using Core;
using FishNet;
using FishNet.Managing.Scened;

public class SceneTransitionManager : PersistentSingleton<SceneTransitionManager>
{
    public string MainMenuSceneName = "MainMenuScene";
    public string OnlineSceneName = "Lobby";

    public void LoadOnlineScene()
    {
        SceneLoadData sceneData = new SceneLoadData(OnlineSceneName);
        sceneData.ReplaceScenes = ReplaceOption.All;

        InstanceFinder.SceneManager.LoadGlobalScenes(sceneData);
    }

    public void LoadMainMenuScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(MainMenuSceneName);
    }
}