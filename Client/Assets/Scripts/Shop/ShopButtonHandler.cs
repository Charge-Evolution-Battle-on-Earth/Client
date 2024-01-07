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
        public GameObject itemPrefab; // Unity Inspector에서 연결할 아이템 프리팹
        public Transform itemListParent; // Unity Inspector에서 연결할 아이템 리스트의 부모 오브젝트
        private string shopItemList;
        private long characterLevel;
        private long characterJob;

        private void Awake()
        {
            // Awake 메서드에서 초기화
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

                    // JSON 배열을 List로 변환
                    List<ItemGetResponse> itemList = JsonUtility.FromJson<List<ItemGetResponse>>(jsonResponse);

                    // 아이템 리스트를 UI에 추가
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
            // 새로운 목록 UI 생성
            foreach (var item in itemList)
            {
                // 아이템 프리팹을 복제하여 씬에 생성
                GameObject newItem = Instantiate(itemPrefab, itemListParent);

                // 생성된 아이템에 정보 전달
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
            // 부모 객체의 모든 자식 객체 삭제
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
        public Shop.Stat stat; // Stat 클래스에 Shop 네임스페이스 추가 후 사용
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
            // 아이템 정보를 UI에 표시
            itemNameText.text = $"Name: {item.itemNm}";
            itemCostText.text = $"Cost: {item.cost}";
            itemStatText.text = $"Stats: HP {item.stat.hp}, ATK {item.stat.atk}, MP {item.stat.mp}, SPD {item.stat.spd}";
        }
    }
}