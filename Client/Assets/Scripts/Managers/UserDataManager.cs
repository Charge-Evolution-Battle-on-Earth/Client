using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    private static UserDataManager _instance;

    // 사용자 데이터
    private string _accessToken;
    private string _nickName;
    private int _selectedJob;
    private int _selectedNation;

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

    public int SelectedJob
    {
        get { return _selectedJob; }
        set { _selectedJob = value; }
    }

    public int SelectedNation
    {
        get { return _selectedNation; }
        set { _selectedNation = value; }
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
