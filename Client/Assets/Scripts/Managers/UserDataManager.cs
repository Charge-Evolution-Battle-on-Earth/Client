using System.Collections.Generic;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    private static UserDataManager _instance;

    // 사용자 데이터
    private string _accessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJjaGFyYWN0ZXJJZCI6MTMsImlhdCI6MTcwNDYzOTQzMywiZXhwIjoyMDY0NjM5NDMzfQ.ERQcrE0takQCukRaNrSEv3beXCjDbxFmkoFi7Q7Or3c";
    private string _nickName;
    private Stat _stat;
    private long _levelId;
    private int _currentExp;
    private int _totalExp;
    private long _nationId;
    private string _nationNm;
    private long _jobId;
    private string _jobNm;
    private int _money;
    private string _imageUrl;

    private long _userId;
    private long _characterId;
    private long _matchRoomId;
    private long _hostId;
    private long _entrantId;
    private MatchStatus _matchStatus;
    private int _stakeGold;

    private long _clickedItemId;
    private long _characterItemId;
    private long _itemTypeId;

    private CONTENT_TYPE _roomInfo;
    private List<CONTENT_TYPE> _roomListInfo;
    /////////////INGAME///////////////
    private bool _isReady = false;
    private bool _isPlaying = false;

    // Singleton 인스턴스 생성
    public static UserDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject userDataManagerObject = new GameObject("UserDataManager");
                _instance = userDataManagerObject.AddComponent<UserDataManager>();
                DontDestroyOnLoad(userDataManagerObject); // 씬 전환 시 파괴되지 않도록 설정
            }
            return _instance;
        }
    }

    public bool IsPlaying
    {
        get { return _isPlaying; }
        set { _isPlaying = value; }
    }

    public bool IsReady
    {
        get { return _isReady; }
        set { _isReady = value; }
    }

    public List<CONTENT_TYPE> RoomListInfo
    {
        get { return _roomListInfo; }
        set { _roomListInfo = value; }
    }

    public CONTENT_TYPE RoomInfo
    {
        get { return _roomInfo; }
        set { _roomInfo = value; }
    }

    public int StakeGold
    {
        get { return _stakeGold; }
        set { _stakeGold = value; }
    }

    public MatchStatus MatchStatus
    {
        get { return _matchStatus; }
        set { _matchStatus = value; }
    }

    public long EntrantId
    {
        get { return _entrantId; }
        set { _entrantId = value; }
    }

    public long HostId
    {
        get { return _hostId; }
        set { _hostId = value; }
    }

    public long ItemTypeId
    {
        get { return _itemTypeId; }
        set { _itemTypeId = value; }
    }

    public long CharacterItemId
    {
        get { return _characterItemId; }
        set { _characterItemId = value; }
    }

    public long ClickedItemId
    {
        get { return _clickedItemId; }
        set { _clickedItemId = value; }
    }

    public long MatchRoomID
    {
        get { return _matchRoomId; }
        set { _matchRoomId = value; }
    }

    public string AccessToken
    {
        get { return _accessToken; }
        set { _accessToken = value; }
    }

    public string NickName
    {
        get { return _nickName; }
        set { _nickName = value; }
    }

    public long JobId
    {
        get { return _jobId; }
        set { _jobId = value; }
    }

    public long NationId
    {
        get { return _nationId; }
        set { _nationId = value; }
    }

    public long UserId
    {
        get { return _userId; }
        set { _userId = value; }
    }

    public long CharacterId
    {
        get { return _characterId; }
        set { _characterId = value; }
    }

    public long LevelId
    {
        get { return _levelId; }
        set { _levelId = value; }
    }

    public Stat Stat
    {
        get { return _stat; }
        set { _stat = value; }
    }
    
    public int CurrentExp
    {
        get { return _currentExp; }
        set { _currentExp = value; }
    }

    public int TotalExp
    {
        get { return _totalExp; }
        set { _totalExp = value; }
    }

    public string NationNm
    {
        get { return _nationNm; }
        set { _nationNm = value; }
    }

    public string JobNm
    {
        get { return _jobNm; }
        set { _jobNm = value; }
    }

    public int Money
    {
        get { return _money; }
        set { _money = value; }
    }
    
    public string ImageUrl
    {
        get { return _imageUrl; }
        set { _imageUrl = value; }
    }
    // 기타 필요한 메서드 등 추가 가능

    private void Awake()
    {
        // 인스턴스가 이미 있다면 새로 생성되는 것을 방지
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
