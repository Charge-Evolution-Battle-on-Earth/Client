using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class GetUserInfo : MonoBehaviour
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
}
[System.Serializable]
public class CharacterMoneyInfoGetResponse
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
    public string nickname;
}