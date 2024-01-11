using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class MatchRoomListFetcher : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(FetchMatchRoomList());
    }
    IEnumerator FetchMatchRoomList()
    {
        string url = GameURL.DBServer.Server_URL + GameURL.DBServer.getMatchRoomListPath;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", $"Bearer {UserDataManager.Instance.AccessToken}");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                List<SliceResponse> sliceResponse = JsonConvert.DeserializeObject<List<SliceResponse>>(jsonResponse);

                UserDataManager.Instance.RoomListInfo = sliceResponse;
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }
    }
}

[System.Serializable]
public class SliceResponse
{
    public List<ContentType> content;
    public bool first;
    public bool last;
    public int size;
    public int number;
    public int numberOfElement;
    public bool empty;
}

[System.Serializable]
public class ContentType
{
    public long matchRoomId;
    public long hostId;
    public long entrantId;
    public matchStatus matchStatus;
    public int stakeGold;
}

[System.Serializable]
public enum matchStatus
{
    WAITING,
    READY,
    IN_PROGRESS,
    FINISHED
}
