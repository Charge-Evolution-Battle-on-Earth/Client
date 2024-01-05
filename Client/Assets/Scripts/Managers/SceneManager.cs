using UnityEngine;
using UnityEngine.SceneManagement;
public enum Scenes
{
    Login,
    Register,
    Choice,
    Lobby,
    Ingame,
    Inventory,
    Shop,
    Status,
    Setting
}
public class CustomSceneManager : MonoBehaviour
{
    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
