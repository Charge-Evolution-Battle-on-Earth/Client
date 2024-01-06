using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class CharacterInfoDisplay : MonoBehaviour
{
    public TMP_Text characterInfoText;

    public void GetCharacterInfo()
    {
        StartCoroutine(GetCharacterInfoCoroutine());
    }

    IEnumerator GetCharacterInfoCoroutine()
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
        // 캐릭터 정보를 이용하여 UI 업데이트 등의 작업 수행
        string characterInfoStr =
            $"캐릭터 정보:\n" +
            $"나라: {characterInfo.nationNm}\n" +
            $"직업: {characterInfo.jobNm}\n" +
            $"HP: {characterInfo.stat.hp}\n" +
            $"ATK: {characterInfo.stat.atk}\n" +
            $"MP: {characterInfo.stat.mp}\n" +
            $"Speed: {characterInfo.stat.speed}";
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
