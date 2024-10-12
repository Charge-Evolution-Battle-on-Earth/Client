using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using System;
using UnityEngine.Video;

public class IngameVideo : MonoBehaviour
{
    public PopupManager popupManager;

    public RawImage myRawImage;
    public RawImage opponentRawImage;
    public VideoPlayer myVideoPlayer;
    public VideoPlayer opponentVideoPlayer;
    public RenderTexture myRenderTexture;
    public RenderTexture opponentRenderTexture;

    public VideoClip[] j_VideoClips;
    public VideoClip[] r_VideoClips;

    private float blinkDuration = 1.0f;
    private float blinkFrequency = 0.1f;
    void Start()
    {
        StartCoroutine(GetSkillList());

        myVideoPlayer.loopPointReached += OnVideoEnd;
        opponentVideoPlayer.loopPointReached += OnVideoEnd;
    }

    void Update()
    {
        if (UserDataManager.Instance.MatchStatus != MatchStatus.IN_PROGRESS)
        {
            myRawImage.texture = myRenderTexture;
            opponentRawImage.texture = opponentRenderTexture;
        }
        if (UserDataManager.Instance.DamageReceiver == DamageReceiver.PLAYER)
        {
            //수정 필요
            //StartCoroutine(Blink(myImg));
            //공격 + 피격 X 나라 개수만큼 if문이 늘어나서 구조 개편 필요
            if (UserDataManager.Instance.EntrantNationNm == "일본")
            {
                PlayAttack(opponentVideoPlayer, opponentRenderTexture, 1, j_VideoClips);
            }
            UserDataManager.Instance.DamageReceiver = DamageReceiver.NULL;
        }
        else if (UserDataManager.Instance.DamageReceiver == DamageReceiver.OPPONENT)
        {
            //수정 필요
            //StartCoroutine(Blink(opponentImg));
            if (UserDataManager.Instance.EntrantNationNm == "로마")
            {
                PlayAttack(myVideoPlayer, myRenderTexture, 1, r_VideoClips);
            }
            UserDataManager.Instance.DamageReceiver = DamageReceiver.NULL;
        }
    }

    IEnumerator GetSkillList()
    {
        string url = GameURL.DBServer.Server_URL + GameURL.DBServer.getSkills;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", $"Bearer {UserDataManager.Instance.AccessToken}");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                List<SkillsGetResponse> skillsResponse = JsonConvert.DeserializeObject<List<SkillsGetResponse>>(jsonResponse);
                DataManager.Instance.SkillsGetResponse = skillsResponse;
            }
            else
            {
                popupManager.ShowPopup("Error: " + www.error + www.downloadHandler.text);
            }
        }
    }


    private IEnumerator Blink(Image image)
    {
        float endTime = Time.time + blinkDuration;

        while (Time.time < endTime)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
            yield return new WaitForSeconds(blinkFrequency);
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
            yield return new WaitForSeconds(blinkFrequency);
        }

        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
    }

    void PlayAttack(VideoPlayer videoPlayer, RenderTexture renderTexture, int attackType, VideoClip[] videoClips)
    {
        videoPlayer.targetTexture = renderTexture;

        videoPlayer.clip = videoClips[attackType];

        videoPlayer.Play();
    }

    void PlayHit(VideoPlayer videoPlayer, RenderTexture renderTexture, VideoClip[] videoClips)
    {
        videoPlayer.targetTexture = renderTexture;

        videoPlayer.clip = videoClips[0];

        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer videoPlayer)
    {
        // 비디오 플레이어에 따른 RawImage 비활성화
        if (videoPlayer == myVideoPlayer)
        {
            myRawImage.gameObject.SetActive(false);
        }
        else if (videoPlayer == opponentVideoPlayer)
        {
            opponentRawImage.gameObject.SetActive(false);
        }
    }
}
