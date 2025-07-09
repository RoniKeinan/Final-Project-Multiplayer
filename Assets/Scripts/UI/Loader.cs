using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public static class Loader {


    public enum Scene
    {
        MainMenu,
        Game,
        LoadingScene
    }
    private static Scene targetScene;

    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoaderCallback()
    {

    }
}
