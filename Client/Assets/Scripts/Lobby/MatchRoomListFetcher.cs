using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class CombinedScrollManager : MonoBehaviour
{
    public Transform content; // ScrollRect.content
    public GameObject listItemPrefab; // �� ����� �׸� ���� ������
    public ScrollRect scrollRect;
    public Button roomEnterBtn;
    public TMP_Text selectedRoomInfo;

    private int itemsPerPage = 10; // �� �������� ǥ�õ� ������ ����
    private int currentPage = 0;
    private bool isLoading = false;

    void Start()
    {
        //ScrollRect scrollRect = GetComponent<ScrollRect>();
        if (scrollRect != null)
        {
            // ScrollRect�� onValueChanged �̺�Ʈ�� OnScrollValueChanged �޼��带 ���
            scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
        }
        else
        {
            Debug.LogError("ScrollRect component not found!");
        }

        StartCoroutine(FetchMatchRoomList(currentPage));
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
            else if (www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error + www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + www.error + www.downloadHandler.text);
            }

            isLoading = false;
        }
    }

    public void RefreshUI()
    {
        ClearUI();
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
            listItem.GetComponentInChildren<TMP_Text>().text = $"�� ����: {room.matchStatus.ToString()}\t�� ��ȣ: {room.matchRoomId}";

            // ��ư�� Ŭ�� �̺�Ʈ �߰�
            listItem.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(room));
        }
    }


    void OnButtonClick(CONTENT_TYPE room)
    {
        roomEnterBtn.interactable = true;
        UserDataManager.Instance.RoomInfo = room;
        UserDataManager.Instance.MatchRoomID = room.matchRoomId;
        selectedRoomInfo.text = $"�� ID: {UserDataManager.Instance.RoomInfo.matchRoomId}\n �ɸ� ��: {UserDataManager.Instance.RoomInfo.stakeGold}\n";
    }

    void ClearUI()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    // onValueChanged �̺�Ʈ�� �����ϴ� �޼���
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
    public int pageSize;// �� �������� ���� �� ����
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