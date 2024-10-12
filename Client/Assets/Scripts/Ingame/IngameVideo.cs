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

    public Image myImg;
    public Image opponentImg;
    public RawImage myRawImage;
    public RawImage opponentRawImage;
    public VideoPlayer myVideoPlayer;
    public VideoPlayer opponentVideoPlayer;
    public RenderTexture myRenderTexture;
    public RenderTexture opponentRenderTexture;

    // VideoClip �迭 0 �ǰ�, 1 ����, 2 ��ų
    public VideoClip[] j_VideoClips;
    public VideoClip[] r_VideoClips;

    private float blinkDuration = 1.0f;
    private float blinkFrequency = 0.1f;
    private bool imgShow = false;

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
            myRawImage.gameObject.SetActive(false);
            opponentRawImage.gameObject.SetActive(false);
            myImg.sprite = null;
            myImg.color = new Color(1, 1, 1, 0);
            opponentImg.sprite = null;
            opponentImg.color = new Color(1, 1, 1, 0);
        }
        else
        {
            if (UserDataManager.Instance.PlayerType == PlayerType.HOST)
            {
                //���� �ʿ�
                /*myImg.sprite = Resources.Load<Sprite>("Prefabs/Choice/" + UserDataManager.Instance.HostJobNm);*/
                myImg.sprite = Resources.Load<Sprite>("Prefabs/Ingame/Player/Japan/" + "J_Idle");
                /*opponentImg.sprite = Resources.Load<Sprite>("Prefabs/Choice/" + UserDataManager.Instance.EntrantJobNm);*/
                opponentImg.sprite = Resources.Load<Sprite>("Prefabs/Ingame/Player/Rome/" + "R_Idle");
                if(!imgShow)
                {
                    myImg.color = Color.white;
                    opponentImg.color = Color.white;
                    imgShow = true;
                }
            }
            else if (UserDataManager.Instance.PlayerType == PlayerType.ENTRANT)
            {
                //���� �ʿ�
                /*myImg.sprite = Resources.Load<Sprite>("Prefabs/Choice/" + UserDataManager.Instance.EntrantJobNm);*/
                myImg.sprite = Resources.Load<Sprite>("Prefabs/Ingame/Player/Japan/" + "J_Idle");
                /*opponentImg.sprite = Resources.Load<Sprite>("Prefabs/Choice/" + UserDataManager.Instance.HostJobNm);*/
                opponentImg.sprite = Resources.Load<Sprite>("Prefabs/Ingame/Player/Rome/" + "R_Idle");
                if (!imgShow)
                {
                    myImg.color = Color.white;
                    opponentImg.color = Color.white;
                    imgShow = true;
                }
            }
        }
        if (UserDataManager.Instance.DamageReceiver == DamageReceiver.PLAYER)
        {
            myRawImage.gameObject.SetActive(true);
            opponentRawImage.gameObject.SetActive(true);

            if (UserDataManager.Instance.SkillType == SkillType.ATTACK)
            {
                PlayAttack(opponentVideoPlayer, opponentRenderTexture, 1, r_VideoClips);
            }
            else if (UserDataManager.Instance.SkillType == SkillType.SKILL)
            {
                PlayAttack(opponentVideoPlayer, opponentRenderTexture, 2, r_VideoClips);
            }
            else
            {
                PlayAttack(opponentVideoPlayer, opponentRenderTexture, 2, r_VideoClips);
            }

            PlayHit(myVideoPlayer, myRenderTexture, j_VideoClips);

            UserDataManager.Instance.SkillType = SkillType.NULL;
            UserDataManager.Instance.DamageReceiver = DamageReceiver.NULL;
        }
        else if (UserDataManager.Instance.DamageReceiver == DamageReceiver.OPPONENT)
        {
            myRawImage.gameObject.SetActive(true);
            opponentRawImage.gameObject.SetActive(true);

            if (UserDataManager.Instance.SkillType == SkillType.ATTACK)
            {
                PlayAttack(myVideoPlayer, myRenderTexture, 1, j_VideoClips);
            }
            else if (UserDataManager.Instance.SkillType == SkillType.SKILL)
            {
                PlayAttack(myVideoPlayer, myRenderTexture, 2, j_VideoClips);
            }
            else
            {
                PlayAttack(myVideoPlayer, myRenderTexture, 2, j_VideoClips);
            }

            PlayHit(opponentVideoPlayer, opponentRenderTexture, r_VideoClips);

            UserDataManager.Instance.SkillType = SkillType.NULL;
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

        // ����ȭ�� ������� ����� ��� ����
        videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;

        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += (vp) => {
            vp.Play();
            StartCoroutine(FadeImages(myImg, opponentImg, (float)vp.length)); // double�� float�� ��ȯ
        };
    }

    void PlayHit(VideoPlayer videoPlayer, RenderTexture renderTexture, VideoClip[] videoClips)
    {
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.clip = videoClips[0];

        // ����ȭ�� ������� ����� ��� ����
        videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;

        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += (vp) => {
            vp.Play();
            StartCoroutine(FadeImages(myImg, opponentImg, (float)vp.length)); // double�� float�� ��ȯ
        };
    }

    private IEnumerator FadeImages(Image img1, Image img2, float duration)
    {
        // �����ϰ�
        img1.color = new Color(img1.color.r, img1.color.g, img1.color.b, 0);
        img2.color = new Color(img2.color.r, img2.color.g, img2.color.b, 0);

        // ���� ���̸�ŭ ���
        yield return new WaitForSeconds(duration + 1.0f);

        // �ٽ� �������
        img1.color = new Color(img1.color.r, img1.color.g, img1.color.b, 1);
        img2.color = new Color(img2.color.r, img2.color.g, img2.color.b, 1);
    }

    void OnVideoEnd(VideoPlayer videoPlayer)
    {
        // ������ ������ ������� ���ߵ��� ó��
        videoPlayer.Stop();

        // ����� �ʱ�ȭ
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;

        // ���� �÷��̾ ���� RawImage ��Ȱ��ȭ
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
