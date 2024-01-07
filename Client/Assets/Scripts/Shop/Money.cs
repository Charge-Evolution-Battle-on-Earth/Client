using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class Money : MonoBehaviour
{
    public TMP_Text MoneyText;

    void Start()
    {
        StartCoroutine(GetMoneyInfo());
    }

    IEnumerator GetMoneyInfo()
    {
        string url = GameURL.DBServer.Server_URL + GameURL.DBServer.getCharacterInfoPath;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", $"Bearer {UserDataManager.Instance.AccessToken}");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                CharacterMoneyInfoGetResponse characterInfo = JsonUtility.FromJson<CharacterMoneyInfoGetResponse>(jsonResponse);

                DisplayMoneyInfo(characterInfo);
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }
    }

    public void DisplayMoneyInfo(CharacterMoneyInfoGetResponse characterInfo)
    {
        string characterInfoStr =
            $"{characterInfo.money}";

        MoneyText.text = characterInfoStr;
        UserDataManager.Instance.LevelId = characterInfo.levelId;
        UserDataManager.Instance.JobId = characterInfo.jobId;
    }
}
[System.Serializable]
public class CharacterMoneyInfoGetResponse
{
    public long levelId;
    public long jobId;
    public int money;
}