using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class Choice : MonoBehaviour
{
    public DropdownController dropdownController;
    public PopupManager popupManager;

    void Start()
    {
        popupManager.HidePopup();
        StartCoroutine(GetNationList());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            popupManager.HidePopup();
        }
    }

    IEnumerator GetNationList()
    {
        string url = GameURL.DBServer.Server_URL + GameURL.DBServer.getNationsPath;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            //www.SetRequestHeader("Authorization", $"Bearer {UserDataManager.Instance.AccessToken}");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // JSON 응답 파싱
                string jsonResponse = www.downloadHandler.text;

                // 배열을 객체로 감싸서 처리
                string wrappedJsonResponse = $"{{ \"data\": {jsonResponse} }}";

                // 객체로 처리
                NationsListWrapper nationsListWrapper = JsonUtility.FromJson<NationsListWrapper>(wrappedJsonResponse);

                // 여기에서 데이터 처리
                dropdownController.AddItemsToDropdown(nationsListWrapper.data);
            }
            else
            {
                popupManager.ShowPopup("Error: " + www.error);
            }
        }
    }

    public void GoToLobby()
    {
        JObject userData = new JObject();
        userData["userId"] = UserDataManager.Instance.UserId;
        userData["nationId"] = UserDataManager.Instance.NationId;
        userData["jobId"] = UserDataManager.Instance.JobId;

        string jsonData = userData.ToString();

        StartCoroutine(SendData(jsonData));

        IEnumerator SendData(string jsonData)
        {
            string url = GameURL.AuthServer.Server_URL + GameURL.AuthServer.userRegisterPath;

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string jsonResponse = request.downloadHandler.text;
                    JObject json = JObject.Parse(jsonResponse);

                    if (json.ContainsKey("characterId"))
                    {
                        // userId 저장
                        string characterId = json["characterId"].ToString();
                        PlayerPrefs.SetString("characterId", characterId);
                        UserDataManager.Instance.CharacterId = Convert.ToInt64(characterId);
                    }
                    if (json.ContainsKey("accessToken"))
                    {
                        // accessToken 저장
                        string accessToken = json["accessToken"].ToString();
                        PlayerPrefs.SetString("accessToken", accessToken);

                        UserDataManager.Instance.AccessToken = accessToken;
                    }
                    SceneController.LoadScene(Scenes.Lobby.ToString());
                }
            }
        }
    }
}
[System.Serializable]
public class NationsListWrapper
{
    public List<NationGetListResponse> data;
}