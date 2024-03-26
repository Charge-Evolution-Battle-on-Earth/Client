using UnityEngine;
using WebSocketSharp;

public class WebSocketClient : MonoBehaviour
{
    private WebSocket ws;

    void Start()
    {
        Debug.Log("START");
        ws = new WebSocket("ws://43.202.117.184:3000/socket.io");
        ws.OnOpen += Ws_OnOpen;
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Received message: " + e.Data);
        };
        ws.Connect();
    }

    private void Ws_OnOpen(object sender, System.EventArgs e)
    {
        Debug.Log("Socket Open");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ws.Send("Hello from Unity!");
        }
    }

    void OnDestroy()
    {
        if (ws != null && ws.IsAlive)
        {
            ws.Close();
        }
    }
}
