using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public enum Scenes
    {
        Login,
        Register,
        Choice,
        Lobby,
        Ingame,
        Inventory,
        Shop,
        Status
    }
    Scenes sceneNumber;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SceneChangeRegister()
    {
        SceneManager.LoadScene("Register");
    }
    public void SceneChangeChoice()
    {
        SceneManager.LoadScene("Choice");
    }
    public void SceneChangeLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
    public void SceneChangeIngame()
    {
        SceneManager.LoadScene("Ingame");
    }
    public void SceneChangeInventory()
    {
        SceneManager.LoadScene("Inventory");
    }
    public void SceneChangeShop()
    {
        SceneManager.LoadScene("Shop");
    }
    public void SceneChangeStatus()
    {
        SceneManager.LoadScene("Status");
    }
}
