using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class MatchRoomListFetcher : MonoBehaviour
{
    public GameObject roomListBtn;
    public GameObject pageListBtn;
    public Transform roomListPanel;
    public Transform pageListPanel;
    public TMP_Text selectedRoomInfo;
    public Button roomEnterBtn;

    private float buttonSpacing = 2.8f; // 버튼 간격
    private int itemsPerPage = 10;
    private int currentPage = 0;

    void Start()
    {
        StartCoroutine(FetchMatchRoomList(0));
    }
    IEnumerator FetchMatchRoomList(int pageNm)
    {
        string pageNumber = GameURL.DBServer.getMatchRoomListPath + $"page={pageNm}&size=10";
        string url = GameURL.DBServer.Server_URL + pageNumber;
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", $"Bearer {UserDataManager.Instance.AccessToken}");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                Slice sliceResponse = JsonUtility.FromJson<Slice>(jsonResponse);

                UserDataManager.Instance.RoomListInfo = sliceResponse.content;
                UpdateUI();
            }
            else if(www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error + www.downloadHandler.text);
            }

            else
            {
                Debug.LogError("Error: " + www.error + www.downloadHandler.text);
            }
        }
    }

    public void UpdateUI()
    {
        ClearUI();
        roomEnterBtn.interactable = false;
        int startIndex = 0;
        int endIndex = Mathf.Min(itemsPerPage, UserDataManager.Instance.RoomListInfo.Count);

        RectTransform contentPanelRect = roomListPanel.GetComponent<RectTransform>();
        float contentPanelWidth = contentPanelRect.rect.width;

        for (int i = startIndex; i < endIndex; i++)
        {
            CONTENT_TYPE room = UserDataManager.Instance.RoomListInfo[i];
            GameObject button = Instantiate(roomListBtn, roomListPanel);

            RectTransform rect = button.GetComponent<RectTransform>();

            int idx = i;
            while (idx >= 10)
            {
                idx -= 10;
            }
            float x = contentPanelWidth / 2;
            float y = -idx * (rect.rect.height + buttonSpacing) - rect.rect.height / 2;

            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            button.GetComponentInChildren<TMP_Text>().text = $"방 상태: {room.matchStatus}\t방 번호: {room.matchRoomId}";

            // 버튼에 클릭 이벤트 추가
            button.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(room));
        }
        CreatePageButtons();
    }

    void CreatePageButtons()
    {
        CreateButton("처음", FirstPage);
        CreateButton("이전", PreviousPage);
        CreateButton("다음", NextPage);
    }


    void CreateButton(string buttonText, UnityEngine.Events.UnityAction buttonAction)
    {
        GameObject button = Instantiate(pageListBtn, pageListPanel);
        button.GetComponentInChildren<TMP_Text>().text = buttonText;
        button.GetComponent<Button>().onClick.AddListener(buttonAction);
    }

    void OnButtonClick(CONTENT_TYPE room)
    {
        roomEnterBtn.interactable = true;
        UserDataManager.Instance.RoomInfo = room;
        UserDataManager.Instance.MatchRoomID = room.matchRoomId;
        selectedRoomInfo.text = $"방 ID: {UserDataManager.Instance.RoomInfo.matchRoomId}\n 걸린 돈: {UserDataManager.Instance.RoomInfo.stakeGold}\n";
    }

    void ClearUI()
    {
        foreach (Transform child in roomListPanel)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in pageListPanel)
        {
            Destroy(child.gameObject);
        }
    }

    public void FirstPage()
    {
        currentPage = 0;
        StartCoroutine(FetchMatchRoomList(currentPage));
    }

    public void NextPage()
    {
        currentPage = currentPage + 1;//Mathf.Min(currentPage + 1, Mathf.CeilToInt((float)UserDataManager.Instance.RoomListInfo.Count / itemsPerPage));
        StartCoroutine(FetchMatchRoomList(currentPage));
    }

    public void PreviousPage()
    {
        currentPage = Mathf.Max(currentPage - 1, 0);
        StartCoroutine(FetchMatchRoomList(currentPage));
    }

    public void GoToPage(int idx)
    {
        Debug.Log($"GoToPage: {idx}");
        currentPage = idx;
        StartCoroutine(FetchMatchRoomList(currentPage));
    }
}

[System.Serializable]
public class Slice
{
    public List<CONTENT_TYPE> content;
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
    public long entrantId;
    public MatchStatus matchStatus;
    public int stakeGold;
}

[System.Serializable]
public enum MatchStatus
{
    WAITING,
    READY,
    IN_PROGRESS,
    FINISHED
}
