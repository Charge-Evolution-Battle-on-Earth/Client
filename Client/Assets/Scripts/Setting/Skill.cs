using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using System;

public class Skill : MonoBehaviour
{
    public PopupManager popupManager;
    public Transform tableContent;
    public GameObject rowPrefab;
    public TMP_InputField searchInput;

    void Start()
    {
        popupManager.HidePopup();
        DataManager.Instance.SkillGetListResponse.Clear();
        RefreshBtn();
        searchInput.onValueChanged.AddListener(OnSearchInputChanged);
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
        DataManager.Instance.SortStatus = "";
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

                UpdateTable(DataManager.Instance.SkillGetListResponse, "skillEffectId");
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

    void UpdateTable(List<SkillGetListResponse> skills, string sortBy)
    {
        foreach (Transform child in tableContent)
        {
            Destroy(child.gameObject);
        }

        bool ascending = DataManager.Instance.SortStatus != sortBy + "DESC";

        skills.Sort((x, y) =>
        {
            switch (sortBy)
            {
                case "skillId":
                    return ascending ? x.skillId.CompareTo(y.skillId) : y.skillId.CompareTo(x.skillId);
                case "skillNm":
                    return ascending ? string.Compare(x.skillNm, y.skillNm) : string.Compare(y.skillNm, x.skillNm);
                case "skillEffectId":
                    return ascending ? x.skillEffectId.CompareTo(y.skillEffectId) : y.skillEffectId.CompareTo(x.skillEffectId);
                case "skillEffectType":
                    return ascending ? x.skillEffectType.CompareTo(y.skillEffectType) : y.skillEffectType.CompareTo(x.skillEffectType);
                case "fixedValue":
                    return ascending ? x.fixedValue.CompareTo(y.fixedValue) : y.fixedValue.CompareTo(x.fixedValue);
                case "hpRate":
                    return ascending ? x.statRate.hpRate.CompareTo(y.statRate.hpRate) : y.statRate.hpRate.CompareTo(x.statRate.hpRate);
                case "atkRate":
                    return ascending ? x.statRate.atkRate.CompareTo(y.statRate.atkRate) : y.statRate.atkRate.CompareTo(x.statRate.atkRate);
                case "mpRate":
                    return ascending ? x.statRate.mpRate.CompareTo(y.statRate.mpRate) : y.statRate.mpRate.CompareTo(x.statRate.mpRate);
                case "spdRate":
                    return ascending ? x.statRate.spdRate.CompareTo(y.statRate.spdRate) : y.statRate.spdRate.CompareTo(x.statRate.spdRate);
                default:
                    return 0;
            }
        });

        DataManager.Instance.SortStatus = ascending ? sortBy + "DESC" : sortBy + "ASC";

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

    public void SortSkillTable(string sortBy)
    {
        UpdateTable(DataManager.Instance.SkillGetListResponse, sortBy);
    }

    void OnSearchInputChanged(string searchQuery)
    {
        string lowerCaseQuery = searchQuery.ToLower();
        List<SkillGetListResponse> filteredSkills = DataManager.Instance.SkillGetListResponse.FindAll(skill =>
            skill.skillNm.ToLower().Contains(lowerCaseQuery));

        UpdateTable(filteredSkills, "skillEffectId");
    }
}