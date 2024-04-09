using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class SkillList : MonoBehaviour
{
    public TMP_Text skillListText;
    public PopupManager popupManager;

    private void Start()
    {
        popupManager.HidePopup();
        StartCoroutine(GetSkillInfo());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            popupManager.HidePopup();
        }
    }

    IEnumerator GetSkillInfo()
    {
        string url = GameURL.DBServer.Server_URL + GameURL.DBServer.getCharacterSkillInfoPath;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", $"Bearer {UserDataManager.Instance.AccessToken}");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;

                // JSON 배열을 JArray로 변환
                JArray skillArray = JArray.Parse(jsonResponse);

                // JArray를 List로 변환
                List<CharacterSkillInfoGetResponse> skillList = new List<CharacterSkillInfoGetResponse>();
                foreach (JToken skillToken in skillArray)
                {
                    CharacterSkillInfoGetResponse skillInfo = skillToken.ToObject<CharacterSkillInfoGetResponse>();
                    skillList.Add(skillInfo);
                }
                string skillListStr = "[스킬 목록]\n";

                foreach (var skill in skillList)
                {
                    skillListStr += $"{skill.skillNm} - {skill.description}\n";
                }

                if (skillListText != null)
                {
                    skillListText.text = skillListStr;
                }
                else
                {
                    Debug.LogError("TMP_Text component not found on CharacterSkillInfoDisplay.");
                }
            }
            else
            {
                popupManager.ShowPopup("Error: " + www.error);
            }
        }
    }
}
[System.Serializable]
public class CharacterSkillInfoGetResponse
{
    public long skillId;
    public string skillNm;
    public string description;
}