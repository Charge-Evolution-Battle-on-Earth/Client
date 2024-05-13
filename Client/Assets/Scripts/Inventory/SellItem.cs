using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json.Linq;

public class SellItemManager : MonoBehaviour
{
    public TMP_Text moneyText;
    public Image itemImage;
    public Button equipBtn;
    public Button sellBtn;
    public Button unequipBtn;
    public TMP_Text itemNameText;
    public TMP_Text itemStatText;
    public TMP_Text itemDescriptionText;
    public Transform contentPanel;
    public PopupManager popupManager;

    void Start()
    {
        popupManager.HidePopup();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            popupManager.HidePopup();
        }
    }

    public void SellItem()
    {
        equipBtn.interactable = false;
        sellBtn.interactable = false;
        unequipBtn.interactable = false;
        ImageTransparency();
        StartCoroutine(SellItemCoroutine());
    }

    IEnumerator SellItemCoroutine()
    {
        JObject sellItemData = new JObject();
        sellItemData["itemTypeId"] = UserDataManager.Instance.ItemTypeId;
        sellItemData["characterItemId"] = UserDataManager.Instance.CharacterItemId;
        if(UserDataManager.Instance.ItemTypeId == 1)
        {
            for(int i = 0; i < UserDataManager.Instance.WeaponItemList.Count; i++)
            {
                if(UserDataManager.Instance.WeaponItemList[i].characterItemId == UserDataManager.Instance.CharacterItemId)
                {
                    UserDataManager.Instance.WeaponItemList.RemoveAt(i);
                    break;
                }
            }
        }
        else if(UserDataManager.Instance.ItemTypeId == 2)
        {
            for (int i = 0; i < UserDataManager.Instance.ArmorItemList.Count; i++)
            {
                if (UserDataManager.Instance.ArmorItemList[i].characterItemId == UserDataManager.Instance.CharacterItemId)
                {
                    UserDataManager.Instance.ArmorItemList.RemoveAt(i);
                    break;
                }
            }
        }
        string jsonData = sellItemData.ToString();
        string url = GameURL.DBServer.Server_URL + GameURL.DBServer.getItemSellPath;

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
                // 응답 데이터를 JObject로 변환
                JObject responseData = JObject.Parse(request.downloadHandler.text);

                int money = responseData["money"].Value<int>();
                moneyText.text = money.ToString();
                UserDataManager.Instance.ClearUI = true;
                popupManager.ShowPopup("판매 성공\n현재 잔액: " + money);
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                JObject json = JObject.Parse(jsonResponse);

                string errorType = json["type"].ToString();
                string errorMessage = json["message"].ToString();
                string error = errorType + ": " + errorMessage;
                popupManager.ShowPopup(error);
            }
        }
    }

    void ImageTransparency()
    {
        Color color = new Color();
        color.a = 0f;
        itemImage.color = color;
    }
}
