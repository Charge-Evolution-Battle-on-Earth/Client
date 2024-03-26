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
                Debug.Log("�����κ��� �޽��� ����: " + message);

                // ���� JSON �����͸� JsonResponseData ��ü�� ������ȭ�Ͽ� �ʿ��� �۾� ����
                JsonResponseData responseData = JsonUtility.FromJson<JsonResponseData>(message);
                // ���� ���� ������ ó��
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
