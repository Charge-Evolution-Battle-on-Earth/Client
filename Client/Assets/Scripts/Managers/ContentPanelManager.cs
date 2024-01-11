using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ContentPanelManager : MonoBehaviour
{
    public GameObject roomListBtn;
    public GameObject pageListBtn;
    public Transform contentPanel;
    public Transform pagePanel;

    private float buttonSpacing = 2.8f; // 버튼 간격
    public TMP_Text selectedRoomInfo;
    public Button selectedRoomBtn;
    private List<SliceResponse> roomList;
    private int itemsPerPage = 10;
    private int currentPage = 1;

    void Start()
    {
        //roomList = GenerateSampleRoomList();//예시
        UpdateUI();
        CreatePageButtons();
    }

    List<SliceResponse> GenerateSampleRoomList()
    {
        // 여기에서 실제로 서버에서 받아온 데이터를 사용하거나
        // 샘플 데이터를 생성하여 사용할 수 있습니다.
        List<SliceResponse> sampleList = new List<SliceResponse>();
        for (int i = 1; i <= 78; i++)
        {
            SliceResponse room = new SliceResponse
            {
                content = new List<ContentType> { new ContentType { matchRoomId = i, hostId = i, entrantId = i, matchStatus = matchStatus.WAITING, stakeGold = i * 100 } },
                first = false,
                last = false,
                size = 0,
                number = i,
                numberOfElement = 0,
                empty = true
            };
            sampleList.Add(room);
        }
        return sampleList;
    }

    void UpdateUI()
    {
        ClearUI();
        selectedRoomBtn.interactable = false;
        int startIndex = (currentPage - 1) * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, roomList.Count);

        RectTransform contentPanelRect = contentPanel.GetComponent<RectTransform>();
        float contentPanelWidth = contentPanelRect.rect.width;

        for (int i = startIndex; i < endIndex; i++)
        {
            SliceResponse room = roomList[i];
            GameObject button = Instantiate(roomListBtn, contentPanel);

            RectTransform rect = button.GetComponent<RectTransform>();

            int idx = i;
            while (idx >= 10)
            {
                idx -= 10;
            }
            float x = contentPanelWidth / 2;
            float y = -idx * (rect.rect.height + buttonSpacing) - rect.rect.height / 2;

            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

            // 버튼에 클릭 이벤트 추가
            button.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(room));
        }
    }

    void CreatePageButtons()
    {
        CreateButton("처음", FirstPage);
        CreateButton("이전", PreviousPage);
        for (int i = 2; i < Mathf.CeilToInt((float)roomList.Count / itemsPerPage); i++)
        {
            int pageIndex = i;
            CreateButton($"{i}", () => GoToPage(pageIndex));
        }
        CreateButton("다음", NextPage);
        CreateButton("마지막", LastPage);
    }


    void CreateButton(string buttonText, UnityEngine.Events.UnityAction buttonAction)
    {
        GameObject button = Instantiate(pageListBtn, pagePanel);
        button.GetComponentInChildren<TMP_Text>().text = buttonText;
        button.GetComponent<Button>().onClick.AddListener(buttonAction);
    }

    void OnButtonClick(SliceResponse room)
    {
        selectedRoomBtn.interactable = true;
        UserDataManager.Instance.RoomInfo = room;
        UserDataManager.Instance.MatchRoomID = room.content[0].matchRoomId;
        selectedRoomInfo.text = $"방 번호: {UserDataManager.Instance.RoomInfo.number}\n 걸린 돈: {UserDataManager.Instance.RoomInfo.content[0].stakeGold}\n";
    }

    void ClearUI()
    {
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }
    }

    public void FirstPage()
    {
        currentPage = 1;
        UpdateUI();
    }

    public void LastPage()
    {
        currentPage = Mathf.CeilToInt((float)roomList.Count / itemsPerPage);
        UpdateUI();
    }

    public void NextPage()
    {
        currentPage = Mathf.Min(currentPage + 1, Mathf.CeilToInt((float)roomList.Count / itemsPerPage));
        UpdateUI();
    }

    public void PreviousPage()
    {
        currentPage = Mathf.Max(currentPage - 1, 1);
        UpdateUI();
    }

    public void GoToPage(int idx)
    {
        Debug.Log($"GoToPage: {idx}");
        currentPage = idx;
        UpdateUI();
    }

}