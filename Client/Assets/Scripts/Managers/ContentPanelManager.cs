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

    private float buttonSpacing = 2.8f; // ��ư ����
    public TMP_Text selectedRoomInfo;
    public Button selectedRoomBtn;
    private List<SliceResponse> roomList;
    private int itemsPerPage = 10;
    private int currentPage = 1;

    void Start()
    {
        //roomList = GenerateSampleRoomList();//����
        UpdateUI();
        CreatePageButtons();
    }

    List<SliceResponse> GenerateSampleRoomList()
    {
        // ���⿡�� ������ �������� �޾ƿ� �����͸� ����ϰų�
        // ���� �����͸� �����Ͽ� ����� �� �ֽ��ϴ�.
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

            // ��ư�� Ŭ�� �̺�Ʈ �߰�
            button.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(room));
        }
    }

    void CreatePageButtons()
    {
        CreateButton("ó��", FirstPage);
        CreateButton("����", PreviousPage);
        for (int i = 2; i < Mathf.CeilToInt((float)roomList.Count / itemsPerPage); i++)
        {
            int pageIndex = i;
            CreateButton($"{i}", () => GoToPage(pageIndex));
        }
        CreateButton("����", NextPage);
        CreateButton("������", LastPage);
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
        selectedRoomInfo.text = $"�� ��ȣ: {UserDataManager.Instance.RoomInfo.number}\n �ɸ� ��: {UserDataManager.Instance.RoomInfo.content[0].stakeGold}\n";
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