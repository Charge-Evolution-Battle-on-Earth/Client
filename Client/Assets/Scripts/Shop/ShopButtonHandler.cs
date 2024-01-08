using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;

public class ShopButtonHandler : MonoBehaviour
{
    public GameObject itemPrefab; // ������ ������
    public Transform contentPanel; // Scroll View�� Content �κ�
    public TMP_Text itemNameText;
    public TMP_Text itemStatText;
    public TMP_Text itemDescriptionText;
    private string shopItemList;
    public Button purchaseButton;

    private void Start()
    {
        purchaseButton.interactable = false;
    }

    public void OnButtonClick(string itemType)
    {
        shopItemList = $"/items/{itemType}/100/1";//{UserDataManager.Instance.JobId}";

        ClearItemList();

        StartCoroutine(GetItems());
    }


    IEnumerator GetItems()
    {
        string url = GameURL.DBServer.Server_URL + shopItemList;
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", $"Bearer {UserDataManager.Instance.AccessToken}");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                //Debug.Log(jsonResponse);
                List<Shop.ShopItemGetResponse> itemList = JsonConvert.DeserializeObject<List<Shop.ShopItemGetResponse>>(jsonResponse);

                AddItemsToUI(itemList);
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }
    }

    void AddItemsToUI(List<Shop.ShopItemGetResponse> itemList)
    {
        // itemPrefab �� contentPanel�� null�� �ƴ��� Ȯ��
        if (itemPrefab == null || contentPanel == null)
        {
            Debug.LogError("itemPrefab �Ǵ� contentPanel�� null�Դϴ�.");
            return;
        }

        // ���ο� ��� UI ����
        foreach (var item in itemList)
        {
            // itemPrefab�� contentPanel�� null�� �ƴ��� Ȯ���ϱ� ���� ����� ��
            //Debug.Log(item.itemNm + "�� ���� ������ UI ���� ��.");

            // ������ �������� �����Ͽ� Content �г��� �ڽ����� �߰�
            GameObject newItem = Instantiate(itemPrefab, contentPanel);
            ItemUIHandler itemUIHandler = newItem.GetComponent<ItemUIHandler>();

            if (itemUIHandler != null)
            {
                // ������ ���� ����
                itemUIHandler.SetItemInfo(item);
            }
            else
            {
                Debug.LogError("�ν��Ͻ�ȭ�� ������ �����տ��� ItemUIHandler ���� ��Ҹ� ã�� �� �����ϴ�.");
            }
        }
    }

    void ClearItemList()
    {
        itemNameText.text = "";
        itemStatText.text = "";
        itemDescriptionText.text = "";
        purchaseButton.interactable = false;

        // �θ� ��ü�� ��� �ڽ� ��ü ����
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }
    }
}

namespace Shop
{
    [System.Serializable]
    public class ShopItemGetResponse
    {
        public long itemId;
        public long levelId;
        public long jobId;
        public long itemTypeId;
        public string itemNm;
        public int cost;
        public Stat stat; // Stat Ŭ������ Shop ���ӽ����̽� �߰� �� ���
        public string description;
    }

    [System.Serializable]
    public class Stat
    {
        public int hp;
        public int atk;
        public int mp;
        public int spd;
    }
}