using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;

public class ShopButtonHandler : MonoBehaviour
{
    public GameObject itemPrefab; // 아이템 프리팹
    public Transform contentPanel; // Scroll View의 Content 부분
    public TMP_Text itemNameText;
    public TMP_Text itemStatText;
    public TMP_Text itemDescriptionText;
    private string shopItemList;
    public Button purchaseButton;
    public GameObject[] itemPrefabs; // 프리팹들을 저장할 배열

    private Dictionary<string, string> itemPrefabMap = new Dictionary<string, string>();

    private void Start()
    {
        purchaseButton.interactable = false;
        itemPrefabMap["누비지갑"] = "누비지갑";
        itemPrefabMap["두정갑"] = "두정갑";
        itemPrefabMap["로리카 세그멘타타"] = "로리카 세그멘타타";
        itemPrefabMap["리노토락스"] = "리노토락스";
        itemPrefabMap["면제배갑"] = "면제배갑";
        itemPrefabMap["미늘갑"] = "미늘갑";
        itemPrefabMap["산문갑"] = "산문갑";
        itemPrefabMap["석갑"] = "석갑";
        itemPrefabMap["찰갑"] = "찰갑";
        itemPrefabMap["탄코"] = "탄코";
        itemPrefabMap["토세이쿠소쿠"] = "토세이쿠소쿠";
        itemPrefabMap["풀 플레이트 아머"] = "풀 플레이트 아머";
        itemPrefabMap["피갑"] = "피갑";
        itemPrefabMap["청동흉갑"] = "청동흉갑";
        itemPrefabMap["크럭흉갑"] = "크럭흉갑";
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
        // itemPrefab 및 contentPanel이 null이 아닌지 확인
        if (itemPrefab == null || contentPanel == null)
        {
            Debug.LogError("itemPrefab 또는 contentPanel이 null입니다.");
            return;
        }

        // 새로운 목록 UI 생성
        foreach (var item in itemList)
        {
            /*if (itemPrefabMap.ContainsKey(item.itemNm))
            {*/
                //string prefabName = itemPrefabMap[item.itemNm];

                // 아이템 프리팹을 복제하여 Content 패널의 자식으로 추가
                GameObject newItem = Instantiate(itemPrefab, contentPanel);
                //GameObject newItem = Instantiate(Resources.Load<GameObject>($"Prefabs/Shop/Armor/{prefabName}"), contentPanel);
                ItemUIHandler itemUIHandler = newItem.GetComponent<ItemUIHandler>();

                if (itemUIHandler != null)
                {
                    // 아이템 정보 설정
                    itemUIHandler.SetItemInfo(item);
                }
                else
                {
                    Debug.LogError("인스턴스화된 아이템 프리팹에서 ItemUIHandler 구성 요소를 찾을 수 없습니다.");
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

        // 부모 객체의 모든 자식 객체 삭제
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
        public Stat stat; // Stat 클래스에 Shop 네임스페이스 추가 후 사용
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