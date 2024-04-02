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
                                // greeting ����
                                string greetingMessage = jsonData["greetingMessage"].ToString();
                                string userId = "";
                                foreach (char c in greetingMessage)
                                {
                                    if (char.IsDigit(c))
                                    {
                                        userId += c;
                                    }
                                }
                                if (int.TryParse(userId, out int number) && UserDataManager.Instance.UserId == 0)
                                {
                                    UserDataManager.Instance.UserId = number;
                                }

                                Debug.Log(greetingMessage);
                            }
                            else if (jsonData.ContainsKey("hostReadyStatus"))
                            {
                                // ready ����
                                bool hostReadyStatus = Convert.ToBoolean(jsonData["hostReadyStatus"]);
                                bool entrantReadyStatus = Convert.ToBoolean(jsonData["entrantReadyStatus"]);
                                string matchStatus = Convert.ToString(jsonData["matchStatus"]);

                                UserDataManager.Instance.HostReady = hostReadyStatus;
                                UserDataManager.Instance.EntrantReady = entrantReadyStatus;
                                UserDataManager.Instance.MatchStatus = (MatchStatus)Enum.Parse(typeof(MatchStatus), matchStatus);
                            }
                            else if (jsonData.ContainsKey("hostTotalStat"))
                            {
                                // start ����
                                var hostTotalStatData = jsonData["hostTotalStat"] as Dictionary<string, object>;
                                Stat hostTotalStat = new Stat();
                                hostTotalStat.hp = Convert.ToInt32(hostTotalStatData["hp"]);
                                hostTotalStat.atk = Convert.ToInt32(hostTotalStatData["atk"]);
                                hostTotalStat.mp = Convert.ToInt32(hostTotalStatData["mp"]);
                                hostTotalStat.spd = Convert.ToInt32(hostTotalStatData["spd"]);
                                UserDataManager.Instance.HostTotalStat = hostTotalStat;

                                var entrantTotalStatData = jsonData["entrantTotalStat"] as Dictionary<string, object>;
                                Stat entrantTotalStat = new Stat();
                                entrantTotalStat.hp = Convert.ToInt32(entrantTotalStatData["hp"]);
                                entrantTotalStat.atk = Convert.ToInt32(entrantTotalStatData["atk"]);
                                entrantTotalStat.mp = Convert.ToInt32(entrantTotalStatData["mp"]);
                                entrantTotalStat.spd = Convert.ToInt32(entrantTotalStatData["spd"]);
                                UserDataManager.Instance.EntrantTotalStat = entrantTotalStat;

                                var hostSkillListData = jsonData["hostSkillList"] as List<Dictionary<string, object>>;
                                List<CharacterSkillGetResponse> hostSkillList = new List<CharacterSkillGetResponse>();
                                foreach (var skillData in hostSkillListData)
                                {
                                    CharacterSkillGetResponse skill = new CharacterSkillGetResponse();
                                    skill.skillId = Convert.ToInt64(skillData["skillId"]);

                                    skill.skillNm = skillData["skillNm"].ToString();
                                    skill.description = skillData["description"].ToString();

                                    hostSkillList.Add(skill);
                                }
                                UserDataManager.Instance.HostSkillList = hostSkillList;

                                var entrantSkillListData = jsonData["entrantSkillList"] as List<Dictionary<string, object>>;
                                List<CharacterSkillGetResponse> entrantSkillList = new List<CharacterSkillGetResponse>();
                                foreach (var skillData in entrantSkillListData)
                                {
                                    CharacterSkillGetResponse skill = new CharacterSkillGetResponse();
                                    skill.skillId = Convert.ToInt64(skillData["skillId"]);

                                    skill.skillNm = skillData["skillNm"].ToString();
                                    skill.description = skillData["description"].ToString();

                                    entrantSkillList.Add(skill);
                                }
                                UserDataManager.Instance.EntrantSkillList = entrantSkillList;

                                string tOwner = Convert.ToString(jsonData["turnOwner"]);
                                PlayerType turnOwner = new PlayerType();
                                turnOwner = (PlayerType)Enum.Parse(typeof(PlayerType), tOwner);
                                UserDataManager.Instance.TurnOwner = turnOwner;

                                string mStatus = Convert.ToString(jsonData["matchStatus"]);
                                MatchStatus matchStatus = new MatchStatus();
                                matchStatus = (MatchStatus)Enum.Parse(typeof(MatchStatus), mStatus);
                                UserDataManager.Instance.MatchStatus = matchStatus;

                                string msg = Convert.ToString(jsonData["message"]);

                                Debug.Log(msg);
                            }
                            else if (jsonData.ContainsKey("isGameOver"))
                            {
                                // turn ����
                            }
                            else if (jsonData.ContainsKey("winnerType"))
                            {
                                // end�� surrender ����
                                string winnerType = Convert.ToString(jsonData["winnerType"]);
                                string loserType = Convert.ToString(jsonData["loserType"]);
                                PlayerType winner = new PlayerType();
                                PlayerType loser = new PlayerType();
                                winner = (PlayerType)Enum.Parse(typeof(PlayerType), winnerType);
                                loser = (PlayerType)Enum.Parse(typeof(PlayerType), loserType);

                                int winnerGold = Convert.ToInt32(jsonData["winnerGold"]);
                                int loserGold = Convert.ToInt32(jsonData["loserGold"]);
                                int winnerTotalGold = Convert.ToInt32(jsonData["winnerTotalGold"]);
                                int loserTotalGold = Convert.ToInt32(jsonData["loserTotalGold"]);
                                int winnerTotalExp = Convert.ToInt32(jsonData["winnerTotalExp"]);
                                int loserTotalExp = Convert.ToInt32(jsonData["loserTotalExp"]);
                                string msg = Convert.ToString(jsonData["message"]);

                                Debug.Log(msg);
                            }
                            else if (jsonData.ContainsKey("playerType"))
                            {
                                // quit ����
                                string playerType = Convert.ToString(jsonData["playerType"]);
                                string msg = Convert.ToString(jsonData["message"]);

                                UserDataManager.Instance.PlayerType = (PlayerType)Enum.Parse(typeof(PlayerType), playerType);
                                if (UserDataManager.Instance.PlayerType == PlayerType.CREATOR && UserDataManager.Instance.UserId == UserDataManager.Instance.EntrantId)
                                {
                                    // ������ ������ �� ���� ��������
                                    UserDataManager.Instance.HostId = UserDataManager.Instance.UserId;
                                    UserDataManager.Instance.EntrantId = 0;
                                }
                                Debug.Log(msg);
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
    public bool hostReadyStatus;
    public bool entrantReadyStatus;
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
[Serializable]
public enum PlayerType 
{
    CREATOR,
    ENTRANT
}
[Serializable]
public class CharacterSkillGetResponse
{
    public long skillId;
    public string skillNm;
    public string description;
}