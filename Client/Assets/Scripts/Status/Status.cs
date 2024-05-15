using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class Status : MonoBehaviour
{
    public TMP_Text characterInfoText;
    public TMP_Text characterNickNameText;
    public TMP_Text expText;
    public Image characterImage;
    public Image expBar;
    public PopupManager popupManager;

    void Start()
    {
        popupManager.HidePopup();
        StartCoroutine(GetCharacterInfo());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            popupManager.HidePopup();
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

                DisplayCharacterInfo(characterInfo);
            } 
            else
            {
                popupManager.ShowPopup("Error: " + www.error);
            }
        }
    }

    void DisplayCharacterInfo(CharacterInfoGetResponse characterInfo)
    {
        string characterInfoStr =
            $"캐릭터 정보\n" +
            $"레벨: {characterInfo.levelId}\n" +
            $"나라: {characterInfo.nationNm}\n" +
            $"직업: {characterInfo.jobNm}\n" +
            $"Hp: {characterInfo.stat.hp}\n" +
            $"Mp: {characterInfo.stat.mp}\n" +
            $"Atk: {characterInfo.stat.atk}\n" +
            $"Spd: {characterInfo.stat.spd}\n" +
            $"돈: {characterInfo.money}";

        characterInfoText.text = characterInfoStr;
        characterNickNameText.text = characterInfo.nickname;

        expText.text = characterInfo.currentExp + " / " + characterInfo.totalExp;
        expBar.fillAmount = (float)characterInfo.currentExp / (float)characterInfo.totalExp;

        SaveUserData(characterInfo);

        Sprite sprite = Resources.Load<Sprite>($"Prefabs/Choice/{UserDataManager.Instance.JobNm}");
        if(sprite!=null)
        {
            characterImage.sprite = sprite;
        }
        else
        {
            popupManager.ShowPopup("직업 이미지를 찾을 수 없습니다: " + UserDataManager.Instance.JobNm);
        }
    }

    void SaveUserData(CharacterInfoGetResponse characterInfo)
    {
        UserDataManager.Instance.Stat = characterInfo.stat;
        UserDataManager.Instance.LevelId = characterInfo.levelId;
        UserDataManager.Instance.CurrentExp = characterInfo.currentExp;
        UserDataManager.Instance.TotalExp = characterInfo.totalExp;
        UserDataManager.Instance.NationId = characterInfo.nationId;
        UserDataManager.Instance.NationNm = characterInfo.nationNm;
        UserDataManager.Instance.JobId = characterInfo.jobId;
        UserDataManager.Instance.JobNm = characterInfo.jobNm;
        UserDataManager.Instance.Money = characterInfo.money;
        UserDataManager.Instance.NickName = characterInfo.nickname;
        UserDataManager.Instance.CharacterId = characterInfo.characterId;
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
    public string nickname;
    public long characterId;
}

[System.Serializable]
public class Stat
{
    public int hp;
    public int atk;
    public int mp;
    public int spd;
}
