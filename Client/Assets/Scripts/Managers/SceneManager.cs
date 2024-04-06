using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;
using System.Collections;

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
        Scenes scene = new Scenes();
        scene = (Scenes)Enum.Parse(typeof(Scenes), sceneName);
        UserDataManager.Instance.Scene = scene;
        SceneManager.LoadScene(sceneName);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Shop" || scene.name == "Lobby")
        {
            GetCharacterInfo();
        }
    }

    IEnumerator GetCharacterInfo()
    {
        string url = GameURL.DBServer.Server_URL + GameURL.DBServer.getCharacterInfoPath;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", $"Bearer {UserDataManager.Instance.AccessToken}");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                CharacterInfoGetResponse characterInfo = JsonUtility.FromJson<CharacterInfoGetResponse>(jsonResponse);

                UserDataManager.Instance.NickName = characterInfo.nickname;
                UserDataManager.Instance.Stat = characterInfo.stat;
                UserDataManager.Instance.LevelId = characterInfo.levelId;
                UserDataManager.Instance.CurrentExp = characterInfo.currentExp;
                UserDataManager.Instance.TotalExp = characterInfo.totalExp;
                UserDataManager.Instance.NationId = characterInfo.nationId;
                UserDataManager.Instance.NationNm = characterInfo.nationNm;
                UserDataManager.Instance.JobId = characterInfo.jobId;
                UserDataManager.Instance.JobNm = characterInfo.jobNm;
                UserDataManager.Instance.Money = characterInfo.money;
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }
    }
}
