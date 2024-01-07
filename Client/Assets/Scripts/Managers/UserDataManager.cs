using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    private static UserDataManager _instance;

    // ����� ������
    private string _accessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJjaGFyYWN0ZXJJZCI6MTMsImlhdCI6MTcwNDYzOTQzMywiZXhwIjoyMDY0NjM5NDMzfQ.ERQcrE0takQCukRaNrSEv3beXCjDbxFmkoFi7Q7Or3c";
    private string _nickName;
    private long _userId;
    private long _characterId;
    private long _jobId;
    private long _nationId;
    private long _levelId;
    // Singleton �ν��Ͻ� ����
    public static UserDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject userDataManagerObject = new GameObject("UserDataManager");
                _instance = userDataManagerObject.AddComponent<UserDataManager>();
                DontDestroyOnLoad(userDataManagerObject); // �� ��ȯ �� �ı����� �ʵ��� ����
            }
            return _instance;
        }
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

    // ��Ÿ �ʿ��� �޼��� �� �߰� ����

    private void Awake()
    {
        // �ν��Ͻ��� �̹� �ִٸ� ���� �����Ǵ� ���� ����
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
