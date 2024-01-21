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

        // 토큰 설정
        webSocket.SetCookie(new WebSocketSharp.Net.Cookie("Authorization", accessToken));

        webSocket.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket connection opened.");

            // 연결 성공 시 greeting message를 요청
            StartCoroutine(apiManager.GetGreetingMessage(UserDataManager.Instance.MatchRoomID.ToString()));
        };

        webSocket.OnMessage += (sender, e) =>
        {
            Debug.Log($"Received message: {e.Data}");
            // 여기서 메시지 처리 로직을 추가하세요.
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
            // JSON 파싱 및 결과 처리 로직을 추가하세요.
            Debug.Log(jsonResult);
        }
        else
        {
            Debug.LogError($"API request failed: {request.error}");
        }
    }
}
