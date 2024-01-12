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

        // WebSocket ���� �ּҿ� �°� ���� (wss ���)
        string serverAddress = "wss://cebone.shop:443/play";
        webSocket = new WebSocket(serverAddress);
        webSocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;

        webSocket.OnOpen += OnWebSocketOpen;
        webSocket.OnClose += OnWebSocketClose;
        webSocket.Connect();

        // ��ư Ŭ�� �̺�Ʈ�� ���� ������ ���
        GetComponent<Button>().onClick.AddListener(OnReadyButtonClick);
    }

    void OnWebSocketOpen(object sender, System.EventArgs e)
    {
        Debug.Log("WebSocket connection opened");

        // STOMP ������ ����
        ConnectStomp();

        // �ʱ� ���¿��� selfReadyStatus�� ����
        SendReadyStatus();
    }

    void ConnectStomp()
    {
        // STOMP ���ῡ ����� ��� ����
        var authHeaders = new System.Collections.Generic.Dictionary<string, string>
        {
            { "Authorization", UserDataManager.Instance.AccessToken },
            { "MatchInfo", matchId }
        };

        // STOMP CONNECT ������ ����
        string stompConnect = $"CONNECT\n" +
                              $"accept-version:1.1\n" +
                              $"heart-beat:10000,10000\n";

        foreach (var header in authHeaders)
        {
            stompConnect += $"{header.Key}:{header.Value}\n";
        }

        stompConnect += "\n" + "\x00"; // STOMP ������ ���� ��Ÿ���� ��(0) ����Ʈ

        // ������ STOMP CONNECT �������� ������ ����
        webSocket.Send(stompConnect);
    }

    void OnReadyButtonClick()
    {
        // ��ư�� Ŭ���Ǹ� selfReadyStatus ����
        selfReadyStatus = !selfReadyStatus;

        // STOMP �������� �����Ͽ� �غ� ���� ����
        SendReadyStatus();
    }

    void SendReadyStatus()
    {
        // STOMP ������ ����
        string stompFrame = $"SEND\n" +
                            $"destination:/play/ready/{matchId}\n" +
                            $"content-type:application/json\n\n" +
                            $"{{\"selfReadyStatus\":{(selfReadyStatus ? "true" : "false")},\"opponentReadyStatus\":false}}\x00";

        // ������ STOMP �������� ������ ����
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
