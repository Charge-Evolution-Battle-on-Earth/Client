using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class ServerManager : MonoBehaviour
{
    public PopupManager popupManager;
    public enum SendType { GET, POST }

    /*private void Start()
    {
        popupManager.HidePopup();

    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            popupManager.HidePopup();
        }
    }*/

    protected IEnumerator SendRequestCor(string url, SendType sendType, string jsonData, Action<string> callback)
    {
        UnityWebRequest www = new UnityWebRequest(url, sendType.ToString());
        www.SetRequestHeader("Authorization", $"Bearer {UserDataManager.Instance.AccessToken}");

        if (sendType == SendType.POST)
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jsonData));
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = www.downloadHandler.text;
            callback?.Invoke(jsonResponse);
        }
        else
        {
            popupManager.ShowPopup("Error: " + www.error);
            callback?.Invoke(null);
        }
    }

    public void SendRequest(string url, SendType sendType, string jsonData, Action<string> callback)
    {
        StartCoroutine(SendRequestCor(url, sendType, jsonData, callback));
    }
}