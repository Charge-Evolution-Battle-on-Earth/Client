using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;

public class Item : MonoBehaviour
{
    public PopupManager popupManager;
    public Transform tableContent;
    public GameObject rowPrefab;

    void Start()
    {
        popupManager.HidePopup();
        DataManager.Instance.itemGetResponse.Clear();
        RefreshBtn();
    }

    void Update()
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

    IEnumerator FetchSkillList()
    {
        string url = GameURL.DBServer.Server_URL + GameURL.DBServer.getItemsListPath;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", $"Bearer {UserDataManager.Instance.AccessToken}");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                List<ItemGetResponse> itemListResponse = JsonConvert.DeserializeObject<List<ItemGetResponse>>(jsonResponse);
                DataManager.Instance.itemGetResponse = itemListResponse;
                UpdateTable(itemListResponse);
            }
            else
            {
                popupManager.ShowPopup("Error: " + www.error + www.downloadHandler.text);
            }
        }
    }

    public void SaveSkillBtn(Transform row)
    {
        ItemPost item = new ItemPost();
        item.itemId = int.Parse(row.Find("ItemId").GetComponent<TMP_InputField>().text);
        item.cost = int.Parse(row.Find("Cost").GetComponent<TMP_InputField>().text);
        item.stat.hp = int.Parse(row.Find("HP").GetComponent<TMP_InputField>().text);
        item.stat.mp = int.Parse(row.Find("MP").GetComponent<TMP_InputField>().text);
        item.stat.atk = int.Parse(row.Find("ATK").GetComponent<TMP_InputField>().text);
        item.stat.spd = int.Parse(row.Find("SPD").GetComponent<TMP_InputField>().text);

        StartCoroutine(SendItemData(item));
    }


    IEnumerator SendItemData(ItemPost item)
    {
        string jsonData = JsonUtility.ToJson(item);
        string url = GameURL.DBServer.Server_URL + GameURL.DBServer.putItemPath;

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

    void UpdateTable(List<ItemGetResponse> items)
    {
        foreach (ItemGetResponse item in items)
        {
            GameObject row = Instantiate(rowPrefab, tableContent);
            row.transform.Find("ItemId").GetComponent<TMP_InputField>().text = item.itemId.ToString();
            row.transform.Find("ItemTypeId").GetComponent<TMP_InputField>().text = item.itemTypeId.ToString();
            row.transform.Find("JobId").GetComponent<TMP_InputField>().text = item.jobId.ToString();
            row.transform.Find("ItemNm").GetComponent<TMP_InputField>().text = item.itemNm.ToString();
            row.transform.Find("LevelId").GetComponent<TMP_InputField>().text = item.levelId.ToString();
            row.transform.Find("Cost").GetComponent<TMP_InputField>().text = item.cost.ToString();
            row.transform.Find("HP").GetComponent<TMP_InputField>().text = item.stat.hp.ToString();
            row.transform.Find("ATK").GetComponent<TMP_InputField>().text = item.stat.atk.ToString();
            row.transform.Find("MP").GetComponent<TMP_InputField>().text = item.stat.mp.ToString();
            row.transform.Find("SPD").GetComponent<TMP_InputField>().text = item.stat.spd.ToString();
        }
    }
}
