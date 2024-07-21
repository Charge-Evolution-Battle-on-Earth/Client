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

    private string defaultSortStatus = "itemTypeIdASC_jobIdASC_levelIdASC"; // 기본 정렬 상태

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
        DataManager.Instance.SortStatus = "";
        StartCoroutine(FetchItemList());
    }

    IEnumerator FetchItemList()
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

                UpdateTable(DataManager.Instance.itemGetResponse, defaultSortStatus);
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

    void UpdateTable(List<ItemGetResponse> items, string sortBy)
    {
        foreach (Transform child in tableContent)
        {
            Destroy(child.gameObject);
        }

        if (sortBy == defaultSortStatus)
        {
            DataManager.Instance.SortStatus = "itemTypeIdDESC";
            items.Sort((x, y) =>
            {
                int comparison = x.itemTypeId.CompareTo(y.itemTypeId);
                if (comparison == 0) comparison = x.jobId.CompareTo(y.jobId);
                if (comparison == 0) comparison = x.levelId.CompareTo(y.levelId);
                return comparison;
            });
        }
        else
        {
            bool ascending = DataManager.Instance.SortStatus != sortBy + "DESC";

            items.Sort((x, y) =>
            {
                switch (sortBy)
                {
                    case "itemId":
                        return ascending ? x.itemId.CompareTo(y.itemId) : y.itemId.CompareTo(x.itemId);
                    case "itemNm":
                        return ascending ? string.Compare(x.itemNm, y.itemNm) : string.Compare(y.itemNm, x.itemNm);
                    case "itemTypeId":
                        return ascending ? x.itemTypeId.CompareTo(y.itemTypeId) : y.itemTypeId.CompareTo(x.itemTypeId);
                    case "jobId":
                        return ascending ? x.jobId.CompareTo(y.jobId) : y.jobId.CompareTo(x.jobId);
                    case "levelId":
                        return ascending ? x.levelId.CompareTo(y.levelId) : y.levelId.CompareTo(x.levelId);
                    case "cost":
                        return ascending ? x.cost.CompareTo(y.cost) : y.cost.CompareTo(x.cost);
                    case "hp":
                        return ascending ? x.stat.hp.CompareTo(y.stat.hp) : y.stat.hp.CompareTo(x.stat.hp);
                    case "mp":
                        return ascending ? x.stat.mp.CompareTo(y.stat.mp) : y.stat.mp.CompareTo(x.stat.mp);
                    case "atk":
                        return ascending ? x.stat.atk.CompareTo(y.stat.atk) : y.stat.atk.CompareTo(x.stat.atk);
                    case "spd":
                        return ascending ? x.stat.spd.CompareTo(y.stat.spd) : y.stat.spd.CompareTo(x.stat.spd);
                    default:
                        return 0;
                }
            });

            DataManager.Instance.SortStatus = ascending ? sortBy + "DESC" : sortBy + "ASC";
        }

        foreach (ItemGetResponse item in items)
        {
            GameObject row = Instantiate(rowPrefab, tableContent);
            row.transform.Find("ItemId").GetComponent<TMP_InputField>().text = item.itemId.ToString();
            if (item.itemTypeId == 1)
            {
                row.transform.Find("ItemTypeId").GetComponent<TMP_InputField>().text = "무기";
            }
            else if (item.itemTypeId == 2)
            {
                row.transform.Find("ItemTypeId").GetComponent<TMP_InputField>().text = "방어구";
            }
            if (item.jobId == 1)
            {
                row.transform.Find("JobId").GetComponent<TMP_InputField>().text = "전사";
            }
            else if (item.jobId == 2)
            {
                row.transform.Find("JobId").GetComponent<TMP_InputField>().text = "궁수";
            }
            else if (item.jobId == 3)
            {
                row.transform.Find("JobId").GetComponent<TMP_InputField>().text = "성직자";
            }
            row.transform.Find("ItemNm").GetComponent<TMP_InputField>().text = item.itemNm.ToString();
            row.transform.Find("LevelId").GetComponent<TMP_InputField>().text = item.levelId.ToString();
            row.transform.Find("Cost").GetComponent<TMP_InputField>().text = item.cost.ToString();
            row.transform.Find("HP").GetComponent<TMP_InputField>().text = item.stat.hp.ToString();
            row.transform.Find("ATK").GetComponent<TMP_InputField>().text = item.stat.atk.ToString();
            row.transform.Find("MP").GetComponent<TMP_InputField>().text = item.stat.mp.ToString();
            row.transform.Find("SPD").GetComponent<TMP_InputField>().text = item.stat.spd.ToString();
        }
    }

    public void SortItemTable(string sortBy)
    {
        UpdateTable(DataManager.Instance.itemGetResponse, sortBy);
    }
}