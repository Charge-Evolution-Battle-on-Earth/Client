using System;
using System.Collections;
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
    public Image myHpBarBackground;
    public Image myMpBarBackground;
    public Image opponentHpBarBackground;
    public Image opponentMpBarBackground;
    public TMP_Text myHpBarText;
    public TMP_Text myMpBarText;
    public TMP_Text opponentHpBarText;
    public TMP_Text opponentMpBarText;
    public TMP_Text serverMsg;
    public float maxBarWidth = 350f;
    public Scrollbar verticalScrollbar;
    public TMP_Text text_Timer;
    public Image myImg;
    public Image opponentImg;

    private WebSocketManager webSocketManager;
    private float limitTime = 30f;
    private float remainingTime = 30f;
    private float blinkDuration = 1.0f;
    private float blinkFrequency = 0.1f;

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

        if (UserDataManager.Instance.MatchStatus == MatchStatus.IN_PROGRESS)
        {
            // 게임이 진행 중이면 시작 버튼 & 레디 버튼 비활성화, 스킬 버튼 & 항복 버튼 활성화, HP & MP 바 활성화
            surrenderBtn.gameObject.SetActive(true);
            surrenderBtn.interactable = true;
            startBtn.interactable = false;
            startBtn.gameObject.SetActive(false);
            readyBtn.interactable = false;
            quitBtn.interactable = false;
            skillBtn0.interactable = true;
            skillBtn1.interactable = true;
            skillBtn2.interactable = true;
            skillBtn0.gameObject.SetActive(true);
            skillBtn1.gameObject.SetActive(true);
            skillBtn2.gameObject.SetActive(true);
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

            myHpBar.enabled = true;
            myMpBar.enabled = true;
            opponentHpBar.enabled = true;
            opponentMpBar.enabled = true;
            myHpBarBackground.enabled = true;
            myMpBarBackground.enabled = true;
            opponentHpBarBackground.enabled = true;
            opponentMpBarBackground.enabled = true;

            if (UserDataManager.Instance.TurnOwner == UserDataManager.Instance.PlayerType)
            {
                remainingTime -= Time.unscaledDeltaTime;
                if (remainingTime < 0)
                {
                    remainingTime = 0;
                    SkillBtn(0);
                    remainingTime = limitTime;
                }
                text_Timer.text = Mathf.Round(remainingTime).ToString();
            }
            else
            {
                text_Timer.text = "";
                remainingTime = limitTime;
            }

            if (UserDataManager.Instance.PlayerType == PlayerType.HOST)
            {
                myHpBar.fillAmount = hostHPfillAmount;
                myMpBar.fillAmount = hostMPfillAmount;
                opponentHpBar.fillAmount = entrantHPfillAmount;
                opponentMpBar.fillAmount = entrantMPfillAmount;
                myHpBarText.text = hostCurrentHP + " / " + hostMaxHP;
                myMpBarText.text = hostCurrentMP + " / " + hostMaxMP;
                opponentHpBarText.text = entrantCurrentHP + " / " + entrantMaxHP;
                opponentMpBarText.text = entrantCurrentMP + " / " + entrantMaxMP;

                skillBtn0Text.text = UserDataManager.Instance.HostSkillList[0].skillNm;
                skillBtn1Text.text = UserDataManager.Instance.HostSkillList[1].skillNm;
                skillBtn2Text.text = UserDataManager.Instance.HostSkillList[2].skillNm;

                myImg.sprite = Resources.Load<Sprite>("Prefabs/Choice/" + UserDataManager.Instance.HostJobNm);
                myImg.color = Color.white;
                opponentImg.sprite = Resources.Load<Sprite>("Prefabs/Choice/" + UserDataManager.Instance.EntrantJobNm);
                opponentImg.color = Color.white;
            }
            else if (UserDataManager.Instance.PlayerType == PlayerType.ENTRANT)
            {
                myHpBar.fillAmount = entrantHPfillAmount;
                myMpBar.fillAmount = entrantMPfillAmount;
                opponentHpBar.fillAmount = hostHPfillAmount;
                opponentMpBar.fillAmount = hostMPfillAmount;
                myHpBarText.text = entrantCurrentHP + " / " + entrantMaxHP;
                myMpBarText.text = entrantCurrentMP + " / " + entrantMaxMP;
                opponentHpBarText.text = hostCurrentHP + " / " + hostMaxHP;
                opponentMpBarText.text = hostCurrentMP + " / " + hostMaxMP;

                skillBtn0Text.text = UserDataManager.Instance.EntrantSkillList[0].skillNm;
                skillBtn1Text.text = UserDataManager.Instance.EntrantSkillList[1].skillNm;
                skillBtn2Text.text = UserDataManager.Instance.EntrantSkillList[2].skillNm;

                myImg.sprite = Resources.Load<Sprite>("Prefabs/Choice/" + UserDataManager.Instance.EntrantJobNm);
                myImg.color = Color.white;
                opponentImg.sprite = Resources.Load<Sprite>("Prefabs/Choice/" + UserDataManager.Instance.HostJobNm);
                opponentImg.color = Color.white;
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
            skillBtn0.gameObject.SetActive(false);
            skillBtn1.interactable = false;
            skillBtn1.gameObject.SetActive(false);
            skillBtn2.interactable = false;
            skillBtn2.gameObject.SetActive(false);
            skillBtn0Text.text = "";
            skillBtn1Text.text = "";
            skillBtn2Text.text = "";
            myHpBar.enabled = false;
            myMpBar.enabled = false;
            opponentHpBar.enabled = false;
            opponentMpBar.enabled = false;
            myHpBarBackground.enabled = false;
            myMpBarBackground.enabled = false;
            opponentHpBarBackground.enabled = false;
            opponentMpBarBackground.enabled = false;
            myHpBarText.text = "";
            myMpBarText.text = "";
            opponentHpBarText.text = "";
            opponentMpBarText.text = "";
            text_Timer.text = "";
            remainingTime = limitTime;
            myImg.sprite = null;
            myImg.color = new Color(1, 1, 1, 0);
            opponentImg.sprite = null;
            opponentImg.color = new Color(1, 1, 1, 0);
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

        if(UserDataManager.Instance.DamageReceiver == DamageReceiver.PLAYER)
        {
            StartCoroutine(Blink(myImg));
            UserDataManager.Instance.DamageReceiver = DamageReceiver.NULL;
        }
        else if(UserDataManager.Instance.DamageReceiver == DamageReceiver.OPPONENT)
        {
            StartCoroutine(Blink(opponentImg));
            UserDataManager.Instance.DamageReceiver = DamageReceiver.NULL;
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
            if(UserDataManager.Instance.HostSkillList[skillId].manaCost<=UserDataManager.Instance.HostStat.mp)
            {
                skillID = (long)UserDataManager.Instance.HostSkillList[skillId].skillId;
                Turn(skillID);
            }
            else
            {
                serverMsg.text += "MP가 부족합니다." + Environment.NewLine;
                verticalScrollbar.value = 0f;
            }
        }
        else if(UserDataManager.Instance.PlayerType == PlayerType.ENTRANT)
        {
            if (UserDataManager.Instance.EntrantSkillList[skillId].manaCost <= UserDataManager.Instance.EntrantStat.mp)
            {
                skillID = (long)UserDataManager.Instance.EntrantSkillList[skillId].skillId;
                Turn(skillID);
            }
            else
            {
                serverMsg.text += "MP가 부족합니다." + Environment.NewLine;
                verticalScrollbar.value = 0f;
            }
        }
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
        if(UserDataManager.Instance.CharacterId == UserDataManager.Instance.HostId)
        {
            requestData.request.hostReadyStatus = !UserDataManager.Instance.HostReady;
            requestData.request.entrantReadyStatus = UserDataManager.Instance.EntrantReady;
        }
        else if(UserDataManager.Instance.CharacterId == UserDataManager.Instance.EntrantId)
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

    private IEnumerator Blink(Image image)
    {
        float endTime = Time.time + blinkDuration;

        while (Time.time < endTime)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
            yield return new WaitForSeconds(blinkFrequency);
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
            yield return new WaitForSeconds(blinkFrequency);
        }

        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
    }
}
