using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json.Linq;

public class UnequipItemManager : MonoBehaviour
{
    public void UnequipItem()
    {
        StartCoroutine(UnequipItemCoroutine());
    }

    IEnumerator UnequipItemCoroutine()
    {
        JObject unequipItemRequest = new JObject();
        unequipItemRequest["itemTypeId"] = UserDataManager.Instance.ItemTypeId;
        unequipItemRequest["characterItemId"] = UserDataManager.Instance.CharacterItemId;

        string jsonData = JsonUtility.ToJson(unequipItemRequest);
        string url = GameURL.DBServer.Server_URL + GameURL.DBServer.getItemUnequipPath;

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
                // ���� �����͸� JObject�� ��ȯ
                JObject responseData = JObject.Parse(request.downloadHandler.text);

                Debug.Log("���� ����. CharacterItemId: " + responseData["characterItemId"]);
            }
            else
            {
                Debug.LogError("���� ����: " + request.error);
            }
        }
    }
}
