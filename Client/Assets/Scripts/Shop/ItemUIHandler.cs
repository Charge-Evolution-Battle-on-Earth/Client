using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Collections;

public class ItemUIHandler : MonoBehaviour
{
    public Image itemImage; // 아이템 이미지를 표시할 Image 컴포넌트
    public TMP_Text itemNameText_Prefab;
    public TMP_Text itemCostText_Prefab;

    public TMP_Text itemNameText;
    public TMP_Text itemStatText;
    public TMP_Text itemDescriptionText;

    public TMP_Text MoneyText;

    public Shop.ShopItemGetResponse item;
    public Button purchaseButton;

    public void SetItemInfo(Shop.ShopItemGetResponse newItem)
    {
        item = newItem;

        itemNameText_Prefab.text = item.itemNm;
        itemCostText_Prefab.text = item.cost.ToString();
    }

    public void OnButtonClick()
    {
        UserDataManager.Instance.ClickedItemId = item.itemId;
        Image clickedImage = GetComponent<Image>();
        itemImage.sprite = clickedImage.sprite;
        itemNameText.text = item.itemNm;
        itemStatText.text = $"HP: {item.stat.hp}\tMP: {item.stat.mp}\nATK: {item.stat.atk}\tSPD: {item.stat.spd}";
        itemDescriptionText.text = item.description;
        purchaseButton.interactable = true;
    }

    public void purchaseOnButtonClick()
    {
        StartCoroutine(BuyItem());
    }
    IEnumerator BuyItem()
    {
        JObject jobjItemId = new JObject();
        jobjItemId["itemId"] = UserDataManager.Instance.ClickedItemId;

        string jsonData = jobjItemId.ToString();
        string url = GameURL.DBServer.Server_URL + GameURL.DBServer.getShopBuyPath;

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.SetRequestHeader("Authorization", $"Bearer {UserDataManager.Instance.AccessToken}");

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                BuyGetResponse buyGetResponse = JsonUtility.FromJson<BuyGetResponse>(jsonResponse);

                MoneyText.text = buyGetResponse.money.ToString();
                Debug.Log("구매에 성공했습니다.");
            }
            else
            {
                Debug.LogError("구매에 실패했습니다.");
            }
        }
    }
}

[System.Serializable]
public class BuyGetResponse
{
    public long itemId;
    public string itemNm;
    public int money;
}
