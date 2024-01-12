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
        // WebSocket 서버 주소에 맞게 수정 (wss 사용)
        string serverAddress = "wss://cebone.shop:443/play/";

        // WebSocket 연결
        webSocket = new WebSocket(serverAddress);
        webSocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12; // TLS 1.2 사용 (필요에 따라 변경)

        webSocket.OnOpen += OnWebSocketOpen;
        webSocket.OnMessage += OnWebSocketMessage;
        webSocket.OnClose += OnWebSocketClose;

        webSocket.Connect();

        // 각 버튼에 대한 클릭 이벤트 추가
        skillButton1.onClick.AddListener(() => OnSkillButtonClick(1));
        skillButton2.onClick.AddListener(() => OnSkillButtonClick(2));
        skillButton3.onClick.AddListener(() => OnSkillButtonClick(3));
        skillButton4.onClick.AddListener(() => OnSkillButtonClick(4));
    }

    void OnSkillButtonClick(long skillId)
    {
        // skillId를 가지고 /play/game/turn/{matchId} API를 호출
        RequestGameTurn(skillId);
    }

    void RequestGameTurn(long skillId)
    {
        // matchId에 적절한 값을 넣어서 요청
        string matchId = UserDataManager.Instance.MatchRoomID.ToString(); // 적절한 matchId 값으로 변경

        // Request Body에 skillId 포함하여 STOMP 형식으로 메시지 생성
        string requestJson = $"SEND\n" +
                             $"destination:/play/game/turn/{matchId}\n" +
                             $"content-type:application/json\n\n" +
                             $"{{\"skillId\": {skillId}}}\x00";

        // 서버로 메시지 전송
        webSocket.Send(requestJson);
    }

    void OnWebSocketOpen(object sender, System.EventArgs e)
    {
        Debug.Log("WebSocket connection opened");

        // WebSocket이 열리면 STOMP 연결을 수행
        ConnectStomp();
        RequestGreetingMessage();
        RequestGameStart();
        RequestGameEnd();
        RequestSurrender();
        RequestQuitGame();
    }

    void ConnectStomp()
    {
        // STOMP 연결에 사용할 헤더 정보
        var authHeaders = new System.Collections.Generic.Dictionary<string, string>
        {
            { "Authorization", UserDataManager.Instance.AccessToken },
            { "MatchInfo", UserDataManager.Instance.MatchRoomID.ToString() }
        };

        // STOMP CONNECT 프레임 생성
        string stompConnect = $"CONNECT\n" +
                              $"accept-version:1.2\n" +
                              $"heart-beat:10000,10000\n";

        foreach (var header in authHeaders)
        {
            stompConnect += $"{header.Key}:{header.Value}\n";
        }

        stompConnect += "\n" + "\x00"; // STOMP 프레임 끝을 나타내는 널(0) 바이트

        // 생성된 STOMP CONNECT 프레임을 서버로 전송
        webSocket.Send(stompConnect);
    }

    void RequestGreetingMessage()
    {
        // matchId에 적절한 값을 넣어서 요청
        string matchId = UserDataManager.Instance.MatchRoomID.ToString(); // 적절한 matchId 값으로 변경
        webSocket.Send($"GET /play/greeting/{matchId} HTTP/1.1\r\nHost: cebone.shop\r\n\r\n");
    }

    void RequestGameStart()
    {
        // 게임 시작 요청 메시지 생성
        string requestJson = $"SEND\ndestination:/play/game/start/{UserDataManager.Instance.MatchRoomID}\n\n\x00";

        // 서버로 메시지 전송
        webSocket.Send(requestJson);
    }

    void RequestGameEnd()
    {
        string matchId = UserDataManager.Instance.MatchRoomID.ToString();

        // Request Body가 없는 경우에도 STOMP 형식에 맞게 보낼 수 있도록 수정
        webSocket.Send($"SEND\n" +
                       $"destination:/play/game/end/{matchId}\n" +
                       $"content-type:application/json\n\n" +
                       "{}\x00");
    }

    void RequestSurrender()
    {
        string matchId = UserDataManager.Instance.MatchRoomID.ToString();

        // Request Body가 없는 경우에도 STOMP 형식에 맞게 보낼 수 있도록 수정
        webSocket.Send($"SEND\n" +
                       $"destination:/play/game/surrender/{matchId}\n" +
                       $"content-type:application/json\n\n" +
                       "{}\x00");
    }

    void RequestQuitGame()
    {
        string matchId = UserDataManager.Instance.MatchRoomID.ToString();

        // Request Body가 없는 경우에도 STOMP 형식에 맞게 보낼 수 있도록 수정
        webSocket.Send($"SEND\n" +
                       $"destination:/play/game/quit/{matchId}\n" +
                       $"content-type:application/json\n\n" +
                       "{}\x00");
    }

    void OnWebSocketMessage(object sender, MessageEventArgs e)
    {
        string message = e.Data;

        // STOMP 프레임 분리
        string[] stompFrames = message.Split('\x00');

        foreach (string stompFrame in stompFrames)
        {
            if (!string.IsNullOrEmpty(stompFrame))
            {
                // STOMP 프레임 처리
                ProcessStompFrame(stompFrame);
            }
        }
    }

    void ProcessStompFrame(string stompFrame)
    {
        // 여기에서는 간단하게 콘솔에 출력하는 예제 코드
        Debug.Log($"Received STOMP frame: {stompFrame}");

        // Greeting 메시지 처리
        if (stompFrame.Contains("/play/greeting"))
        {
            GreetingResponse greetingResponse = JsonConvert.DeserializeObject<GreetingResponse>(stompFrame);

            // TODO: 처리 로직 추가
            Debug.Log($"Received greeting message: {greetingResponse.GreetingMessage}");
        }
        // 게임 시작 응답 처리
        else if (stompFrame.Contains("game/start"))
        {
            StartGameResponse startGameResponse = JsonConvert.DeserializeObject<StartGameResponse>(stompFrame);

            // TODO: 게임 시작 후의 처리 로직 추가
            Debug.Log($"Game started. Host total stat: {startGameResponse.HostTotalStat}, Entrant total stat: {startGameResponse.EntrantTotalStat}");
        }
        // 게임 턴 수행(스킬 사용) 응답 처리
        else if (stompFrame.Contains("game/turn"))
        {
            TurnResponse turnResponse = JsonConvert.DeserializeObject<TurnResponse>(stompFrame);

            // TODO: 게임 턴 수행 후의 처리 로직 추가
            Debug.Log($"Turn performed. Turn owner: {turnResponse.TurnOwner}, Use skill name: {turnResponse.UseSkillNm}");
        }
        // Game End 메시지 처리
        else if (stompFrame.Contains("/play/game/end"))
        {
            GameEndResponse gameEndResponse = JsonConvert.DeserializeObject<GameEndResponse>(stompFrame);

            // TODO: 처리 로직 추가
            Debug.Log($"Game ended. Winner: {gameEndResponse.WinnerType}, Loser: {gameEndResponse.LoserType}");
        }
        // Surrender 메시지 처리
        else if (stompFrame.Contains("/play/game/surrender"))
        {
            SurrenderResponse surrenderResponse = JsonConvert.DeserializeObject<SurrenderResponse>(stompFrame);

            // TODO: 처리 로직 추가
            Debug.Log($"Surrendered. Winner: {surrenderResponse.WinnerType}, Loser: {surrenderResponse.LoserType}");
        }
        // QuitGame 메시지 처리
        else if (stompFrame.Contains("/play/game/quit"))
        {
            QuitGameResponse quitGameResponse = JsonConvert.DeserializeObject<QuitGameResponse>(stompFrame);

            // TODO: 처리 로직 추가
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
// 게임 시작 응답에 대한 데이터 클래스
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

// Greeting 응답에 대한 데이터 클래스
public class GreetingResponse
{
    public string GreetingMessage { get; set; }
}

// 게임 턴 수행(스킬 사용) 요청에 대한 데이터 클래스
public class TurnRequest
{
    public long SkillId { get; set; }
}

// 게임 턴 수행(스킬 사용) 응답에 대한 데이터 클래스
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
    // playerType 열거형 구현
}

// Game End 응답에 대한 데이터 클래스
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

// Surrender 응답에 대한 데이터 클래스
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

// QuitGame 응답에 대한 데이터 클래스
public class QuitGameResponse
{
    public playerType PlayerType { get; set; }
    public string Message { get; set; }
}