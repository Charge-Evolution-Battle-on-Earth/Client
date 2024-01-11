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

                foreach (JobGetListResponse job in jobList)
                {
                    Debug.Log($"Job Name: {job.jobNm}, Level Stat Factor: {job.levelStatFactor}");

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
