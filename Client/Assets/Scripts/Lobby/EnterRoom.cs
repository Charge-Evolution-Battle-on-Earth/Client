using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class EnterRoom : MonoBehaviour
{
    public void OnclickButton()
    {
        StartCoroutine(RoomEnter());
    }

    IEnumerator RoomEnter()
    {
        string url = GameURL.DBServer.Server_URL + GameURL.DBServer.getRoomEnterPath;
        JObject roomId = new JObject();
        roomId["matchRoomId"] = UserDataManager.Instance.MatchRoomID;
        string jsonData = roomId.ToString();

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.SetRequestHeader("Authorization", $"Bearer {UserDataManager.Instance.AccessToken}");

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                //Response Body → 200 응답 시 참여 완료 처리 / 참여 불가능 할 경우 예외(403 or 404) return 함
                SceneController.LoadScene(Scenes.Ingame.ToString());
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }
    }
}
