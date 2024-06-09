using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class InventoryItemsHandler : MonoBehaviour
{
    public Image itemImage;
    public GameObject itemPrefab; // ������ ������
    public Transform contentPanel; // Scroll View�� Content �κ�
    public TMP_Text itemNameText;
    public TMP_Text itemStatText;
    public TMP_Text itemDescriptionText;
    public GameObject[] itemPrefabs;
    private Dictionary<string, string> itemPrefabMap = new Dictionary<string, string>();

    public Button equipBtn;
    public Button sellBtn;
    public Button unequipBtn;
    public PopupManager popupManager;
    public ServerManager serverManager;

    private void Start()
    {
        popupManager.HidePopup();
        contentPanel = GameObject.Find("Content").transform;
        GridLayoutGroup gridLayout = contentPanel.GetComponent<GridLayoutGroup>();
        if (gridLayout != null)
        {
            gridLayout.padding.left = 10; // ���� ���� ����
            gridLayout.padding.right = 10; // ���� ���� ����
            gridLayout.padding.top = 10; // ��� ���� ����
            gridLayout.padding.bottom = 10; // �ϴ� ���� ����
        }
        ClearItemList();

        InitializeItemPrefabMap();

        OnClickButton("1");
        UserDataManager.Instance.ItemTypeId = 1;
    }

    private void Update()
    {
        if (UserDataManager.Instance.ClearUI)
        {
            ClearItemList();
            if (UserDataManager.Instance.ItemTypeId == 1)
            {
                AddItemsToUI(UserDataManager.Instance.WeaponItemList);
            }
            else if (UserDataManager.Instance.ItemTypeId == 2)
            {
                AddItemsToUI(UserDataManager.Instance.ArmorItemList);
            }
            UserDataManager.Instance.ClearUI = false;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            popupManager.HidePopup();
        }
    }
    public void OnClickButton(string itemType)//���� ���� ��ư
    {

        UserDataManager.Instance.ItemTypeId = long.Parse(itemType);
        string invenItemList = $"/items/inven/{itemType}";
        string invenEquippedItem = $"/items/inven/equipped-item/{itemType}";

        serverManager.SendRequest(GameURL.DBServer.Server_URL + invenEquippedItem, ServerManager.SendType.GET, "", OnResponseReceived);

        ClearItemList();

        if (itemType == "1")
        {
            UserDataManager.Instance.ItemTypeId = 1;
            if (UserDataManager.Instance.WeaponItemList.Count == 0)
            {
                StartCoroutine(GetItems(invenItemList, itemType));
            }
            else
            {
                AddItemsToUI(UserDataManager.Instance.WeaponItemList);
            }
        }
        else if (itemType == "2")
        {
            UserDataManager.Instance.ItemTypeId = 2;
            if (UserDataManager.Instance.ArmorItemList.Count == 0)
            {
                StartCoroutine(GetItems(invenItemList, itemType));
            }
            else
            {
                AddItemsToUI(UserDataManager.Instance.ArmorItemList);
            }
        }
    }

    IEnumerator GetItems(string invenItemList, string itemType)
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
                if (itemType == "1")
                {
                    UserDataManager.Instance.WeaponItemList = itemList;
                }
                else if (itemType == "2")
                {
                    UserDataManager.Instance.ArmorItemList = itemList;
                }
                AddItemsToUI(itemList);
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                JObject json = JObject.Parse(jsonResponse);

                string errorType = json["type"].ToString();
                string errorMessage = json["message"].ToString();
                string error = errorType + ": " + errorMessage;
                popupManager.ShowPopup(error);
            }
        }
    }

    void AddItemsToUI(List<Shop.ShopItemGetResponse> itemList)
    {
        foreach (var item in itemList)
        {
            if (itemPrefabMap.ContainsKey(item.itemNm))
            {
                string prefabName = itemPrefabMap[item.itemNm];


                // ������ �������� �����Ͽ� Content �г��� �ڽ����� �߰�
                GameObject newItem = Instantiate(itemPrefabs.FirstOrDefault(item => item.name == prefabName), contentPanel);
                InventoryItemUIHandler inventoryItemUIHandler = newItem.GetComponent<InventoryItemUIHandler>();

                if (inventoryItemUIHandler != null)
                {
                    // ������ ���� ����
                    inventoryItemUIHandler.SetItemInfo(item);

                    // �׵θ� �߰� �� ����
                    Outline outline = newItem.GetComponent<Outline>();
                    if (outline == null)
                    {
                        outline = newItem.AddComponent<Outline>();
                    }

                    if (item.characterItemId == UserDataManager.Instance.EquippedItemId)
                    {
                        // ������ ������ ����: �׵θ� ������ ��������� ����
                        outline.effectColor = Color.red;
                        outline.effectDistance = new Vector2(5, 5); // �׵θ� �β� ����
                    }
                    else
                    {
                        // �������� ���� ������: �׵θ� ������ �����ϰ� ����
                        outline.effectColor = Color.clear;
                    }
                }
                else
                {
                    popupManager.ShowPopup("�ν��Ͻ�ȭ�� ������ �����տ��� InventoryItemUIHandler ���� ��Ҹ� ã�� �� �����ϴ�.");
                }
            }
        }
    }

    void ClearItemList()
    {
        ImageTransparency();
        itemNameText.text = "";
        itemStatText.text = "";
        itemDescriptionText.text = "";
        equipBtn.interactable = false;
        sellBtn.interactable = false;
        unequipBtn.interactable = false;

        // �θ� ��ü�� ��� �ڽ� ��ü ����
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }
    }

    void ImageTransparency()
    {
        Color color = new Color();
        color.a = 0f;
        itemImage.color = color;
    }

    public void ClearInventoryList()
    {
        UserDataManager.Instance.WeaponItemList.Clear();
        UserDataManager.Instance.ArmorItemList.Clear();
    }

    void OnResponseReceived(string jsonResponse)
    {
        if (!string.IsNullOrEmpty(jsonResponse))
        {
            JObject json = JObject.Parse(jsonResponse);
            var equippedItemIdToken = json["equippedItemId"];
            if (equippedItemIdToken != null && equippedItemIdToken.Type == JTokenType.Integer)
            {
                long equippedItemId = equippedItemIdToken.Value<long>();
                UserDataManager.Instance.EquippedItemId = equippedItemId;
            }
            else
            {
                Debug.LogError("�������� ������ ����");
            }
        }
        else
        {
            Debug.LogError("��Ʈ��ũ ���� ����");
        }
    }

    private void InitializeItemPrefabMap()
    {
        // ������ �� �ʱ�ȭ �ڵ�
        itemPrefabMap["��������"] = "��������";
        itemPrefabMap["�ǰ�"] = "�ǰ�";
        itemPrefabMap["����"] = "����";
        itemPrefabMap["ũ���䰩"] = "ũ���䰩";
        itemPrefabMap["���������"] = "���������";
        itemPrefabMap["û������"] = "û������";
        itemPrefabMap["�����谩"] = "�����谩";
        itemPrefabMap["������ �Ƹ�"] = "������ �Ƹ�";
        itemPrefabMap["ź��"] = "ź��";
        itemPrefabMap["����"] = "����";
        itemPrefabMap["�깮��"] = "�깮��";
        itemPrefabMap["�θ�ī ���׸�ŸŸ"] = "�θ�ī ���׸�ŸŸ";
        itemPrefabMap["������"] = "������";
        itemPrefabMap["�似�̱�����"] = "�似�̱�����";
        itemPrefabMap["Ǯ �÷���Ʈ�Ƹ�"] = "Ǯ �÷���Ʈ�Ƹ�";

        // ���� ����
        itemPrefabMap["��â"] = "��â";
        itemPrefabMap["����â"] = "����â";
        itemPrefabMap["�÷�������̽�"] = "�÷�������̽�";
        itemPrefabMap["���"] = "���";
        itemPrefabMap["��â"] = "��â";
        itemPrefabMap["�丶ȣũ"] = "�丶ȣũ";
        itemPrefabMap["������"] = "������";
        itemPrefabMap["�ݼ��"] = "�ݼ��";
        itemPrefabMap["������ ��õȭ��"] = "������ ��õȭ��";
        itemPrefabMap["īŸ��"] = "īŸ��";
        itemPrefabMap["������ û������"] = "������ û������";
        itemPrefabMap["�ҹ���"] = "�ҹ���";
        itemPrefabMap["�湫���� ���"] = "�湫���� ���";
        itemPrefabMap["�۶��콺"] = "�۶��콺";
        itemPrefabMap["���ȣ���ڿ�"] = "���ȣ���ڿ�";

        // �ü� ����
        itemPrefabMap["�ٶ���"] = "�ٶ���";
        itemPrefabMap["����"] = "����";
        itemPrefabMap["������"] = "������";
        itemPrefabMap["ī�̻縣�� �ʷ�"] = "ī�̻縣�� �ʷ�";
        itemPrefabMap["��ť"] = "��ť";
        itemPrefabMap["�񰢱�"] = "�񰢱�";
        itemPrefabMap["���"] = "���";
        itemPrefabMap["��������"] = "��������";
        itemPrefabMap["�ױ�"] = "�ױ�";
        itemPrefabMap["ȭ����"] = "ȭ����";
        itemPrefabMap["�������� ����"] = "�������� ����";
        itemPrefabMap["�氢��"] = "�氢��";
        itemPrefabMap["������ �ø�Ʈ���ӽ�Ŷ"] = "������ �ø�Ʈ���ӽ�Ŷ";
        itemPrefabMap["��������"] = "��������";
        itemPrefabMap["������ ��ñ�"] = "������ ��ñ�";

        // ������ ����
        itemPrefabMap["û�����"] = "û�����";
        itemPrefabMap["���ΰ�"] = "���ΰ�";
        itemPrefabMap["�������� ����"] = "�������� ����";
        itemPrefabMap["û����"] = "û����";
        itemPrefabMap["�ձ⴩���� â"] = "�ձ⴩���� â";
        itemPrefabMap["�������庸��"] = "�������庸��";
        itemPrefabMap["�߷��þ��� ����"] = "�߷��þ��� ����";
        itemPrefabMap["����"] = "����";
        itemPrefabMap["�����ڰ�"] = "�����ڰ�";
        itemPrefabMap["���ʰ���� ĥ����"] = "���ʰ���� ĥ����";
        itemPrefabMap["�ݵ������"] = "�ݵ������";
        itemPrefabMap["�ܴٸ�ũ�� ����"] = "�ܴٸ�ũ�� ����";
        itemPrefabMap["���ع���"] = "���ع���";
        itemPrefabMap["����������ٶ�ϰ�"] = "����������ٶ�ϰ�";
        itemPrefabMap["���"] = "���";
    }
}