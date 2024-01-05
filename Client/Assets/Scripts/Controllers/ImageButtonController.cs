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
        // 서버에서 직업 데이터를 받아옴
        StartCoroutine(GetJobs());

        // 각 버튼에 클릭 이벤트 추가
        warriorButton.onClick.AddListener(() => DisplayJobInfo(0, imageButton1));
        archerButton.onClick.AddListener(() => DisplayJobInfo(1, imageButton2));
        mageButton.onClick.AddListener(() => DisplayJobInfo(2, imageButton3));

        // 초기 이미지 컬러 저장
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
                // 수정된 부분: 배열로 감싸진 JSON을 처리하기 위해 List를 추가
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

            // 이미지 버튼 토글링
            ToggleImageButton(clickedButton);
        }
    }

    void ToggleImageButton(Image clickedButton)
    {
        // 선택된 버튼의 이미지 어두워지게 만들기
        clickedButton.color = originalColor * 0.7f;

        // 나머지 버튼의 이미지 원래대로 돌리기
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