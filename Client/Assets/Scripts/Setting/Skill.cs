using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Skill : MonoBehaviour
{
    public PopupManager popupManager;
    public Transform tableContent;
    public GameObject rowPrefab;

    void Start()
    {
        popupManager.HidePopup();
        DataManager.Instance.SkillGetListResponse.Clear();
        StartCoroutine(FetchSkillList());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            popupManager.HidePopup();
        }
    }

    public void RefreshBtn()
    {
        StartCoroutine(FetchSkillList());
    }

    public void SaveSkillBtn(Transform row)
    {
        SkillPost skill = new SkillPost();
        skill.skillId = int.Parse(row.Find("SkillID").GetComponent<InputField>().text);
        skill.skillEffectId = int.Parse(row.Find("SkillEffectID").GetComponent<InputField>().text);
        skill.fixedValue = int.Parse(row.Find("FixedValue").GetComponent<InputField>().text);
        skill.hpRate = int.Parse(row.Find("HPRate").GetComponent<InputField>().text);
        skill.atkRate = int.Parse(row.Find("ATKRate").GetComponent<InputField>().text);
        skill.mpRate = int.Parse(row.Find("MPRate").GetComponent<InputField>().text);
        skill.spdRate = int.Parse(row.Find("SPDRate").GetComponent<InputField>().text);

        StartCoroutine(SendSkillData(skill));
    }

    IEnumerator FetchSkillList()
    {
        string url = GameURL.DBServer.Server_URL; // 뒷 주소 추가 필요
        
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", $"Bearer {UserDataManager.Instance.AccessToken}");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                List<SkillGetListResponse> skillListResponse = JsonConvert.DeserializeObject<List<SkillGetListResponse>>(jsonResponse);
                UpdateTable(skillListResponse);
            }
            else
            {
                popupManager.ShowPopup("Error: " + www.error + www.downloadHandler.text);
            }
        }
    }
    
    IEnumerator SendSkillData(SkillPost skill)
    {
        JObject skillData = new JObject();
        skillData["skillId"] = skill.skillId;
        skillData["skillEffectId"] = skill.skillEffectId;
        skillData["fixedValue"] = skill.fixedValue;
        skillData["hpRate"] = skill.hpRate;
        skillData["atkRate"] = skill.atkRate;
        skillData["mpRate"] = skill.mpRate;
        skillData["spdRate"] = skill.spdRate;

        string jsonData = skillData.ToString();
        string url = GameURL.DBServer.Server_URL; // 뒷 주소 추가 필요
        
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.SetRequestHeader("Authorization", $"Bearer {UserDataManager.Instance.AccessToken}");

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                popupManager.ShowPopup("저장 성공");
            }
            else
            {
                popupManager.ShowPopup("Error: " + www.error + www.downloadHandler.text);
            }
        }
    }

    void UpdateTable(List<SkillGetListResponse> skills)
    {
        foreach (SkillGetListResponse skill in skills)
        {
            GameObject row = Instantiate(rowPrefab, tableContent);
            row.transform.Find("SkillID").GetComponent<InputField>().text = skill.skillId.ToString();
            row.transform.Find("SkillName").GetComponent<InputField>().text = skill.skillNm;
            row.transform.Find("SkillEffectID").GetComponent<InputField>().text = skill.skillEffectId.ToString();
            row.transform.Find("FixedValue").GetComponent<InputField>().text = skill.fixedValue.ToString();
            row.transform.Find("HPRate").GetComponent<InputField>().text = skill.hpRate.ToString();
            row.transform.Find("ATKRate").GetComponent<InputField>().text = skill.atkRate.ToString();
            row.transform.Find("MPRate").GetComponent<InputField>().text = skill.mpRate.ToString();
            row.transform.Find("SPDRate").GetComponent<InputField>().text = skill.spdRate.ToString();
        }
    }

}