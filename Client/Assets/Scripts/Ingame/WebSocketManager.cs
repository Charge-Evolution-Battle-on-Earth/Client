using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
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
            Debug.Log("WebSocket ���� ����");

            ReceiveMessages();
        }
        catch (Exception ex)
        {
            Debug.LogError("WebSocket ���� ����: " + ex.Message);
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
                Debug.Log("JSON ��û ����: " + jsonData);
            }
            catch (Exception ex)
            {
                Debug.LogError("JSON ��û ������ ����: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("WebSocket�� ����Ǿ� ���� �ʽ��ϴ�.");
        }
    }

    async Task ReceiveMessages()
    {
        try
        {
            byte[] buffer = new byte[1024];
            while (ws != null && ws.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    if (!string.IsNullOrEmpty(message))
                    {
                        Debug.Log("�����κ��� �޽��� ����: " + message);
                        var jsonData = JsonConvert.DeserializeObject<Dictionary<string, object>>(message);

                        try
                        {
                            if (jsonData.ContainsKey("greetingMessage"))
                            {
                                string greetingMessage = jsonData["greetingMessage"].ToString();
                                Debug.Log(greetingMessage);
                            }
                            else if (jsonData.ContainsKey("hostReadyStatus"))
                            {
                                bool hostReadyStatus = Convert.ToBoolean(jsonData["hostReadyStatus"]);
                                bool entrantReadyStatus = Convert.ToBoolean(jsonData["entrantReadyStatus"]);
                                string matchStatus = Convert.ToString(jsonData["matchStatus"]);
                                
                                if(UserDataManager.Instance.UserId==UserDataManager.Instance.HostId)
                                {
                                    UserDataManager.Instance.IsReady = hostReadyStatus;
                                    UserDataManager.Instance.OpponentIsReady = entrantReadyStatus;
                                }
                                else
                                {
                                    UserDataManager.Instance.IsReady = entrantReadyStatus;
                                    UserDataManager.Instance.OpponentIsReady = hostReadyStatus;
                                }
                                UserDataManager.Instance.MatchStatus = (MatchStatus)Enum.Parse(typeof(MatchStatus), matchStatus);
                            }
                            else if (jsonData.ContainsKey("hostTotalStat"))
                            {
                                // start ����
                            }
                            else if (jsonData.ContainsKey("isGameOver"))
                            {
                                // turn ����
                            }
                            else if (jsonData.ContainsKey("winnerType"))
                            {
                                // end�� surrender ����
                            }
                            else if (jsonData.ContainsKey("playerType"))
                            {
                                // quit ����
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError("JSON �Ľ� ����: " + ex.Message);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("������ �޽����� ��� �ֽ��ϴ�.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("�޽��� ���� �� ���� �߻�: " + ex.Message);
        }
    }


    public async Task DisconnectWebSocket()
    {
        if (ws != null && ws.State == WebSocketState.Open)
        {
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed by client", CancellationToken.None);
            Debug.Log("WebSocket ���� ����");
        }
    }
}
[Serializable]
public class RequestJson
{
    public string command;
    public long matchId;
    public EmptyRequest request = new EmptyRequest();
}
[Serializable]
public class EmptyRequest
{

}
[Serializable]
public class ReadyJson
{
    public string command;
    public long matchId;
    public ReadyRequest request = new ReadyRequest();
}
[Serializable]
public class ReadyRequest
{
    public bool selfReadyStatus;
    public bool opponentReadyStatus;
}
[Serializable]
public class StartJson
{
    public string command;
    public long matchId;
    public EmptyRequest request = new EmptyRequest();
}
[Serializable]
public class TurnJson
{
    public string command;
    public long matchId;
    public TurnRequest request = new TurnRequest();
}
[Serializable]
public class TurnRequest
{
    public long skillId;
}