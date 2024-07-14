using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;

public class Skill : MonoBehaviour
{
    public PopupManager popupManager;
    public Transform tableContent;
    public GameObject rowPrefab;

    void Start()
    {
        popupManager.HidePopup();
        DataManager.Instance.SkillGetListResponse.Clear();
        RefreshBtn();
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
        foreach (Transform child in tableContent)
        {
            Destroy(child.gameObject);
        }
        StartCoroutine(FetchSkillList());
    }

    public void SaveSkillBtn(Transform row)
    {
        SkillPost skill = new SkillPost();
        skill.skillEffectId = int.Parse(row.Find("SkillEffectID").GetComponent<TMP_InputField>().text);
        skill.fixedValue = int.Parse(row.Find("FixedValue").GetComponent<TMP_InputField>().text);
        skill.statRate.hpRate = int.Parse(row.Find("HPRate").GetComponent<TMP_InputField>().text);
        skill.statRate.atkRate = int.Parse(row.Find("ATKRate").GetComponent<TMP_InputField>().text);
        skill.statRate.mpRate = int.Parse(row.Find("MPRate").GetComponent<TMP_InputField>().text);
        skill.statRate.spdRate = int.Parse(row.Find("SPDRate").GetComponent<TMP_InputField>().text);

        StartCoroutine(SendSkillData(skill));
    }

    IEnumerator FetchSkillList()
    {
        string url = GameURL.DBServer.Server_URL + GameURL.DBServer.getSkillEffectsListPath;
        
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", $"Bearer {UserDataManager.Instance.AccessToken}");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                List<SkillGetListResponse> skillListResponse = JsonConvert.DeserializeObject<List<SkillGetListResponse>>(jsonResponse);
                DataManager.Instance.SkillGetListResponse = skillListResponse;
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
        string jsonData = JsonUtility.ToJson(skill);
        string url = GameURL.DBServer.Server_URL + GameURL.DBServer.putSkillEffectPath;
        
        using (UnityWebRequest www = new UnityWebRequest(url, "PUT"))
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
            row.transform.Find("SkillEffectType").GetComponent<TMP_InputField>().text = skill.skillEffectType.ToString();
            row.transform.Find("SkillName").GetComponent<TMP_InputField>().text = skill.skillNm;
            row.transform.Find("SkillEffectID").GetComponent<TMP_InputField>().text = skill.skillEffectId.ToString();
            row.transform.Find("FixedValue").GetComponent<TMP_InputField>().text = skill.fixedValue.ToString();
            row.transform.Find("HPRate").GetComponent<TMP_InputField>().text = skill.statRate.hpRate.ToString();
            row.transform.Find("ATKRate").GetComponent<TMP_InputField>().text = skill.statRate.atkRate.ToString();
            row.transform.Find("MPRate").GetComponent<TMP_InputField>().text = skill.statRate.mpRate.ToString();
            row.transform.Find("SPDRate").GetComponent<TMP_InputField>().text = skill.statRate.spdRate.ToString();
        }
    }
}