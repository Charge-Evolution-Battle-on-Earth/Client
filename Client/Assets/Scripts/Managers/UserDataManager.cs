using System.Collections.Generic;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    private static UserDataManager _instance;

    // 사용자 데이터
    private string _accessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJjaGFyYWN0ZXJJZCI6MSwiaWF0IjoxNzEyMDcxMjY0LCJleHAiOjIwNzIwNzEyNjR9.-RWQoK3Z_eXT3pJOjxZ0vjnQBn4deXd__n4SGLqBh1Q";
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
    private int _stakeGold;

    private long _clickedItemId;
    private long _characterItemId;
    private long _itemTypeId;
    private bool _clearUI = false;

    private CONTENT_TYPE _selectedRoomInfo;
    private List<CONTENT_TYPE> _roomListInfo = new List<CONTENT_TYPE>();
    private Scenes _scenes;
    private List<Shop.ShopItemGetResponse> _weaponItemList = new List<Shop.ShopItemGetResponse>();
    private List<Shop.ShopItemGetResponse> _armorItemList = new List<Shop.ShopItemGetResponse>();
    /////////////INGAME///////////////
    private long _matchRoomId;
    private long _hostId;
    private long _entrantId;
    private bool _hostReady = false;
    private bool _entrantReady = false;
    private PlayerType _playerType;
    private Stat _hostTotalStat = new Stat();
    private Stat _entrantTotalStat = new Stat();
    private Stat _hostStat = new Stat();
    private Stat _entrantStat = new Stat();
    private List<CharacterSkillGetResponse> _hostSkillList = new List<CharacterSkillGetResponse>();
    private List<CharacterSkillGetResponse> _entrantSkillList = new List<CharacterSkillGetResponse>();
    private PlayerType _turnOwner;
    private MatchStatus _matchStatus;
    private bool _isGameOver = false;

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

    public void GameEnd()
    {
        _isGameOver = false;
        _hostReady = false;
        _entrantReady = false;
    }

    public void ResetIngameData()
    {
        _matchRoomId = 0;
        _hostId = 0;
        _entrantId = 0;
        _hostReady = false;
        _entrantReady = false;
        _playerType = PlayerType.HOST;
        _hostTotalStat = new Stat();
        _entrantTotalStat = new Stat();
        _hostStat = new Stat();
        _entrantStat = new Stat();
        _hostSkillList.Clear();
        _entrantSkillList.Clear();
        _turnOwner = PlayerType.HOST;
        _matchStatus = MatchStatus.WAITING;
        _isGameOver = false;
    }

    public void HostQuit()
    {
        _hostId = _entrantId;
        _entrantId = 0;
        _playerType = PlayerType.HOST;
        _hostReady = false;
        _entrantReady = false;
        _isGameOver = false;
    }

    public void EntrantQuit()
    {
        _entrantId = 0;
        _hostReady = false;
        _entrantReady = false;
        _isGameOver = false;
    }

    public List<Shop.ShopItemGetResponse> ArmorItemList
    {
        get { return _armorItemList; }
        set { _armorItemList = value; }
    }

    public List<Shop.ShopItemGetResponse> WeaponItemList
    {
        get { return _weaponItemList; }
        set { _weaponItemList = value; }
    }

    public Stat EntrantTotalStat
    {
        get { return _entrantTotalStat; }
        set { _entrantTotalStat = value; }
    }

    public Stat HostTotalStat
    {
        get { return _hostTotalStat; }
        set { _hostTotalStat = value; }
    }
    public Scenes Scene
    {
        get { return _scenes; }
        set { _scenes = value; }
    }

    public bool IsGameOver
    {
        get { return _isGameOver; }
        set { _isGameOver = value; }
    }

    public List<CharacterSkillGetResponse> EntrantSkillList
    {
        get { return _entrantSkillList; }
        set { _entrantSkillList = value; }
    }

    public List<CharacterSkillGetResponse> HostSkillList
    {
        get { return _hostSkillList; }
        set { _hostSkillList = value; }
    }

    public PlayerType TurnOwner
    {
        get { return _turnOwner; }
        set { _turnOwner = value; }
    }

    public Stat EntrantStat
    {
        get { return _entrantStat; }
        set { _entrantStat = value; }
    }

    public Stat HostStat
    {
        get { return _hostStat; }
        set { _hostStat = value; }
    }

    public bool ClearUI
    {
        get { return _clearUI; }
        set { _clearUI = value; }
    }

    public PlayerType PlayerType
    {
        get { return _playerType; }
        set { _playerType = value; }
    }

    public bool EntrantReady
    {
        get { return _entrantReady; }
        set { _entrantReady = value; }
    }

    public bool HostReady
    {
        get { return _hostReady; }
        set { _hostReady = value; }
    }

    public List<CONTENT_TYPE> RoomListInfo
    {
        get { return _roomListInfo; }
        set { _roomListInfo = value; }
    }

    public CONTENT_TYPE SelectedRoomInfo
    {
        get { return _selectedRoomInfo; }
        set { _selectedRoomInfo = value; }
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
