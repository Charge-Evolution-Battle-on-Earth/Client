using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public enum matchStatus
{ 
    WAITING,
    READY,
    IN_PROGRESS,
    FINISHED
}

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
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                SliceResponse sliceResponse = JObject.Parse(jsonResponse).ToObject<SliceResponse>();


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