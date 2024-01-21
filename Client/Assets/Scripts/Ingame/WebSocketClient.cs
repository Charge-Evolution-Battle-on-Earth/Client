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

        string socketAndApiUrl = GameURL.DBServer.Server_WebSocketURL;
        string accessToken = UserDataManager.Instance.AccessToken;

        // ��ū ����
        webSocket.SetCookie(new WebSocketSharp.Net.Cookie("Authorization", accessToken));

        webSocket.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket connection opened.");

            // ���� ���� �� greeting message�� ��û
            StartCoroutine(apiManager.GetGreetingMessage(UserDataManager.Instance.MatchRoomID.ToString()));
        };

        webSocket.OnMessage += (sender, e) =>
        {
            Debug.Log($"Received message: {e.Data}");
            // ���⼭ �޽��� ó�� ������ �߰��ϼ���.
        };

        webSocket.Connect();
    }

    void OnDestroy()
    {
        if (webSocket != null && webSocket.IsAlive)
        {
            webSocket.Close();
        }
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
            // JSON �Ľ� �� ��� ó�� ������ �߰��ϼ���.
            Debug.Log(jsonResult);
        }
        else
        {
            Debug.LogError($"API request failed: {request.error}");
        }
    }
}
