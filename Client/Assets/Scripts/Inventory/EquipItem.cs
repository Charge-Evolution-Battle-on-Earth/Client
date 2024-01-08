using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json.Linq;

public class EquipItemManager : MonoBehaviour
{
    public long itemTypeId;          // 장착할 아이템 타입 ID
    public long characterItemId;     // 캐릭터 아이템 ID

    public void EquipItem()
    {
        StartCoroutine(EquipItemCoroutine());
    }

    IEnumerator EquipItemCoroutine()
    {
        JObject equipItemRequest = new JObject();
        equipItemRequest["itemTypeId"] = itemTypeId;//무기 or 갑옷
        equipItemRequest["characterItemId"] = characterItemId;//장착중인 아이템 아이디?
       
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
