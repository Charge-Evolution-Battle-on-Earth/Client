using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MakeRoom : MonoBehaviour
{
    public void onClickButton()
    {
        StartCoroutine(RoomMake());
    }

    IEnumerator RoomMake()
    {
        string url = GameURL.DBServer.Server_URL + GameURL.DBServer.getMatchRoomListPath;

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.SetRequestHeader("Authorization", $"Bearer {UserDataManager.Instance.AccessToken}");
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                RoomId roomId = JsonUtility.FromJson<RoomId>(jsonResponse);

                UserDataManager.Instance.MatchRoomID = roomId.matchRoomId;
                UserDataManager.Instance.HostId = roomId.hostId;
                UserDataManager.Instance.EntrantId = roomId.entrantId;
                UserDataManager.Instance.MatchStatus = roomId.matchStatus;
                UserDataManager.Instance.StakeGold = roomId.stakeGold;

                Debug.Log("방 생성 성공.");
                SceneController.LoadScene(Scenes.Ingame.ToString());
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }
    }
}

[System.Serializable]
public class RoomId
{
    public long matchRoomId;
    public long hostId;
    public long entrantId;
    public matchStatus matchStatus;
    public int stakeGold;
}