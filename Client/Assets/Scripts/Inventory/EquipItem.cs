using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json.Linq;

public class EquipItemManager : MonoBehaviour
{
    public void EquipItem()
    {
        StartCoroutine(EquipItemCoroutine());
    }

    IEnumerator EquipItemCoroutine()
    {
        JObject equipItemRequest = new JObject();
        equipItemRequest["itemTypeId"] = UserDataManager.Instance.ItemTypeId;
        equipItemRequest["characterItemId"] = UserDataManager.Instance.CharacterItemId;

        string jsonData = JsonUtility.ToJson(equipItemRequest);
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

                Debug.Log("아이템 장착 성공. CharacterItemId: " + responseData.characterItemId);
            }
            else
            {
                Debug.LogError("장착 실패: " + request.error);
            }
        }
    }
}

[System.Serializable]
public class EquipItemResponse
{
    public long characterItemId;
}
