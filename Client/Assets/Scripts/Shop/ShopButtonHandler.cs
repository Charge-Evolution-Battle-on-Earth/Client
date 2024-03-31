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
    public GameObject[] itemPrefabs; // �����յ��� ������ �迭

    private Dictionary<string, string> itemPrefabMap = new Dictionary<string, string>();

    private void Start()
    {
        purchaseButton.interactable = false;
        itemPrefabMap["��������"] = "��������";
        itemPrefabMap["������"] = "������";
        itemPrefabMap["�θ�ī ���׸�ŸŸ"] = "�θ�ī ���׸�ŸŸ";
        itemPrefabMap["���������"] = "���������";
        itemPrefabMap["�����谩"] = "�����谩";
        itemPrefabMap["�̴ð�"] = "�̴ð�";
        itemPrefabMap["�깮��"] = "�깮��";
        itemPrefabMap["����"] = "����";
        itemPrefabMap["����"] = "����";
        itemPrefabMap["ź��"] = "ź��";
        itemPrefabMap["�似�������"] = "�似�������";
        itemPrefabMap["Ǯ �÷���Ʈ �Ƹ�"] = "Ǯ �÷���Ʈ �Ƹ�";
        itemPrefabMap["�ǰ�"] = "�ǰ�";
        itemPrefabMap["û���䰩"] = "û���䰩";
        itemPrefabMap["ũ���䰩"] = "ũ���䰩";
    }

    public void OnButtonClick(string itemType)
    {
        shopItemList = $"/items/{itemType}/{UserDataManager.Instance.LevelId}/{UserDataManager.Instance.JobId}";

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
            /*if (itemPrefabMap.ContainsKey(item.itemNm))
            {*/
                //string prefabName = itemPrefabMap[item.itemNm];

                // ������ �������� �����Ͽ� Content �г��� �ڽ����� �߰�
                GameObject newItem = Instantiate(itemPrefab, contentPanel);
                //GameObject newItem = Instantiate(Resources.Load<GameObject>($"Prefabs/Shop/Armor/{prefabName}"), contentPanel);
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
            //}
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
        public long characterItemId;
        public Image image;
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