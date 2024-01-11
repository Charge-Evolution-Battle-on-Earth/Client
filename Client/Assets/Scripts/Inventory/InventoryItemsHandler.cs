using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;

public class InventoryItemsHandler : MonoBehaviour
{
    public GameObject itemPrefab; // 아이템 프리팹
    public Transform contentPanel; // Scroll View의 Content 부분
    public TMP_Text itemNameText;
    public TMP_Text itemStatText;
    public TMP_Text itemDescriptionText;
    private string invenItemList;
    
    public void OnClickButton(string itemType)//무기 갑옷 버튼
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
        // itemPrefab 및 contentPanel이 null이 아닌지 확인
        if (itemPrefab == null || contentPanel == null)
        {
            Debug.LogError("itemPrefab 또는 contentPanel이 null입니다.");
            return;
        }

        // 새로운 목록 UI 생성
        foreach (var item in itemList)
        {
            // itemPrefab과 contentPanel이 null이 아닌지 확인하기 위한 디버그 문
            //Debug.Log(item.itemNm + "에 대한 아이템 UI 생성 중.");

            // 아이템 프리팹을 복제하여 Content 패널의 자식으로 추가
            GameObject newItem = Instantiate(itemPrefab, contentPanel);
            InventoryItemUIHandler inventoryItemUIHandler = newItem.GetComponent<InventoryItemUIHandler>();

            if (inventoryItemUIHandler != null)
            {
                // 아이템 정보 설정
                inventoryItemUIHandler.SetItemInfo(item);
            }
            else
            {
                Debug.LogError("인스턴스화된 아이템 프리팹에서 InventoryItemUIHandler 구성 요소를 찾을 수 없습니다.");
            }
        }
    }

    void ClearItemList()
    {
        itemNameText.text = "";
        itemStatText.text = "";
        itemDescriptionText.text = "";


        // 부모 객체의 모든 자식 객체 삭제
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }
    }
}
