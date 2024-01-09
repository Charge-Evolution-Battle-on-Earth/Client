using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json.Linq;

public class SellItemManager : MonoBehaviour
{
    public TMP_Text moneyText;

    public void SellItem()
    {
        StartCoroutine(SellItemCoroutine());
    }

    IEnumerator SellItemCoroutine()
    {
        JObject sellItemData = new JObject();
        sellItemData["itemTypeId"] = UserDataManager.Instance.ItemTypeId;
        sellItemData["characterItemId"] = UserDataManager.Instance.CharacterItemId;

        string jsonData = JsonUtility.ToJson(sellItemData);
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
                // ���� �����͸� JObject�� ��ȯ
                JObject responseData = JObject.Parse(request.downloadHandler.text);

                int money = responseData["money"].Value<int>();
                moneyText.text = money.ToString();

                Debug.Log("�Ǹ� ����. ���� �ܾ�: " + money);
            }
            else
            {
                Debug.LogError("�Ǹ� ����: " + request.error);
            }
        }
    }
}
