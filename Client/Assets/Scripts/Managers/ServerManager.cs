using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

// ServerManager�� ���̽��� �� Ŭ����
public class HttpServerBase : MonoBehaviour
{
    public enum SendType { GET, POST, PUT, DELETE }

    // ���������� ������ ��
    protected virtual IEnumerator SendRequestCor(string url, SendType sendType, JObject jobj) //, Action<Result> onSucceed, Action<Result> onFailed, Action<Result> onNetworkFailed)
    {
        // ��Ʈ��ũ ������¸� Ȯ���Ѵ�.
        yield return StartCoroutine(CheckNetwork());

        using (var req = new UnityWebRequest(url, sendType.ToString()))
        {
            // ���� �����͸� Ȯ���ϱ� ���� �α�
            Debug.LogFormat("url: {0} \n" +
                "����������: {1}",
                url,
                JsonConvert.SerializeObject(jobj, Formatting.Indented));

            // body �Է�
            byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jobj));
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            // header �Է�
            req.SetRequestHeader("Content-Type", "application/json");

            // ����
            yield return req.SendWebRequest();

            // �������� �������� Ȯ��
            var result = ResultCheck(req);
            // ��Ʈ��ũ �������
            /*if (result.IsNetworkError)
            {
                onNetworkFailed?.Invoke(result);

                // TODO: ��Ʈ��ũ ��õ� �˾� ȣ��.

                yield return new WaitForSeconds(1f);
                Debug.LogError("��õ�");
                yield return StartCoroutine(SendRequestCor(url, sendType, jobj, onSucceed, onFailed, onNetworkFailed));
            }
            else
            {
                // ��ż��� �̶��
                if (result.IsSuccess)
                {
                    onSucceed?.Invoke(result);
                }
                // ������ ���ж��(��ǲ�� �߸��ƉB�� �������� ��������� ������)
                else
                {
                    onFailed?.Invoke(result);
                }
            }*/
        }
    }

    protected virtual IEnumerator CheckNetwork()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            // TODO: ��Ʈ��ũ ���� �˾� ȣ��
            Debug.LogError("��Ʈ��ũ ���� �ȵ�");

            yield return new WaitUntil(() => Application.internetReachability != NetworkReachability.NotReachable);

            Debug.Log("��Ʈ��ũ �翬���");
        }
    }

    protected virtual Result ResultCheck(UnityWebRequest req)
    {
        Result res;
        switch (req.result)
        {
            case UnityWebRequest.Result.InProgress:
                res = new Result(req.downloadHandler.text, false, true, "InProgress");
                return res;
            case UnityWebRequest.Result.Success:
                JObject jobj = JObject.Parse(req.downloadHandler.text);
                // ���������� "code"�����Ͱ� 0�� �ƴϸ� ���� ���� ���̽��� ����� �ߴ�.
                bool isSuccess = int.Parse(jobj["code"].ToString()) == 0 ? true : false;

                Debug.Log(req.downloadHandler.text);
                // ����
                if (isSuccess)
                {
                    res = new Result(req.downloadHandler.text, true, false, string.Empty);
                    return res;
                }
                // ����
                else
                {
                    Debug.LogErrorFormat("��û ����: {0}", jobj["message"].ToString());
                    res = new Result(req.downloadHandler.text, false, false,
                        string.Format("Code: {0} - {1}", jobj["code"].ToString(), jobj["message"].ToString()));
                    return res;
                }
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.ProtocolError:
            case UnityWebRequest.Result.DataProcessingError:
                // ��ſ���
                Debug.LogError(req.error);
                Debug.Log(req.downloadHandler.text);
                res = new Result(req.downloadHandler.text, false, true, req.error);
                return res;
            default:
                Debug.LogError("����Ʈ ���̽��� �ɸ�");
                Debug.LogError(req.error);
                Debug.Log(req.downloadHandler.text);
                res = new Result(req.downloadHandler.text, false, true, "Unknown");
                return res;
        }
    }

    // ��� ����� ������� Ŭ����
    public class Result
    {
        // �����κ��� ���� ���ϰ��� ���� ����
        private string json;
        // �������� ����
        private bool isSuccess;
        // ��Ʈ��ũ �������� ������ �������� ����
        private bool isNetworkError;
        // ���� ����
        private string error;

        public string Json => json;
        public bool IsSuccess => isSuccess;
        public bool IsNetworkError => isNetworkError;
        public string Error => error;

        public Result(string json, bool isSuccess, bool isNetworkError, string error)
        {
            this.json = json;
            this.isSuccess = isSuccess;
            this.isNetworkError = isNetworkError;
            this.error = error;
        }
    }
}