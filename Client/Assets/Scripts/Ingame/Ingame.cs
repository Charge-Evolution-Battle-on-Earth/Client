using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Ingame : MonoBehaviour
{
    public Button startBtn;
    public Button quitBtn;
    public Button surrenderBtn;
    public Button readyBtn;
    public Image readyButtonImage;
    public Image userImg;
    public Button skillBtn0;
    public Button skillBtn1;
    public Button skillBtn2;
    public TMP_Text skillBtn0Text;
    public TMP_Text skillBtn1Text;
    public TMP_Text skillBtn2Text;
    public TMP_Text roomIdText;
    public Image myHpBar;
    public Image myMpBar;
    public Image opponentHpBar;
    public Image opponentMpBar;
    public float maxBarWidth = 350f;

    private WebSocketManager webSocketManager;
    private async void Start()
    {
        roomIdText.text = "방 번호: " + UserDataManager.Instance.MatchRoomID.ToString();
        LoadJobImg();
        webSocketManager = WebSocketManager.Instance;

        Uri serverUri = new Uri(GameURL.DBServer.PlayURL);

        await webSocketManager.ConnectWebSocket(serverUri);

        Greeting();
    }
    private void Update()
    {
        if (UserDataManager.Instance.MatchStatus == MatchStatus.READY && UserDataManager.Instance.PlayerType == PlayerType.HOST)
        {
            // 둘 다 레디 상태이고 HOST일 경우 시작 버튼 활성화
            startBtn.interactable = true;
        }
        else
        {
            startBtn.interactable = false;
        }

        if (UserDataManager.Instance.MatchStatus == MatchStatus.IN_PROGRESS || UserDataManager.Instance.MatchStatus == MatchStatus.FINISHED)
        {
            // 게임이 진행 중이면 시작 버튼 & 레디 버튼 비활성화, 스킬 버튼 & 항복 버튼 활성화
            surrenderBtn.gameObject.SetActive(true);
            surrenderBtn.interactable = true;
            startBtn.interactable = false;
            startBtn.gameObject.SetActive(false);
            readyBtn.interactable = false;
            quitBtn.interactable = false;
            skillBtn0.interactable = true;
            skillBtn1.interactable = true;
            skillBtn2.interactable = true;

            float hostMaxHP = UserDataManager.Instance.HostTotalStat.hp;
            float hostMaxMP = UserDataManager.Instance.HostTotalStat.mp;
            float entrantMaxHP = UserDataManager.Instance.EntrantTotalStat.hp;
            float entrantMaxMP = UserDataManager.Instance.EntrantTotalStat.mp;

            float hostCurrentHP = UserDataManager.Instance.HostStat.hp;
            float hostCurrentMP = UserDataManager.Instance.HostStat.mp;
            float entrantCurrentHP = UserDataManager.Instance.EntrantStat.hp;
            float entrantCurrentMP = UserDataManager.Instance.EntrantStat.mp;

            float hostHPfillAmount = hostCurrentHP / hostMaxHP;
            float hostMPfillAmount = hostCurrentMP / hostMaxMP;
            float entrantHPfillAmount = entrantCurrentHP / entrantMaxHP;
            float entrantMPfillAmount = entrantCurrentMP / entrantMaxMP;

            if (UserDataManager.Instance.PlayerType == PlayerType.HOST)
            {
                myHpBar.fillAmount = hostHPfillAmount;
                myMpBar.fillAmount = hostMPfillAmount;
                opponentHpBar.fillAmount = entrantHPfillAmount;
                opponentMpBar.fillAmount = entrantMPfillAmount;
            }
            else if (UserDataManager.Instance.PlayerType == PlayerType.ENTRANT)
            {
                myHpBar.fillAmount = entrantHPfillAmount;
                myMpBar.fillAmount = entrantMPfillAmount;
                opponentHpBar.fillAmount = hostHPfillAmount;
                opponentMpBar.fillAmount = hostMPfillAmount;
            } 

            if (UserDataManager.Instance.PlayerType == PlayerType.HOST) // 스킬 설명 저장
            {
                skillBtn0Text.text = UserDataManager.Instance.HostSkillList[0].skillNm;
                skillBtn1Text.text = UserDataManager.Instance.HostSkillList[1].skillNm;
                skillBtn2Text.text = UserDataManager.Instance.HostSkillList[2].skillNm;
            }
            else if (UserDataManager.Instance.PlayerType == PlayerType.ENTRANT)
            {
                skillBtn0Text.text = UserDataManager.Instance.EntrantSkillList[0].skillNm;
                skillBtn1Text.text = UserDataManager.Instance.EntrantSkillList[1].skillNm;
                skillBtn2Text.text = UserDataManager.Instance.EntrantSkillList[2].skillNm;
            }
        }
        else
        {
            surrenderBtn.interactable = false;
            surrenderBtn.gameObject.SetActive(false);
            startBtn.gameObject.SetActive(true);
            readyBtn.interactable = true;
            quitBtn.interactable = true;
            skillBtn0.interactable = false;
            skillBtn1.interactable = false;
            skillBtn2.interactable = false;
            skillBtn0Text.text = "";
            skillBtn1Text.text = "";
            skillBtn2Text.text = "";
        }

        if (UserDataManager.Instance.MatchStatus == MatchStatus.READY || UserDataManager.Instance.MatchStatus == MatchStatus.IN_PROGRESS)
        {
            // 둘 다 레디 상태이거나 게임이 진행 중이면 못 나감
            quitBtn.interactable = false;
        }
        else
        {
            quitBtn.interactable = true;
        }

        if(UserDataManager.Instance.IsGameOver && UserDataManager.Instance.PlayerType == PlayerType.HOST)
        {
            End();
        }

        if(UserDataManager.Instance.PlayerType == PlayerType.ENTRANT) // HOST만 시작 버튼 보이게
        {
            startBtn.gameObject.SetActive(false);
        }
        else if(UserDataManager.Instance.PlayerType == PlayerType.HOST)
        {
            startBtn.gameObject.SetActive(true);
        }

        if (UserDataManager.Instance.TurnOwner == UserDataManager.Instance.PlayerType)//자신의 차례에만 스킬 버튼 활성화
        {
            skillBtn0.interactable = true;
            skillBtn1.interactable = true;
            skillBtn2.interactable = true;
        }
        else
        {
            skillBtn0.interactable = false;
            skillBtn1.interactable = false;
            skillBtn2.interactable = false;
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
        UserDataManager.Instance.GameEnd();
    }

    async void Surrender()
    {
        RequestJson requestData = new RequestJson();
        requestData.command = "SURRENDER_GAME";
        requestData.matchId = UserDataManager.Instance.MatchRoomID;
        EmptyRequest emptyRequest = new EmptyRequest();
        requestData.request = emptyRequest;
        await webSocketManager.SendJsonRequest(requestData);
        UserDataManager.Instance.GameEnd();
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

        UserDataManager.Instance.ResetIngameData();
    }

    void LoadJobImg()
    {
        Sprite sprite = Resources.Load<Sprite>($"Prefabs/Choice/{UserDataManager.Instance.JobNm}");
        if (sprite != null)
        {
            userImg.sprite = sprite;
        }
        else
        {
            Debug.LogError("이미지를 찾을 수 없습니다: " + UserDataManager.Instance.JobNm);
        }
    }
}
