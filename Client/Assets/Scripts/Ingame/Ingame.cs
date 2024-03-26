using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
public class Ingame : MonoBehaviour
{
    public Button quitBtn;
    public Button surrenderBtn;
    public Image readyButtonImage;

    private WebSocketManager webSocketManager;
    private async void Start()
    {
        webSocketManager = WebSocketManager.Instance;

        Uri serverUri = new Uri(GameURL.DBServer.PlayURL);

        await webSocketManager.ConnectWebSocket(serverUri);

        Greeting(UserDataManager.Instance.MatchRoomID);

    }

    async void Greeting(long matchId)
    {
        JsonRequestData requestData = new JsonRequestData();
        requestData.command = "GREETING";
        //requestData.matchId = matchId;
        await webSocketManager.SendJsonRequest(requestData);
    }
}
