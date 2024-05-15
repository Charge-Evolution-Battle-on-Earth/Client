using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class MatchRoomListFetcher : MonoBehaviour
{
    public Transform content; // ScrollRect.content
    public GameObject listItemPrefab; // 방 목록의 항목에 사용될 프리팹
    public ScrollRect scrollRect;
    public Button roomEnterBtn;
    public TMP_Text selectedRoomInfo;
    public TMP_Text userInfo;
    public PopupManager popupManager;

    private int itemsPerPage = 10; // 한 페이지에 표시될 아이템 개수
    private int currentPage = 0;
    private bool isLoading = false;

    void Start()
    {
        popupManager.HidePopup();
        roomEnterBtn.interactable = false;
        if (UserDataManager.Instance.RoomListInfo.Count > 0)
        {
            UserDataManager.Instance.RoomListInfo.Clear();
        }

        if (scrollRect != null)
        {
            scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
        }
        else
        {
            popupManager.ShowPopup("ScrollRect component not found!");
        }

        StartCoroutine(FetchMatchRoomList(currentPage));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            popupManager.HidePopup();
        }
    }

    IEnumerator FetchMatchRoomList(int pageNm)
    {
        if (isLoading)
            yield break;

        isLoading = true;

        string pageNumber = $"page={pageNm}&size={itemsPerPage}";
        string url = GameURL.DBServer.Server_URL + GameURL.DBServer.getMatchRoomListPath + pageNumber;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", $"Bearer {UserDataManager.Instance.AccessToken}");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                Slice sliceResponse = JsonConvert.DeserializeObject<Slice>(jsonResponse);

                if (sliceResponse.size > 0)
                {
                    if (UserDataManager.Instance.RoomListInfo == null)
                    {
                        UserDataManager.Instance.RoomListInfo = sliceResponse.content;
                    }
                    else
                    {
                        UserDataManager.Instance.RoomListInfo.AddRange(sliceResponse.content);
                    }
                    UpdateUI();
                }
                else
                {
                    currentPage--;
                    StartCoroutine(FetchMatchRoomList(currentPage));
                }
            }
            else
            {
                popupManager.ShowPopup("Error: " + www.error + www.downloadHandler.text);
            }

            isLoading = false;
        }

        url = GameURL.DBServer.Server_URL + GameURL.DBServer.getCharacterInfoPath;
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", $"Bearer {UserDataManager.Instance.AccessToken}");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                CharacterInfoGetResponse characterInfo = JsonUtility.FromJson<CharacterInfoGetResponse>(jsonResponse);

                SaveUserData(characterInfo);
                userInfo.text = "<align=center>내 정보</align>\n닉네임: " + UserDataManager.Instance.NickName + "\n레벨: " + UserDataManager.Instance.LevelId + "\n직업: " + UserDataManager.Instance.JobNm;
            }
        }
    }

    public void RefreshUI()
    {
        ClearUI();
        roomEnterBtn.interactable = false;
        UserDataManager.Instance.RoomListInfo.Clear();
        currentPage = 0;
        StartCoroutine(FetchMatchRoomList(currentPage));
    }

    public void UpdateUI()
    {
        ClearUI();

        int startIndex = 0;
        int endIndex = UserDataManager.Instance.RoomListInfo.Count;

        RectTransform contentPanelRect = content.GetComponent<RectTransform>();
        float contentPanelWidth = contentPanelRect.rect.width;
        float listItemHeight = listItemPrefab.GetComponent<RectTransform>().rect.height;

        for (int i = startIndex; i < endIndex; i++)
        {
            CONTENT_TYPE room = UserDataManager.Instance.RoomListInfo[i];
            GameObject listItem = Instantiate(listItemPrefab, content);

            RectTransform rect = listItem.GetComponent<RectTransform>();

            float x = contentPanelWidth / 2;
            float y = -i * listItemHeight - listItemHeight / 2;

            listItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            TMP_Text[] textComponents = listItem.GetComponentsInChildren<TMP_Text>();
            textComponents[0].text = $"방 번호: {room.matchRoomId}";
            textComponents[1].text = $"방장: {room.hostNickname}\t 참가자: {room.entrantNickname}";
            textComponents[2].text = $"상태: {room.matchStatus}";

            listItem.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(room));
        }
    }

    void SaveUserData(CharacterInfoGetResponse characterInfo)
    {
        UserDataManager.Instance.Stat = characterInfo.stat;
        UserDataManager.Instance.LevelId = characterInfo.levelId;
        UserDataManager.Instance.CurrentExp = characterInfo.currentExp;
        UserDataManager.Instance.TotalExp = characterInfo.totalExp;
        UserDataManager.Instance.NationId = characterInfo.nationId;
        UserDataManager.Instance.NationNm = characterInfo.nationNm;
        UserDataManager.Instance.JobId = characterInfo.jobId;
        UserDataManager.Instance.JobNm = characterInfo.jobNm;
        UserDataManager.Instance.Money = characterInfo.money;
        UserDataManager.Instance.NickName = characterInfo.nickname;
        UserDataManager.Instance.CharacterId = characterInfo.characterId;
    }

    void OnButtonClick(CONTENT_TYPE room)
    {
        roomEnterBtn.interactable = true;
        UserDataManager.Instance.SelectedRoomInfo = room;
        UserDataManager.Instance.MatchRoomID = room.matchRoomId;
        selectedRoomInfo.text = $"방 번호: {UserDataManager.Instance.SelectedRoomInfo.matchRoomId}\n방장: {UserDataManager.Instance.SelectedRoomInfo.hostNickname}\n참가자: {UserDataManager.Instance.SelectedRoomInfo.entrantNickname}\n상금: {UserDataManager.Instance.SelectedRoomInfo.stakeGold}\n";
    }

    void ClearUI()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        selectedRoomInfo.text = "";
    }

    // onValueChanged 이벤트에 대응하는 메서드
    public void OnScrollValueChanged(Vector2 scrollPosition)
    {
        if (scrollRect.verticalNormalizedPosition <= 0 && !isLoading && 0 == Mathf.CeilToInt((float)UserDataManager.Instance.RoomListInfo.Count % itemsPerPage))
        {
            currentPage++;
            StartCoroutine(FetchMatchRoomList(currentPage));
        }
    }
}
[System.Serializable]
public class Slice
{
    public List<CONTENT_TYPE> content = new List<CONTENT_TYPE>();
    public Pageable pageable;
    public int size;
    public int number;
    public Sort sort;
    public int numberOfElements;
    public bool first;
    public bool last;
    public bool empty;
}

[System.Serializable]
public class Pageable
{
    public int pageNumber;
    public int pageSize;// 한 페이지에 들어가는 방 개수
    public Sort sort;
    public int offset;
    public bool paged;
    public bool unpaged;
}

[System.Serializable]
public class Sort
{
    public bool empty;
    public bool sorted;
    public bool unsorted;
}

[System.Serializable]
public class CONTENT_TYPE
{
    public long matchRoomId;
    public long hostId;
    public long? entrantId;//?를 사용하면 null값이어도 됨
    public string hostNickname;
    public string entrantNickname;
    public MatchStatus matchStatus { get; set; }
    public int stakeGold;
}

[System.Serializable]
public enum MatchStatus
{
    WAITING,
    READY,
    IN_PROGRESS,
    FINISHED,
}