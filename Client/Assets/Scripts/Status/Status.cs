using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class StatusSceneController : MonoBehaviour
{
    public TMP_Text characterInfoText;

    void Start()
    {
        StartCoroutine(GetCharacterInfo());
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

                DisplayCharacterInfo(characterInfo);
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }
    }

    void DisplayCharacterInfo(CharacterInfoGetResponse characterInfo)
    {
        string characterInfoStr =
            $"캐릭터 정보\n" +
            $"나라: {characterInfo.nationNm}\n" +
            $"직업: {characterInfo.jobNm}\n" +
            $"HP: {characterInfo.stat.hp}\n" +
            $"ATK: {characterInfo.stat.atk}\n" +
            $"MP: {characterInfo.stat.mp}\n" +
            $"Speed: {characterInfo.stat.speed}\n" +
            $"돈: {characterInfo.money}";

        characterInfoText.text = characterInfoStr;
    }
}
[System.Serializable]
public class CharacterInfoGetResponse
{
    public Stat stat;
    public int levelId;
    public int currentExp;
    public int totalExp;
    public long nationId;
    public string nationNm;
    public long jobId;
    public string jobNm;
    public int money;
    public string imageUrl;
}

[System.Serializable]
public class Stat
{
    public int hp;
    public int atk;
    public int mp;
    public int speed;
}
