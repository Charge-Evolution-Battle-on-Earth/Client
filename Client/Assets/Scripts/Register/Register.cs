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
            Debug.LogWarning("�Է����� ���� ĭ�� �ֽ��ϴ�.");
            return;
        }
        if (pw != confirmPw)
        {
            Debug.LogWarning("��й�ȣ�� ��й�ȣ Ȯ���� ��ġ���� �ʽ��ϴ�.");
            return;
        }

        JObject userData = new JObject();
        userData["email"] = email_Input.text;
        userData["password"] = pw_Input.text;
        userData["nickname"] = nick_Input.text;

        string jsonData = userData.ToString();

        // UnityWebRequest�� ����Ͽ� ������ POST ��û ������
        StartCoroutine(SendData(jsonData));

        IEnumerator SendData(string jsonData)
        {
            string url = GameURL.AuthServer.Server_URL+GameURL.AuthServer.userJoinPath;  //���� �ּҷ� ����

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(request.error);
                }
                else
                {
                    CustomSceneManager.LoadScene(Scenes.Choice.ToString());
                    Debug.Log("���� ����");
                }
            }
        }

        // TODO: �̸����� ���ų� �г����� ���� ����ڰ� ���� �� �޽���
    }

    public void ReturnScene()
    {
        SceneController.LoadScene(Scenes.Login.ToString());
    }
}
