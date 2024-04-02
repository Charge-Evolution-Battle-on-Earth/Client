using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;

public class InventoryItemsHandler : MonoBehaviour
{
    public GameObject itemPrefab; // ������ ������
    public Transform contentPanel; // Scroll View�� Content �κ�
    public TMP_Text itemNameText;
    public TMP_Text itemStatText;
    public TMP_Text itemDescriptionText;
    private string invenItemList;
    
    public void OnClickButton(string itemType)//���� ���� ��ư
    {
        invenItemList = $"/items/inven/{itemType}";

        ClearItemList();

        StartCoroutine(GetItems());
    }

    IEnumerator GetItems()
    {
        string url = GameURL.DBServer.Server_URL + invenItemList;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", $"Bearer {UserDataManager.Instance.AccessToken}");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
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
            InventoryItemUIHandler inventoryItemUIHandler = newItem.GetComponent<InventoryItemUIHandler>();

            if (inventoryItemUIHandler != null)
            {
                // ������ ���� ����
                inventoryItemUIHandler.SetItemInfo(item);
            }
            else
            {
                Debug.LogError("�ν��Ͻ�ȭ�� ������ �����տ��� InventoryItemUIHandler ���� ��Ҹ� ã�� �� �����ϴ�.");
            }
        }
    }

    void ClearItemList()
    {
        itemNameText.text = "";
        itemStatText.text = "";
        itemDescriptionText.text = "";


        // �θ� ��ü�� ��� �ڽ� ��ü ����
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }
    }
}
