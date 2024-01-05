using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    private static UserDataManager _instance;

    // ����� ������
    private string _accessToken;
    private string _nickName;
    private int _selectedJob;
    private int _selectedNation;

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
