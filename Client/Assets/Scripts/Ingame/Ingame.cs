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
    private void Update()
    {
        if (UserDataManager.Instance.HostReady && UserDataManager.Instance.EntrantReady && UserDataManager.Instance.HostId == UserDataManager.Instance.UserId)
        {
            startBtn.interactable = true;
        }
        else
        {
            startBtn.interactable = false;
        }

        if (UserDataManager.Instance.MatchStatus == MatchStatus.IN_PROGRESS || UserDataManager.Instance.MatchStatus == MatchStatus.FINISHED)
        {
            surrenderBtn.gameObject.SetActive(true);
            surrenderBtn.interactable = true;
            startBtn.interactable = false;
            startBtn.gameObject.SetActive(false);
        }
        else
        {
            surrenderBtn.interactable = false;
            surrenderBtn.gameObject.SetActive(false);
            startBtn.gameObject.SetActive(true);
        }

        if (UserDataManager.Instance.MatchStatus == MatchStatus.READY || UserDataManager.Instance.MatchStatus == MatchStatus.IN_PROGRESS)
        {
            quitBtn.interactable = false;
        }
        else
        {
            quitBtn.interactable = true;
        }

        if(UserDataManager.Instance.IsGameOver)
        {
            End();
        }
    }
    public void ReadyBtn()
    {
        if (UserDataManager.Instance.PlayerType == PlayerType.HOST)
        {
            UserDataManager.Instance.HostReady = !UserDataManager.Instance.HostReady;
        }
        else if (UserDataManager.Instance.PlayerType == PlayerType.ENTRANT)
        {
            UserDataManager.Instance.EntrantReady = !UserDataManager.Instance.EntrantReady;
        }
        Ready();
    }

    public void StartBtn()
    {
        GameStart();
    }

    public void SkillBtn(int skillId)
    {
        long skillID = new long();
        if(UserDataManager.Instance.PlayerType == PlayerType.HOST)
        {
            skillID = (long)UserDataManager.Instance.HostSkillList[skillId].skillId;
        }
        else if(UserDataManager.Instance.PlayerType == PlayerType.ENTRANT)
        {
            skillID = (long)UserDataManager.Instance.EntrantSkillList[skillId].skillId;
        }
        Turn(skillID);
    }

    public void QuitBtn()
    {
        Quit();
    }

    public void SurrenderBtn()
    {
        Surrender();
    }

    async void Greeting()
    {
        RequestJson requestData = new RequestJson();
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
        if(UserDataManager.Instance.UserId == UserDataManager.Instance.HostId)
        {
            requestData.request.hostReadyStatus = !UserDataManager.Instance.HostReady;
            requestData.request.entrantReadyStatus = UserDataManager.Instance.EntrantReady;
        }
        else if(UserDataManager.Instance.UserId==UserDataManager.Instance.EntrantId)
        {
            requestData.request.hostReadyStatus = UserDataManager.Instance.HostReady;
            requestData.request.entrantReadyStatus = !UserDataManager.Instance.EntrantReady;
        }
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

    async void End()
    {
        RequestJson requestData = new RequestJson();
        requestData.command = "END_GAME";
        requestData.matchId = UserDataManager.Instance.MatchRoomID;
        EmptyRequest emptyRequest = new EmptyRequest();
        requestData.request = emptyRequest;
        await webSocketManager.SendJsonRequest(requestData);
    }

    async void Surrender()
    {
        RequestJson requestData = new RequestJson();
        requestData.command = "SURRENDER_GAME";
        requestData.matchId = UserDataManager.Instance.MatchRoomID;
        EmptyRequest emptyRequest = new EmptyRequest();
        requestData.request = emptyRequest;
        await webSocketManager.SendJsonRequest(requestData);
    }

    async void Quit()
    {
        RequestJson requestData = new RequestJson();
        requestData.command = "QUIT_GAME";
        requestData.matchId = UserDataManager.Instance.MatchRoomID;
        EmptyRequest emptyRequest = new EmptyRequest();
        requestData.request = emptyRequest;
        await webSocketManager.SendJsonRequest(requestData);
        await webSocketManager.DisconnectWebSocket();

        UserDataManager.Instance.HostReady = false;
        UserDataManager.Instance.EntrantReady = false;
        UserDataManager.Instance.HostId = 0;
        UserDataManager.Instance.EntrantId = 0;
        UserDataManager.Instance.MatchStatus = MatchStatus.WAITING;
    }
}
