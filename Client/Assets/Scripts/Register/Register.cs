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

    public void RegisterUser()
    {
        string email = email_Input.text;
        string nick = nick_Input.text;
        string pw = pw_Input.text;
        string confirmPw = confirmPw_Input.text;
        //PlayerPrefs.DeleteAll();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(nick) || 
            string.IsNullOrEmpty(pw) || string.IsNullOrEmpty(confirmPw))
        {
            ShowErrorMessage("입력하지 않은 칸이 있습니다.");
            return;
        }
        if (pw != confirmPw)
        {
            ShowErrorMessage("비밀번호와 비밀번호 확인이 일치하지 않습니다.");
            return;
        }

        JObject userData = new JObject();
        userData["email"] = email_Input.text;
        userData["password"] = pw_Input.text;
        userData["nickname"] = nick_Input.text;

        string jsonData = userData.ToString();

        // UnityWebRequest를 사용하여 서버에 POST 요청 보내기
        StartCoroutine(SendData(jsonData));

        IEnumerator SendData(string jsonData)
        {
            string url = GameURL.AuthServer.Server_URL+GameURL.AuthServer.userJoinPath;  //서버 주소로 변경

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
                    ShowErrorMessage(error);
                }
                else
                {
                    // JSON 응답 파싱
                    string jsonResponse = request.downloadHandler.text;
                    JObject json = JObject.Parse(jsonResponse);

                    if(json.ContainsKey("userId"))
                    {
                        // userId 저장
                        string userId = json["userId"].ToString();
                        PlayerPrefs.SetString("userId", userId);
                        UserDataManager.Instance.UserId = Convert.ToInt64(userId);

                    }
                    ShowErrorMessage("가입 성공");
                    CustomSceneManager.LoadScene(Scenes.Choice.ToString());
                }
            }
        }
    }

    public void ReturnScene()
    {
        SceneController.LoadScene(Scenes.Login.ToString());
    }

    public void ShowErrorMessage(string errorMessage)
    {
        popup.enabled = true;
        popupMessage.text = errorMessage;

        popup.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
    }
}
