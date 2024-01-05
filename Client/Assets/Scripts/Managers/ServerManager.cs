using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

// ServerManager의 베이스가 될 클래스
public class HttpServerBase : MonoBehaviour
{
    public enum SendType { GET, POST, PUT, DELETE }

    // 최종적으로 보내는 곳
    protected virtual IEnumerator SendRequestCor(string url, SendType sendType, JObject jobj) //, Action<Result> onSucceed, Action<Result> onFailed, Action<Result> onNetworkFailed)
    {
        // 네트워크 연결상태를 확인한다.
        yield return StartCoroutine(CheckNetwork());

        using (var req = new UnityWebRequest(url, sendType.ToString()))
        {
            // 보낸 데이터를 확인하기 위한 로그
            Debug.LogFormat("url: {0} \n" +
                "보낸데이터: {1}",
                url,
                JsonConvert.SerializeObject(jobj, Formatting.Indented));

            // body 입력
            byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jobj));
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            // header 입력
            req.SetRequestHeader("Content-Type", "application/json");

            // 전송
            yield return req.SendWebRequest();

            // 성공인지 실패인지 확인
            var result = ResultCheck(req);
            // 네트워크 에러라면
            /*if (result.IsNetworkError)
            {
                onNetworkFailed?.Invoke(result);

                // TODO: 네트워크 재시도 팝업 호출.

                yield return new WaitForSeconds(1f);
                Debug.LogError("재시도");
                yield return StartCoroutine(SendRequestCor(url, sendType, jobj, onSucceed, onFailed, onNetworkFailed));
            }
            else
            {
                // 통신성공 이라면
                if (result.IsSuccess)
                {
                    onSucceed?.Invoke(result);
                }
                // 서버측 실패라면(인풋이 잘못됐덨가 서버에서 연산오류가 났던가)
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
            // TODO: 네트워크 오류 팝업 호출
            Debug.LogError("네트워크 연결 안됨");

            yield return new WaitUntil(() => Application.internetReachability != NetworkReachability.NotReachable);

            Debug.Log("네트워크 재연결됨");
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
                // 서버측에서 "code"데이터가 0이 아니면 전부 실패 케이스로 쓰기로 했다.
                bool isSuccess = int.Parse(jobj["code"].ToString()) == 0 ? true : false;

                Debug.Log(req.downloadHandler.text);
                // 성공
                if (isSuccess)
                {
                    res = new Result(req.downloadHandler.text, true, false, string.Empty);
                    return res;
                }
                // 실패
                else
                {
                    Debug.LogErrorFormat("요청 실패: {0}", jobj["message"].ToString());
                    res = new Result(req.downloadHandler.text, false, false,
                        string.Format("Code: {0} - {1}", jobj["code"].ToString(), jobj["message"].ToString()));
                    return res;
                }
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.ProtocolError:
            case UnityWebRequest.Result.DataProcessingError:
                // 통신에러
                Debug.LogError(req.error);
                Debug.Log(req.downloadHandler.text);
                res = new Result(req.downloadHandler.text, false, true, req.error);
                return res;
            default:
                Debug.LogError("디폴트 케이스에 걸림");
                Debug.LogError(req.error);
                Debug.Log(req.downloadHandler.text);
                res = new Result(req.downloadHandler.text, false, true, "Unknown");
                return res;
        }
    }

    // 통신 결과를 담기위한 클래스
    public class Result
    {
        // 서버로부터 받은 리턴값을 담을 변수
        private string json;
        // 성공인지 여부
        private bool isSuccess;
        // 네트워크 에러인지 서버측 에러인지 여부
        private bool isNetworkError;
        // 에러 내용
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