using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using Newtonsoft.Json;
using System.Collections.Generic;

public class StompManager : MonoBehaviour
{
    public Button skillButton1;
    public Button skillButton2;
    public Button skillButton3;
    public Button skillButton4;

    private WebSocket webSocket;

    void Start()
    {
        // WebSocket ���� �ּҿ� �°� ���� (wss ���)
        string serverAddress = "wss://cebone.shop:443/play/";

        // WebSocket ����
        webSocket = new WebSocket(serverAddress);
        webSocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12; // TLS 1.2 ��� (�ʿ信 ���� ����)

        webSocket.OnOpen += OnWebSocketOpen;
        webSocket.OnMessage += OnWebSocketMessage;
        webSocket.OnClose += OnWebSocketClose;

        webSocket.Connect();

        // �� ��ư�� ���� Ŭ�� �̺�Ʈ �߰�
        skillButton1.onClick.AddListener(() => OnSkillButtonClick(1));
        skillButton2.onClick.AddListener(() => OnSkillButtonClick(2));
        skillButton3.onClick.AddListener(() => OnSkillButtonClick(3));
        skillButton4.onClick.AddListener(() => OnSkillButtonClick(4));
    }

    void OnSkillButtonClick(long skillId)
    {
        // skillId�� ������ /play/game/turn/{matchId} API�� ȣ��
        RequestGameTurn(skillId);
    }

    void RequestGameTurn(long skillId)
    {
        // matchId�� ������ ���� �־ ��û
        string matchId = UserDataManager.Instance.MatchRoomID.ToString(); // ������ matchId ������ ����

        // Request Body�� skillId �����Ͽ� STOMP �������� �޽��� ����
        string requestJson = $"SEND\n" +
                             $"destination:/play/game/turn/{matchId}\n" +
                             $"content-type:application/json\n\n" +
                             $"{{\"skillId\": {skillId}}}\x00";

        // ������ �޽��� ����
        webSocket.Send(requestJson);
    }

    void OnWebSocketOpen(object sender, System.EventArgs e)
    {
        Debug.Log("WebSocket connection opened");

        // WebSocket�� ������ STOMP ������ ����
        ConnectStomp();
        RequestGreetingMessage();
        RequestGameStart();
        RequestGameEnd();
        RequestSurrender();
        RequestQuitGame();
    }

    void ConnectStomp()
    {
        // STOMP ���ῡ ����� ��� ����
        var authHeaders = new System.Collections.Generic.Dictionary<string, string>
        {
            { "Authorization", UserDataManager.Instance.AccessToken },
            { "MatchInfo", UserDataManager.Instance.MatchRoomID.ToString() }
        };

        // STOMP CONNECT ������ ����
        string stompConnect = $"CONNECT\n" +
                              $"accept-version:1.2\n" +
                              $"heart-beat:10000,10000\n";

        foreach (var header in authHeaders)
        {
            stompConnect += $"{header.Key}:{header.Value}\n";
        }

        stompConnect += "\n" + "\x00"; // STOMP ������ ���� ��Ÿ���� ��(0) ����Ʈ

        // ������ STOMP CONNECT �������� ������ ����
        webSocket.Send(stompConnect);
    }

    void RequestGreetingMessage()
    {
        // matchId�� ������ ���� �־ ��û
        string matchId = UserDataManager.Instance.MatchRoomID.ToString(); // ������ matchId ������ ����
        webSocket.Send($"GET /play/greeting/{matchId} HTTP/1.1\r\nHost: cebone.shop\r\n\r\n");
    }

    void RequestGameStart()
    {
        // ���� ���� ��û �޽��� ����
        string requestJson = $"SEND\ndestination:/play/game/start/{UserDataManager.Instance.MatchRoomID}\n\n\x00";

        // ������ �޽��� ����
        webSocket.Send(requestJson);
    }

    void RequestGameEnd()
    {
        string matchId = UserDataManager.Instance.MatchRoomID.ToString();

        // Request Body�� ���� ��쿡�� STOMP ���Ŀ� �°� ���� �� �ֵ��� ����
        webSocket.Send($"SEND\n" +
                       $"destination:/play/game/end/{matchId}\n" +
                       $"content-type:application/json\n\n" +
                       "{}\x00");
    }

    void RequestSurrender()
    {
        string matchId = UserDataManager.Instance.MatchRoomID.ToString();

        // Request Body�� ���� ��쿡�� STOMP ���Ŀ� �°� ���� �� �ֵ��� ����
        webSocket.Send($"SEND\n" +
                       $"destination:/play/game/surrender/{matchId}\n" +
                       $"content-type:application/json\n\n" +
                       "{}\x00");
    }

    void RequestQuitGame()
    {
        string matchId = UserDataManager.Instance.MatchRoomID.ToString();

        // Request Body�� ���� ��쿡�� STOMP ���Ŀ� �°� ���� �� �ֵ��� ����
        webSocket.Send($"SEND\n" +
                       $"destination:/play/game/quit/{matchId}\n" +
                       $"content-type:application/json\n\n" +
                       "{}\x00");
    }

    void OnWebSocketMessage(object sender, MessageEventArgs e)
    {
        string message = e.Data;

        // STOMP ������ �и�
        string[] stompFrames = message.Split('\x00');

        foreach (string stompFrame in stompFrames)
        {
            if (!string.IsNullOrEmpty(stompFrame))
            {
                // STOMP ������ ó��
                ProcessStompFrame(stompFrame);
            }
        }
    }

    void ProcessStompFrame(string stompFrame)
    {
        // ���⿡���� �����ϰ� �ֿܼ� ����ϴ� ���� �ڵ�
        Debug.Log($"Received STOMP frame: {stompFrame}");

        // Greeting �޽��� ó��
        if (stompFrame.Contains("/play/greeting"))
        {
            GreetingResponse greetingResponse = JsonConvert.DeserializeObject<GreetingResponse>(stompFrame);

            // TODO: ó�� ���� �߰�
            Debug.Log($"Received greeting message: {greetingResponse.GreetingMessage}");
        }
        // ���� ���� ���� ó��
        else if (stompFrame.Contains("game/start"))
        {
            StartGameResponse startGameResponse = JsonConvert.DeserializeObject<StartGameResponse>(stompFrame);

            // TODO: ���� ���� ���� ó�� ���� �߰�
            Debug.Log($"Game started. Host total stat: {startGameResponse.HostTotalStat}, Entrant total stat: {startGameResponse.EntrantTotalStat}");
        }
        // ���� �� ����(��ų ���) ���� ó��
        else if (stompFrame.Contains("game/turn"))
        {
            TurnResponse turnResponse = JsonConvert.DeserializeObject<TurnResponse>(stompFrame);

            // TODO: ���� �� ���� ���� ó�� ���� �߰�
            Debug.Log($"Turn performed. Turn owner: {turnResponse.TurnOwner}, Use skill name: {turnResponse.UseSkillNm}");
        }
        // Game End �޽��� ó��
        else if (stompFrame.Contains("/play/game/end"))
        {
            GameEndResponse gameEndResponse = JsonConvert.DeserializeObject<GameEndResponse>(stompFrame);

            // TODO: ó�� ���� �߰�
            Debug.Log($"Game ended. Winner: {gameEndResponse.WinnerType}, Loser: {gameEndResponse.LoserType}");
        }
        // Surrender �޽��� ó��
        else if (stompFrame.Contains("/play/game/surrender"))
        {
            SurrenderResponse surrenderResponse = JsonConvert.DeserializeObject<SurrenderResponse>(stompFrame);

            // TODO: ó�� ���� �߰�
            Debug.Log($"Surrendered. Winner: {surrenderResponse.WinnerType}, Loser: {surrenderResponse.LoserType}");
        }
        // QuitGame �޽��� ó��
        else if (stompFrame.Contains("/play/game/quit"))
        {
            QuitGameResponse quitGameResponse = JsonConvert.DeserializeObject<QuitGameResponse>(stompFrame);

            // TODO: ó�� ���� �߰�
            Debug.Log($"Quit game. Player Type: {quitGameResponse.PlayerType}");
        }
    }

    void OnWebSocketClose(object sender, CloseEventArgs e)
    {
        Debug.Log("WebSocket connection closed");
    }

    void OnDestroy()
    {
        if (webSocket != null && webSocket.ReadyState == WebSocketState.Open)
        {
            webSocket.Close();
        }
    }
}
// ���� ���� ���信 ���� ������ Ŭ����
public class StartGameResponse
{
    public Stat HostTotalStat { get; set; }
    public Stat EntrantTotalStat { get; set; }
    public List<CharacterSkillGetResponse> HostSkillList { get; set; }
    public List<CharacterSkillGetResponse> EntrantSkillList { get; set; }
    public playerType TurnOwner { get; set; }
    public matchStatus MatchStatus { get; set; }
    public string Message { get; set; }
}

// Greeting ���信 ���� ������ Ŭ����
public class GreetingResponse
{
    public string GreetingMessage { get; set; }
}

// ���� �� ����(��ų ���) ��û�� ���� ������ Ŭ����
public class TurnRequest
{
    public long SkillId { get; set; }
}

// ���� �� ����(��ų ���) ���信 ���� ������ Ŭ����
public class TurnResponse
{
    public bool IsGameOver { get; set; }
    public Stat HostStat { get; set; }
    public Stat EntrantStat { get; set; }
    public playerType TurnOwner { get; set; }
    public string UseSkillNm { get; set; }
    public string Message { get; set; }
}

public class CharacterSkillGetResponse
{
    public long skillId;
    public string skillNm;
    public string description;
}

public enum playerType
{
    // playerType ������ ����
}

// Game End ���信 ���� ������ Ŭ����
public class GameEndResponse
{
    public playerType WinnerType { get; set; }
    public playerType LoserType { get; set; }
    public int WinnerGold { get; set; }
    public int LoserGold { get; set; }
    public int WinnerExp { get; set; }
    public int LoserExp { get; set; }
    public int WinnerTotalGold { get; set; }
    public int LoserTotalGold { get; set; }
    public int WinnerTotalExp { get; set; }
    public int LoserTotalExp { get; set; }
    public string Message { get; set; }
}

// Surrender ���信 ���� ������ Ŭ����
public class SurrenderResponse
{
    public playerType WinnerType { get; set; }
    public playerType LoserType { get; set; }
    public int WinnerGold { get; set; }
    public int LoserGold { get; set; }
    public int WinnerExp { get; set; }
    public int LoserExp { get; set; }
    public int WinnerTotalGold { get; set; }
    public int LoserTotalGold { get; set; }
    public int WinnerTotalExp { get; set; }
    public int LoserTotalExp { get; set; }
    public string Message { get; set; }
}

// QuitGame ���信 ���� ������ Ŭ����
public class QuitGameResponse
{
    public playerType PlayerType { get; set; }
    public string Message { get; set; }
}