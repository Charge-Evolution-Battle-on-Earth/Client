using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
public class Ingame : MonoBehaviour
{
    public Button startBtn;
    public Button quitBtn;
    public Button surrenderBtn;
    public Button readyBtn;
    public Image readyButtonImage;

    private WebSocketManager webSocketManager;
    private async void Start()
    {
        webSocketManager = WebSocketManager.Instance;

        Uri serverUri = new Uri(GameURL.DBServer.PlayURL);

        await webSocketManager.ConnectWebSocket(serverUri);

        Greeting();

    }
    private async void Update()
    {
        if (UserDataManager.Instance.IsReady)
        {
            if (UserDataManager.Instance.IsReady == UserDataManager.Instance.OpponentIsReady)
            {
                startBtn.interactable = true;
            }
        }
    }
    public void ReadyBtn()
    {
        if (UserDataManager.Instance.IsReady)
        {
            UserDataManager.Instance.IsReady = false;
        }
        else
        {
            UserDataManager.Instance.IsReady = true;
        }

        Ready();
    }

    public void StartBtn()
    {
        GameStart();
    }

    public void SkillBtn(long skillId)
    {
        Turn(skillId);
    }

    async void Greeting()
    {
        GreetingJson requestData = new GreetingJson();
        requestData.command = "GREETING";
        requestData.matchId = UserDataManager.Instance.MatchRoomID;
        EmptyRequest emptyRequest = new EmptyRequest();
        requestData.request = emptyRequest;
        await webSocketManager.SendJsonRequest(requestData);
    }

    async void Ready()
    {
        ReadyJson requestData = new ReadyJson();
        requestData.command = "READY";
        requestData.matchId = UserDataManager.Instance.MatchRoomID;
        requestData.request.selfReadyStatus = UserDataManager.Instance.IsReady;
        requestData.request.opponentReadyStatus = UserDataManager.Instance.OpponentIsReady;
        await webSocketManager.SendJsonRequest(requestData);
    }

    async void GameStart()
    {
        StartJson requestData = new StartJson();
        requestData.command = "START";
        requestData.matchId = UserDataManager.Instance.MatchRoomID;
        EmptyRequest emptyRequest = new EmptyRequest();
        requestData.request = emptyRequest;
        await webSocketManager.SendJsonRequest(requestData);
    }

    async void Turn(long skillId)
    {
        TurnJson requestData = new TurnJson();
        requestData.command = "TURN_GAME";
        requestData.matchId = UserDataManager.Instance.MatchRoomID;
        requestData.request.skillId = skillId;
        await webSocketManager.SendJsonRequest(requestData);
    }


}
