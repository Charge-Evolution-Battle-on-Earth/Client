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

    public void LoginBtn()
    {
        string id = id_Input.text;
        string pw = pw_Input.text;
        
        if(string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pw))
        {
            ShowErrorMessage("�Է����� ���� ĭ�� �ֽ��ϴ�.");
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
                        ShowErrorMessage("�α��ο� �����߽��ϴ�. ���̵�� ��й�ȣ�� �ٽ� Ȯ�����ּ���.");
                    }
                }
                else
                {
                    string jsonResponse = request.downloadHandler.text;
                    JObject json = JObject.Parse(jsonResponse);

                    string errorType = json["type"].ToString();
                    string errorMessage = json["message"].ToString();
                    string error = errorType + ": " + errorMessage;
                    ShowErrorMessage(error);
                }
            }
        }
    }

    public void RegisterBtn()
    {
        SceneController.LoadScene(Scenes.Register.ToString());
    }

    public void ShowErrorMessage(string errorMessage)
    {
        popup.enabled = true;
        popupMessage.text = errorMessage;

        popup.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
    }
}
