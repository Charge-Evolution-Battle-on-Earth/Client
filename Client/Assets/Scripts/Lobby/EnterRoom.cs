using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json.Linq;

public class EnterRoom : MonoBehaviour
{
    public Image popup;
    public TMP_Text popupMessage;

    public void Start()
    {
        popup.enabled = false;
    }
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            popup.enabled = false;
            popupMessage.text = "";
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
                //Response Body �� 200 ���� �� ���� �Ϸ� ó�� / ���� �Ұ��� �� ��� ����(403 or 404) return ��
                SceneController.LoadScene(Scenes.Ingame.ToString());
            }
            else
            {
                ShowErrorMessage("Error: " + www.error);
            }
        }
    }

    public void ShowErrorMessage(string errorMessage)
    {
        popup.enabled = true;
        popupMessage.text = errorMessage;

        popup.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
    }
}
