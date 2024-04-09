using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using TMPro;

public class Register : MonoBehaviour
{
    public TMP_InputField email_Input;
    public TMP_InputField nick_Input;
    public TMP_InputField pw_Input;
    public TMP_InputField confirmPw_Input;
    public Button registerBtn;
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

    public void RegisterUser()
    {
        string email = email_Input.text;
        string nick = nick_Input.text;
        string pw = pw_Input.text;
        string confirmPw = confirmPw_Input.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(nick) ||
            string.IsNullOrEmpty(pw) || string.IsNullOrEmpty(confirmPw))
        {
            popupManager.ShowPopup("입력하지 않은 칸이 있습니다.");
            return;
        }
        if (pw != confirmPw)
        {
            popupManager.ShowPopup("비밀번호와 비밀번호 확인이 일치하지 않습니다.");
            return;
        }

        JObject userData = new JObject();
        userData["email"] = email_Input.text;
        userData["password"] = pw_Input.text;
        userData["nickname"] = nick_Input.text;

        string jsonData = userData.ToString();

        StartCoroutine(SendData(jsonData));
    }

    private IEnumerator SendData(string jsonData)
    {
        string url = GameURL.AuthServer.Server_URL + GameURL.AuthServer.userJoinPath;

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                JObject json = JObject.Parse(jsonResponse);

                string errorType = json["type"].ToString();
                string errorMessage = json["message"].ToString();
                string error = errorType + ": " + errorMessage;
                popupManager.ShowPopup(error);
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                JObject json = JObject.Parse(jsonResponse);

                if (json.ContainsKey("userId"))
                {
                    string userId = json["userId"].ToString();
                    PlayerPrefs.SetString("userId", userId);
                    UserDataManager.Instance.UserId = Convert.ToInt64(userId);
                }
                popupManager.ShowPopup("가입 성공");
                CustomSceneManager.LoadScene(Scenes.Choice.ToString());
            }
        }
    }

    public void ReturnScene()
    {
        SceneController.LoadScene(Scenes.Login.ToString());
    }
}
