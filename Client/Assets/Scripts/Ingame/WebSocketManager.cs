using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class WebSocketManager : MonoBehaviour
{
    private static WebSocketManager instance;
    private ClientWebSocket ws;
    private Uri serverUri;

    public static WebSocketManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<WebSocketManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("WebSocketManager");
                    instance = singletonObject.AddComponent<WebSocketManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public async Task ConnectWebSocket(Uri serverUri)
    {
        this.serverUri = serverUri;
        ws = new ClientWebSocket();
        ws.Options.SetRequestHeader("Authorization", "Bearer " + UserDataManager.Instance.AccessToken);

        try
        {
            await ws.ConnectAsync(serverUri, CancellationToken.None);
            Debug.Log("WebSocket 연결 성공");

            ReceiveMessages();
        }
        catch (Exception ex)
        {
            Debug.LogError("WebSocket 연결 실패: " + ex.Message);
        }
    }

    public async Task SendJsonRequest<T>(T data)
    {
        if (ws != null && ws.State == WebSocketState.Open)
        {
            try
            {
                string jsonData = JsonUtility.ToJson(data);
                byte[] requestBytes = Encoding.UTF8.GetBytes(jsonData);
                await ws.SendAsync(new ArraySegment<byte>(requestBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                Debug.Log("JSON 요청 보냄: " + jsonData);
            }
            catch (Exception ex)
            {
                Debug.LogError("JSON 요청 보내기 실패: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("WebSocket이 연결되어 있지 않습니다.");
        }
    }

    private async Task ReceiveMessages()
    {
        try
        {
            byte[] buffer = new byte[1024];
            WebSocketReceiveResult result;
            while (ws != null && ws.State == WebSocketState.Open)
            {
                result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Debug.Log("서버로부터 메시지 수신: " + message);

                // 받은 JSON 데이터를 JsonResponseData 객체로 역직렬화하여 필요한 작업 수행
                JsonResponseData responseData = JsonUtility.FromJson<JsonResponseData>(message);
                // 받은 응답 데이터 처리
            }

        }
        catch (Exception ex)
        {
            Debug.LogError("메시지 수신 중 오류 발생: " + ex.Message);
        }
    }

    public async Task DisconnectWebSocket()
    {
        if (ws != null && ws.State == WebSocketState.Open)
        {
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed by client", CancellationToken.None);
            Debug.Log("WebSocket 연결 종료");
        }
    }
}

[Serializable]
public class JsonRequestData
{
    public string command;
    public long matchId;
}

[Serializable]
public class JsonResponseData
{
    public string greetingMessage;
}
