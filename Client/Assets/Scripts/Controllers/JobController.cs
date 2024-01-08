using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class JobController : MonoBehaviour
{
    public void Start()
    {
        StartCoroutine(GetJobs());
    }

    IEnumerator GetJobs()
    {
        string url = GameURL.DBServer.Server_URL + GameURL.DBServer.getJobsPath;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                List<JobGetListResponse> jobList = JsonUtility.FromJson<List<JobGetListResponse>>(jsonResponse);

                // 받아온 데이터를 사용하는 로직을 작성하세요.
                foreach (JobGetListResponse job in jobList)
                {
                    Debug.Log($"Job Name: {job.jobNm}, Level Stat Factor: {job.levelStatFactor}");

                    // 여기서 받아온 데이터를 활용하여 필요한 작업을 수행할 수 있습니다.
                }
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }
    }
}

[System.Serializable]
public class JobGetListResponse
{
    public long jobId;
    public string jobNm;
    public float levelStatFactor;
    public StatResponse stat;
}

[System.Serializable]
public class StatResponse
{
    public int hp;
    public int atk;
    public int mp;
    public int spd;
}
