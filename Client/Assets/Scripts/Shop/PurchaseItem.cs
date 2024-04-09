using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json.Linq;

public class PurchaseItem : MonoBehaviour
{
    public TMP_Text MoneyText;
    public PopupManager popupManager;

    public void Start()
    {
        popupManager.HidePopup();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            popupManager.HidePopup();
        }
    }

    public IEnumerator BuyItem()
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
                popupManager.ShowPopup("구매 성공");
            }
            else
            {
                popupManager.ShowPopup("구매 실패");
            }
        }
    }
}
