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
    public GameObject itemPrefab; // 아이템 프리팹
    public Transform contentPanel; // Scroll View의 Content 부분
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
            gridLayout.padding.left = 10; // 좌측 여백 설정
            gridLayout.padding.right = 10; // 우측 여백 설정
            gridLayout.padding.top = 10; // 상단 여백 설정
            gridLayout.padding.bottom = 10; // 하단 여백 설정
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
    public void OnClickButton(string itemType)//무기 갑옷 버튼
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


                // 아이템 프리팹을 복제하여 Content 패널의 자식으로 추가
                GameObject newItem = Instantiate(itemPrefabs.FirstOrDefault(item => item.name == prefabName), contentPanel);
                InventoryItemUIHandler inventoryItemUIHandler = newItem.GetComponent<InventoryItemUIHandler>();

                if (inventoryItemUIHandler != null)
                {
                    // 아이템 정보 설정
                    inventoryItemUIHandler.SetItemInfo(item);

                    // 테두리 추가 및 설정
                    Outline outline = newItem.GetComponent<Outline>();
                    if (outline == null)
                    {
                        outline = newItem.AddComponent<Outline>();
                    }

                    if (item.characterItemId == UserDataManager.Instance.EquippedItemId)
                    {
                        // 장착된 아이템 강조: 테두리 색상을 노란색으로 설정
                        outline.effectColor = Color.red;
                        outline.effectDistance = new Vector2(5, 5); // 테두리 두께 설정
                    }
                    else
                    {
                        // 장착되지 않은 아이템: 테두리 색상을 투명하게 설정
                        outline.effectColor = Color.clear;
                    }
                }
                else
                {
                    popupManager.ShowPopup("인스턴스화된 아이템 프리팹에서 InventoryItemUIHandler 구성 요소를 찾을 수 없습니다.");
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

        // 부모 객체의 모든 자식 객체 삭제
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
                Debug.LogError("장착중인 아이템 없음");
            }
        }
        else
        {
            Debug.LogError("네트워크 연결 오류");
        }
    }

    private void InitializeItemPrefabMap()
    {
        // 아이템 맵 초기화 코드
        itemPrefabMap["누비지갑"] = "누비지갑";
        itemPrefabMap["피갑"] = "피갑";
        itemPrefabMap["석갑"] = "석갑";
        itemPrefabMap["크럭흉갑"] = "크럭흉갑";
        itemPrefabMap["리노토락스"] = "리노토락스";
        itemPrefabMap["청동갑옷"] = "청동갑옷";
        itemPrefabMap["면제배갑"] = "면제배갑";
        itemPrefabMap["스케일 아머"] = "스케일 아머";
        itemPrefabMap["탄코"] = "탄코";
        itemPrefabMap["찰갑"] = "찰갑";
        itemPrefabMap["산문갑"] = "산문갑";
        itemPrefabMap["로리카 세그멘타타"] = "로리카 세그멘타타";
        itemPrefabMap["두정갑"] = "두정갑";
        itemPrefabMap["토세이구소쿠"] = "토세이구소쿠";
        itemPrefabMap["풀 플레이트아머"] = "풀 플레이트아머";

        // 전사 무기
        itemPrefabMap["죽창"] = "죽창";
        itemPrefabMap["당파창"] = "당파창";
        itemPrefabMap["플랜지드메이스"] = "플랜지드메이스";
        itemPrefabMap["편곤"] = "편곤";
        itemPrefabMap["장창"] = "장창";
        itemPrefabMap["토마호크"] = "토마호크";
        itemPrefabMap["샴쉬르"] = "샴쉬르";
        itemPrefabMap["금쇄봉"] = "금쇄봉";
        itemPrefabMap["여포의 방천화극"] = "여포의 방천화극";
        itemPrefabMap["카타나"] = "카타나";
        itemPrefabMap["관우의 청룡언월도"] = "관우의 청룡언월도";
        itemPrefabMap["할버드"] = "할버드";
        itemPrefabMap["충무공의 장검"] = "충무공의 장검";
        itemPrefabMap["글라디우스"] = "글라디우스";
        itemPrefabMap["모노호시자오"] = "모노호시자오";

        // 궁수 무기
        itemPrefabMap["바람총"] = "바람총";
        itemPrefabMap["새총"] = "새총";
        itemPrefabMap["투석구"] = "투석구";
        itemPrefabMap["카이사르의 필룸"] = "카이사르의 필룸";
        itemPrefabMap["와큐"] = "와큐";
        itemPrefabMap["골각궁"] = "골각궁";
        itemPrefabMap["쇠뇌"] = "쇠뇌";
        itemPrefabMap["승자총통"] = "승자총통";
        itemPrefabMap["죽궁"] = "죽궁";
        itemPrefabMap["화승총"] = "화승총";
        itemPrefabMap["제갈량의 연노"] = "제갈량의 연노";
        itemPrefabMap["흑각궁"] = "흑각궁";
        itemPrefabMap["대제의 플린트락머스킷"] = "대제의 플린트락머스킷";
        itemPrefabMap["오오즈츠"] = "오오즈츠";
        itemPrefabMap["태조의 어궁구"] = "태조의 어궁구";

        // 성직자 무기
        itemPrefabMap["청동방울"] = "청동방울";
        itemPrefabMap["사인검"] = "사인검";
        itemPrefabMap["엘레나의 성정"] = "엘레나의 성정";
        itemPrefabMap["청동검"] = "청동검";
        itemPrefabMap["롱기누스의 창"] = "롱기누스의 창";
        itemPrefabMap["금제감장보검"] = "금제감장보검";
        itemPrefabMap["발렌시아의 성배"] = "발렌시아의 성배";
        itemPrefabMap["석장"] = "석장";
        itemPrefabMap["성십자가"] = "성십자가";
        itemPrefabMap["근초고왕의 칠지도"] = "근초고왕의 칠지도";
        itemPrefabMap["금동대향로"] = "금동대향로";
        itemPrefabMap["잔다르크의 반지"] = "잔다르크의 반지";
        itemPrefabMap["사해문서"] = "사해문서";
        itemPrefabMap["무구정광대다라니경"] = "무구정광대다라니경";
        itemPrefabMap["쿠란"] = "쿠란";
    }
}