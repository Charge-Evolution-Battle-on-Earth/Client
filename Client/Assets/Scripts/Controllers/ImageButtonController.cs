using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class ImageButtonController : MonoBehaviour
{
    public Button warriorButton;
    public Button archerButton;
    public Button mageButton;

    public TMP_Text jobInfoText;

    public Image imageButton1;
    public Image imageButton2;
    public Image imageButton3;

    private List<JobGetListResponse> jobList;
    private Color originalColor;

    void Start()
    {
        // �������� ���� �����͸� �޾ƿ�
        StartCoroutine(GetJobs());

        // �� ��ư�� Ŭ�� �̺�Ʈ �߰�
        warriorButton.onClick.AddListener(() => DisplayJobInfo(0, imageButton1));
        archerButton.onClick.AddListener(() => DisplayJobInfo(1, imageButton2));
        mageButton.onClick.AddListener(() => DisplayJobInfo(2, imageButton3));

        // �ʱ� �̹��� �÷� ����
        originalColor = imageButton1.color;
    }


    IEnumerator GetJobs()
    {
        string url = GameURL.DBServer.Server_URL + GameURL.DBServer.getJobsPath;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // ������ �κ�: �迭�� ������ JSON�� ó���ϱ� ���� List�� �߰�
                string jsonResponse = $"{{\"jobs\":{www.downloadHandler.text}}}";
                var responseData = JsonUtility.FromJson<JobsResponse>(jsonResponse);

                if (responseData != null && responseData.jobs != null)
                {
                    jobList = responseData.jobs;
                }
                else
                {
                    Debug.LogError("Failed to parse JSON data.");
                }
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }
    }

    void DisplayJobInfo(int index, Image clickedButton)
    {
        UserDataManager.Instance.SelectedJob = index;
        if (jobList != null && index < jobList.Count)
        {
            JobGetListResponse selectedJob = jobList[index];
            string jobInfo = $"Job Name: {selectedJob.jobNm}\nLevel Stat Factor: {selectedJob.levelStatFactor}\n"
                           + $"HP: {selectedJob.stat.hp}, ATK: {selectedJob.stat.atk}, MP: {selectedJob.stat.mp}, Speed: {selectedJob.stat.speed}";
            jobInfoText.text = jobInfo;

            // �̹��� ��ư ��۸�
            ToggleImageButton(clickedButton);
        }
    }

    void ToggleImageButton(Image clickedButton)
    {
        // ���õ� ��ư�� �̹��� ��ο����� �����
        clickedButton.color = originalColor * 0.7f;

        // ������ ��ư�� �̹��� ������� ������
        if (clickedButton != imageButton1)
            imageButton1.color = originalColor;

        if (clickedButton != imageButton2)
            imageButton2.color = originalColor;

        if (clickedButton != imageButton3)
            imageButton3.color = originalColor;
    }
}
[System.Serializable]
public class JobsResponse
{
    public List<JobGetListResponse> jobs;
}