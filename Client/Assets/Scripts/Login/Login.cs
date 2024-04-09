using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using TMPro;

public class Login : HttpServerBase
{
    public TMP_InputField id_Input;
    public TMP_InputField pw_Input;
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

    public void LoginBtn()
    {
        string id = id_Input.text;
        string pw = pw_Input.text;
        
        if(string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pw))
        {
            popupManager.ShowPopup("�Է����� ���� ĭ�� �ֽ��ϴ�.");
            return;
        }

        //Newtonsoft.Json ��Ű���� �̿��� Json ����
        JObject userData = new JObject();
        userData["email"] = id;
        userData["password"] = pw;

        string jsonData = userData.ToString();

        // UnityWebRequest�� ����Ͽ� ������ POST ��û ������
        StartCoroutine(SendData(jsonData));

        IEnumerator SendData(string jsonData)
        {
            string url = GameURL.AuthServer.Server_URL + GameURL.AuthServer.userLogInPath;

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    // JSON ���� �Ľ�
                    string jsonResponse = request.downloadHandler.text;
                    JObject json = JObject.Parse(jsonResponse);

                    if (json.ContainsKey("accessToken"))
                    {
                        // accessToken ����
                        string accessToken = json["accessToken"].ToString();
                        PlayerPrefs.SetString("accessToken", accessToken);

                        UserDataManager.Instance.AccessToken = accessToken;

                        SceneController.LoadScene(Scenes.Lobby.ToString());
                    }
                    else
                    {
                        popupManager.ShowPopup("�α��ο� �����߽��ϴ�. ���̵�� ��й�ȣ�� �ٽ� Ȯ�����ּ���.");
                    }
                }
                else
                {
                    string jsonResponse = request.downloadHandler.text;
                    JObject json = JObject.Parse(jsonResponse);

                    string errorType = json["type"].ToString();
                    string errorMessage = json["message"].ToString();
                    string error = errorType + ": " + errorMessage;
                    popupManager.ShowPopup(error);
                }
            }
        }
    }

    public void RegisterBtn()
    {
        SceneController.LoadScene(Scenes.Register.ToString());
    }
}
