using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections.Generic;
using System.Collections;

namespace Shop
{
    [System.Serializable]
    public class Stat
    {
        public int hp;
        public int atk;
        public int mp;
        public int spd;
    }
}
namespace Shop
{
    public class ShopButtonHandler : MonoBehaviour
    {
        public GameObject itemPrefab; // Unity Inspector���� ������ ������ ������
        public Transform itemListParent; // Unity Inspector���� ������ ������ ����Ʈ�� �θ� ������Ʈ
        private string shopItemList;
        private long characterLevel;
        private long characterJob;

        private void Awake()
        {
            // Awake �޼��忡�� �ʱ�ȭ
            characterLevel = UserDataManager.Instance.LevelId;
            characterJob = UserDataManager.Instance.JobId;
        }
        public void OnWeaponButtonClick()
        {
            string itemType = "weapon";
            shopItemList = $"/items/{itemType}/{characterLevel}/{characterJob}";

            ClearItemList();

            StartCoroutine(GetItems());
        }

        public void OnArmorButtonClick()
        {
            string itemType = "armor";
            shopItemList = $"/items/{itemType}/{characterLevel}/{characterJob}";

            ClearItemList();

            StartCoroutine(GetItems());
        }

        IEnumerator GetItems()
        {
            string url = GameURL.DBServer.Server_URL + shopItemList;
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string jsonResponse = www.downloadHandler.text;

                    // JSON �迭�� List�� ��ȯ
                    List<ItemGetResponse> itemList = JsonUtility.FromJson<List<ItemGetResponse>>(jsonResponse);

                    // ������ ����Ʈ�� UI�� �߰�
                    AddItemsToUI(itemList);
                }
                else
                {
                    Debug.LogError("Error: " + www.error);
                }
            }
        }

        void AddItemsToUI(List<ItemGetResponse> itemList)
        {
            // ���ο� ��� UI ����
            foreach (var item in itemList)
            {
                // ������ �������� �����Ͽ� ���� ����
                GameObject newItem = Instantiate(itemPrefab, itemListParent);

                // ������ �����ۿ� ���� ����
                ItemUIHandler itemUIHandler = newItem.GetComponent<ItemUIHandler>();
                if (itemUIHandler != null)
                {
                    itemUIHandler.DisplayItemInfo(item);
                }
                else
                {
                    Debug.LogError("ItemUIHandler script not found on the item prefab.");
                }
            }
        }

        void ClearItemList()
        {
            // �θ� ��ü�� ��� �ڽ� ��ü ����
            foreach (Transform child in itemListParent)
            {
                Destroy(child.gameObject);
            }
        }
    }
}

namespace Shop
{
    [System.Serializable]
    public class ItemGetResponse
    {
        public long itemId;
        public long levelId;
        public long jobId;
        public long itemTypeId;
        public string itemNm;
        public int cost;
        public Shop.Stat stat; // Stat Ŭ������ Shop ���ӽ����̽� �߰� �� ���
        public string description;
    }
}

namespace Shop
{
    [System.Serializable]
    public class ItemUIHandler : MonoBehaviour
    {
        public TMP_Text itemNameText;
        public TMP_Text itemCostText;
        public TMP_Text itemStatText;

        public void DisplayItemInfo(ItemGetResponse item)
        {
            // ������ ������ UI�� ǥ��
            itemNameText.text = $"Name: {item.itemNm}";
            itemCostText.text = $"Cost: {item.cost}";
            itemStatText.text = $"Stats: HP {item.stat.hp}, ATK {item.stat.atk}, MP {item.stat.mp}, SPD {item.stat.spd}";
        }
    }
}