using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json.Linq;

public class EquipItemManager : MonoBehaviour
{
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

    public void EquipItem()
    {
        StartCoroutine(EquipItemCoroutine());
    }

    IEnumerator EquipItemCoroutine()
    {
        JObject equipItemRequest = new JObject();
        equipItemRequest["itemTypeId"] = UserDataManager.Instance.ItemTypeId;
        equipItemRequest["characterItemId"] = UserDataManager.Instance.ClickedCharacterItemId;
        UserDataManager.Instance.EquippedItemId = UserDataManager.Instance.ClickedCharacterItemId;

        string jsonData = equipItemRequest.ToString();
        string url = GameURL.DBServer.Server_URL + GameURL.DBServer.getItemEquipPath;

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
                EquipItemResponse responseData = JsonUtility.FromJson<EquipItemResponse>(request.downloadHandler.text);
                UserDataManager.Instance.ClearUI = true;

                popupManager.ShowPopup("장착 성공\nCharacterItemId: " + responseData.characterItemId);
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
}

[System.Serializable]
public class EquipItemResponse
{
    public long characterItemId;
}
