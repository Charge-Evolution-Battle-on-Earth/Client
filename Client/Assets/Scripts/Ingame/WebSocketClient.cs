using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using WebSocketSharp;

public class WebSocketClient : MonoBehaviour
{
    private WebSocket webSocket;
    private APIManager apiManager;

    void Start()
    {
        apiManager = GetComponent<APIManager>();

        string gameUrl = GameURL.DBServer.Server_URL;
        string socketUrl = GameURL.DBServer.Server_WebSocketURL;
        string accessToken = UserDataManager.Instance.AccessToken;

        // ������ URL�� ��ū�� ���� �Ű������� �߰�
        string socketAndApiUrl = $"{socketUrl}?Authorization: 'Bearer {accessToken}'";

        // ���� �������� Ȯ���ϱ� ���� CertificateHandler ���
        UnityWebRequest www = UnityWebRequest.Get(gameUrl+"/play");
        www.certificateHandler = new AcceptAllCertificates();

        www.SendWebRequest().completed += (asyncOperation) =>
        {
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("���� �������� ��ȿ�մϴ�.");

                // ���� �������� ��ȿ�ϸ� ������ ���� ����
                webSocket = new WebSocket(socketAndApiUrl);

                webSocket.OnOpen += OnWebSocketOpen;
                webSocket.OnMessage += OnWebSocketMessage;
                webSocket.OnError += OnWebSocketError;

                webSocket.Connect();
            }
            else
            {
                Debug.LogError($"���� ������ ��ȿ�� �˻� ����: {www.error}");
            }
        };
    }

    void OnDestroy()
    {
        if (webSocket != null && webSocket.IsAlive)
        {
            webSocket.Close();
        }
    }
    private void OnWebSocketError(object sender, ErrorEventArgs e)
    {
        Debug.LogError($"WebSocket ����: {e.Message}");
    }

    private void OnWebSocketOpen(object sender, EventArgs e)
    {
        Debug.Log("������ ������ ���Ƚ��ϴ�.");

        // ���� ���� �� �λ� �޽��� ��û
        StartCoroutine(apiManager.GetGreetingMessage(UserDataManager.Instance.MatchRoomID.ToString()));
    }

    private void OnWebSocketMessage(object sender, MessageEventArgs e)
    {
        Debug.Log($"���� �޽���: {e.Data}");
        // �޽��� ó�� ���� �߰�
    }
}

// AcceptAllCertificates Ŭ���� ����
public class AcceptAllCertificates : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // ��� �������� ����Ϸ��� true�� ��ȯ
        return true;
    }

}

public class APIManager : MonoBehaviour
{
    private string baseApiUrl;

    void Start()
    {
        baseApiUrl = GameURL.DBServer.Server_WebSocketURL;
    }

    public IEnumerator GetGreetingMessage(string matchRoomId)
    {
        string apiUrl = $"{baseApiUrl}/greeting/{matchRoomId}";

        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        request.SetRequestHeader("Authorization", UserDataManager.Instance.AccessToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            // JSON �Ľ� �� ��� ó�� ���� �߰�
            Debug.Log(jsonResult);
        }
        else
        {
            Debug.LogError($"API ��û ����: {request.error}");
        }
    }
}
