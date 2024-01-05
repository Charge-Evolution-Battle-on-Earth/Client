using UnityEngine.SceneManagement;

public static class SceneController
{
    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
