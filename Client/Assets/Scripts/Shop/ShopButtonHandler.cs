using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;

public class ShopButtonHandler : MonoBehaviour
{
    public Image itemImage;
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
        ImageTransparency();
        purchaseButton.interactable = false;
        // ����
        itemPrefabMap["��������"] = "��������"; itemPrefabMap["�ǰ�"] = "�ǰ�"; itemPrefabMap["����"] = "����";
        itemPrefabMap["ũ���䰩"] = "ũ���䰩"; itemPrefabMap["���������"] = "���������"; itemPrefabMap["û������"] = "û������";
        itemPrefabMap["�����谩"] = "�����谩"; itemPrefabMap["������ �Ƹ�"] = "������ �Ƹ�"; itemPrefabMap["ź��"] = "ź��";
        itemPrefabMap["����"] = "����"; itemPrefabMap["�깮��"] = "�깮��"; itemPrefabMap["�θ�ī ���׸�ŸŸ"] = "�θ�ī ���׸�ŸŸ";
        itemPrefabMap["������"] = "������"; itemPrefabMap["�似�̱�����"] = "�似�̱�����"; itemPrefabMap["Ǯ �÷���Ʈ�Ƹ�"] = "Ǯ �÷���Ʈ�Ƹ�";

        // ���� ����
        itemPrefabMap["��â"] = "��â"; itemPrefabMap["����â"] = "����â"; itemPrefabMap["�÷�������̽�"] = "�÷�������̽�";
        itemPrefabMap["���"] = "���"; itemPrefabMap["��â"] = "��â"; itemPrefabMap["�丶ȣũ"] = "�丶ȣũ";
        itemPrefabMap["������"] = "������"; itemPrefabMap["�ݼ��"] = "�ݼ��"; itemPrefabMap["������ ��õȭ��"] = "������ ��õȭ��";
        itemPrefabMap["īŸ��"] = "īŸ��"; itemPrefabMap["������ û������"] = "������ û������"; itemPrefabMap["�ҹ���"] = "�ҹ���";
        itemPrefabMap["�湫���� ���"] = "�湫���� ���"; itemPrefabMap["�۶��콺"] = "�۶��콺"; itemPrefabMap["���ȣ���ڿ�"] = "���ȣ���ڿ�";

        // �ü� ����
        itemPrefabMap["�ٶ���"] = "�ٶ���"; itemPrefabMap["����"] = "����"; itemPrefabMap["������"] = "������";
        itemPrefabMap["ī�̻縣�� �ʷ�"] = "ī�̻縣�� �ʷ�"; itemPrefabMap["��ť"] = "��ť"; itemPrefabMap["�񰢱�"] = "�񰢱�";
        itemPrefabMap["���"] = "���"; itemPrefabMap["��������"] = "��������"; itemPrefabMap["�ױ�"] = "�ױ�";
        itemPrefabMap["ȭ����"] = "ȭ����"; itemPrefabMap["�������� ����"] = "�������� ����"; itemPrefabMap["�氢��"] = "�氢��";
        itemPrefabMap["������ �ø�Ʈ���ӽ�Ŷ"] = "������ �ø�Ʈ���ӽ�Ŷ"; itemPrefabMap["��������"] = "��������"; itemPrefabMap["������ ��ñ�"] = "������ ��ñ�";

        // ������ ����
        itemPrefabMap["û�����"] = "û�����"; itemPrefabMap["���ΰ�"] = "���ΰ�"; itemPrefabMap["�������� ����"] = "�������� ����";
        itemPrefabMap["û����"] = "û����"; itemPrefabMap["�ձ⴩���� â"] = "�ձ⴩���� â"; itemPrefabMap["�������庸��"] = "�������庸��";
        itemPrefabMap["�߷��þ��� ����"] = "�߷��þ��� ����"; itemPrefabMap["����"] = "����"; itemPrefabMap["�����ڰ�"] = "�����ڰ�";
        itemPrefabMap["���ʰ���� ĥ����"] = "���ʰ���� ĥ����"; itemPrefabMap["�ݵ������"] = "�ݵ������"; itemPrefabMap["�ܴٸ�ũ�� ����"] = "�ܴٸ�ũ�� ����";
        itemPrefabMap["���ع���"] = "���ع���"; itemPrefabMap["����������ٶ�ϰ�"] = "����������ٶ�ϰ�"; itemPrefabMap["���"] = "���";

        OnButtonClick("1");
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
        foreach (var item in itemList)
        {
            if (itemPrefabMap.ContainsKey(item.itemNm))
            {
                string prefabName = itemPrefabMap[item.itemNm];

                // ������ �������� �����Ͽ� Content �г��� �ڽ����� �߰�
                GameObject newItem = Instantiate(itemPrefabs.FirstOrDefault(item => item.name == prefabName), contentPanel);

                ShopItemUIHandler itemUIHandler = newItem.GetComponent<ShopItemUIHandler>();

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
    }

    void ClearItemList()
    {
        ImageTransparency();
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
    
    void ImageTransparency()
    {
        Color color = new Color();
        color.a = 0f;
        itemImage.color = color;
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