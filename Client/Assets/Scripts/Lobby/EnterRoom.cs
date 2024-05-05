using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json.Linq;

public class EnterRoom : MonoBehaviour
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
                RoomId room = JsonUtility.FromJson<RoomId>(jsonResponse);

                UserDataManager.Instance.PlayerType = PlayerType.ENTRANT;
                UserDataManager.Instance.MatchRoomID = room.matchRoomId;
                UserDataManager.Instance.HostId = room.hostId;
                UserDataManager.Instance.EntrantId = room.entrantId;
                UserDataManager.Instance.MatchStatus = room.matchStatus;
                UserDataManager.Instance.StakeGold = room.stakeGold;

                UserDataManager.Instance.RoomListInfo.Clear();
                //Response Body → 200 응답 시 참여 완료 처리 / 참여 불가능 할 경우 예외(403 or 404) return 함
                SceneController.LoadScene(Scenes.Ingame.ToString());
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                JObject json = JObject.Parse(jsonResponse);

                string errorType = json["type"].ToString();
                string errorMessage = json["message"].ToString();
                string error = errorType + ": " + errorMessage;
                popupManager.ShowPopup(error);
            }
        }
    }
}
