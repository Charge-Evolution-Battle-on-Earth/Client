using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class ReadyButton : MonoBehaviour
{
    private WebSocket webSocket;
    private string matchId;
    private bool selfReadyStatus = false;

    void Start()
    {
        matchId = UserDataManager.Instance.MatchRoomID.ToString();

        // WebSocket 서버 주소에 맞게 수정 (wss 사용)
        string serverAddress = "wss://cebone.shop:443/play";
        webSocket = new WebSocket(serverAddress);
        webSocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;

        webSocket.OnOpen += OnWebSocketOpen;
        webSocket.OnClose += OnWebSocketClose;
        webSocket.Connect();

        // 버튼 클릭 이벤트에 대한 리스너 등록
        GetComponent<Button>().onClick.AddListener(OnReadyButtonClick);
    }

    void OnWebSocketOpen(object sender, System.EventArgs e)
    {
        Debug.Log("WebSocket connection opened");

        // STOMP 연결을 수행
        ConnectStomp();

        // 초기 상태에서 selfReadyStatus를 전송
        SendReadyStatus();
    }

    void ConnectStomp()
    {
        // STOMP 연결에 사용할 헤더 정보
        var authHeaders = new System.Collections.Generic.Dictionary<string, string>
        {
            { "Authorization", UserDataManager.Instance.AccessToken },
            { "MatchInfo", matchId }
        };

        // STOMP CONNECT 프레임 생성
        string stompConnect = $"CONNECT\n" +
                              $"accept-version:1.1\n" +
                              $"heart-beat:10000,10000\n";

        foreach (var header in authHeaders)
        {
            stompConnect += $"{header.Key}:{header.Value}\n";
        }

        stompConnect += "\n" + "\x00"; // STOMP 프레임 끝을 나타내는 널(0) 바이트

        // 생성된 STOMP CONNECT 프레임을 서버로 전송
        webSocket.Send(stompConnect);
    }

    void OnReadyButtonClick()
    {
        // 버튼이 클릭되면 selfReadyStatus 변경
        selfReadyStatus = !selfReadyStatus;

        // STOMP 프레임을 생성하여 준비 상태 전송
        SendReadyStatus();
    }

    void SendReadyStatus()
    {
        // STOMP 프레임 생성
        string stompFrame = $"SEND\n" +
                            $"destination:/play/ready/{matchId}\n" +
                            $"content-type:application/json\n\n" +
                            $"{{\"selfReadyStatus\":{(selfReadyStatus ? "true" : "false")},\"opponentReadyStatus\":false}}\x00";

        // 생성된 STOMP 프레임을 서버로 전송
        webSocket.Send(stompFrame);
    }

    void OnWebSocketClose(object sender, CloseEventArgs e)
    {
        Debug.Log($"WebSocket connection closed. Code: {e.Code}, Reason: {e.Reason}");
    }

    void OnDestroy()
    {
        if (webSocket != null && webSocket.ReadyState == WebSocketState.Open)
        {
            webSocket.Close();
        }
    }
}
