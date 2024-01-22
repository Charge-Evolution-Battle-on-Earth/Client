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

        // 웹소켓 URL에 토큰을 쿼리 매개변수로 추가
        string socketAndApiUrl = $"{socketUrl}?Authorization: 'Bearer {accessToken}'";

        // 서버 인증서를 확인하기 위해 CertificateHandler 사용
        UnityWebRequest www = UnityWebRequest.Get(gameUrl+"/play");
        www.certificateHandler = new AcceptAllCertificates();

        www.SendWebRequest().completed += (asyncOperation) =>
        {
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("서버 인증서가 유효합니다.");

                // 서버 인증서가 유효하면 웹소켓 연결 생성
                webSocket = new WebSocket(socketAndApiUrl);

                webSocket.OnOpen += OnWebSocketOpen;
                webSocket.OnMessage += OnWebSocketMessage;
                webSocket.OnError += OnWebSocketError;

                webSocket.Connect();
            }
            else
            {
                Debug.LogError($"서버 인증서 유효성 검사 실패: {www.error}");
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
        Debug.LogError($"WebSocket 에러: {e.Message}");
    }

    private void OnWebSocketOpen(object sender, EventArgs e)
    {
        Debug.Log("웹소켓 연결이 열렸습니다.");

        // 연결 성공 후 인사 메시지 요청
        StartCoroutine(apiManager.GetGreetingMessage(UserDataManager.Instance.MatchRoomID.ToString()));
    }

    private void OnWebSocketMessage(object sender, MessageEventArgs e)
    {
        Debug.Log($"받은 메시지: {e.Data}");
        // 메시지 처리 로직 추가
    }
}

// AcceptAllCertificates 클래스 정의
public class AcceptAllCertificates : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // 모든 인증서를 허용하려면 true를 반환
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
            // JSON 파싱 및 결과 처리 로직 추가
            Debug.Log(jsonResult);
        }
        else
        {
            Debug.LogError($"API 요청 실패: {request.error}");
        }
    }
}
