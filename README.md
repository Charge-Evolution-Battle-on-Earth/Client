# Charge! Evolution Battle on Earth Client
이 프로젝트는 Unity를 사용하여 클라이언트를 개발한 프로젝트입니다.   
클라이언트는 HTTP 통신과 WebSocket 통신을 사용하여 서버와 통신합니다.   

## 개발환경
- Unity version : 2022.3.13f1
- IDE : Unity, Visual Studio 2019
- language : C#

## 특징
- 이 프로젝트는 HTTP 통신과 WebSocket 통신을 사용하여 서버와의 통신을 구현했습니다.
- HTTP 통신은 UnityEngine.Networking을 사용하여 구현되었습니다.
- WebSocket 통신은 System.Net.WebSockets을 사용하여 구현되었습니다.

## 통신 방법
- **HTTP 통신** : 서버에서 정해둔 API를 사용해 GET 요청으로 원하는 데이터를 받아오고, POST 요청으로 클라이언트의 데이터를 보냅니다. 데이터의 형식은 JSON으로 이루어집니다.
> GET 요청 예시
```C#
//Newtonsoft.Json 패키지를 이용한 Json 생성
JObject userData = new JObject();
userData["email"] = id;
userData["password"] = pw;

string jsonData = userData.ToString();

// UnityWebRequest를 사용하여 서버에 POST 요청 보내기
StartCoroutine(SendData(jsonData));

IEnumerator SendData(string jsonData)
{
  string url = GameURL.AuthServer.Server_URL + GameURL.AuthServer.userLogInPath;

  using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
  {
    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    request.downloadHandler = new DownloadHandlerBuffer();
    request.SetRequestHeader("Content-Type", "application/json");

    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.Success)
    {
      // JSON 응답 파싱
      string jsonResponse = request.downloadHandler.text;
      JObject json = JObject.Parse(jsonResponse);

      if (json.ContainsKey("accessToken"))
      {
        // accessToken 저장
        string accessToken = json["accessToken"].ToString();
        PlayerPrefs.SetString("accessToken", accessToken);

        UserDataManager.Instance.AccessToken = accessToken;

        SceneController.LoadScene(Scenes.Lobby.ToString());
      }
      else
      {
        popupManager.ShowPopup("로그인에 실패했습니다. 아이디와 비밀번호를 다시 확인해주세요.");
      }
    }
    else
    {
      string jsonResponse = request.downloadHandler.text;
      JObject json = JObject.Parse(jsonResponse);

      string errorType = json["type"].ToString();
      string errorMessage = json["message"].ToString();
      string error = errorType + ": " + errorMessage;
      popupManager.ShowPopup(error);
    }
  }
}
```
> POST 요청 예시
```C#
IEnumerator RoomEnter()
{
  string url = GameURL.DBServer.Server_URL + GameURL.DBServer.getRoomEnterPath;
  JObject roomId = new JObject();
  roomId["matchRoomId"] = UserDataManager.Instance.MatchRoomID;
  string jsonData = roomId.ToString();

  using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
  {
    www.SetRequestHeader("Authorization", $"Bearer {UserDataManager.Instance.AccessToken}");

    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
    www.uploadHandler = new UploadHandlerRaw(bodyRaw);
    www.downloadHandler = new DownloadHandlerBuffer();
    www.SetRequestHeader("Content-Type", "application/json");

    yield return www.SendWebRequest();

    if (www.result == UnityWebRequest.Result.Success)
    {
      string jsonResponse = www.downloadHandler.text;
      RoomId room = JsonUtility.FromJson<RoomId>(jsonResponse);

      UserDataManager.Instance.PlayerType = PlayerType.ENTRANT;
      UserDataManager.Instance.MatchRoomID = room.matchRoomId;
      UserDataManager.Instance.HostId = room.hostId;
      UserDataManager.Instance.EntrantId = room.entrantId;
      UserDataManager.Instance.MatchStatus = room.matchStatus;
      UserDataManager.Instance.StakeGold = room.stakeGold;

      UserDataManager.Instance.RoomListInfo.Clear();
      SceneController.LoadScene(Scenes.Ingame.ToString());
    }
    else
    {
      string jsonResponse = www.downloadHandler.text;
      JObject json = JObject.Parse(jsonResponse);

      string errorType = json["type"].ToString();
      string errorMessage = json["message"].ToString();
      string error = errorType + ": " + errorMessage;
      popupManager.ShowPopup(error);
    }
  }
}
```
- **WebSocket 통신** : 서버에서 1대1 방에 있는 최대 2명의 플레이어에게 실시간으로 보내주는 메시지를 받아 JSON으로 Deserialize한 후 해석, 어떤 메시지인가에 따라 다른 액션을 취합니다. 클라이언트에서는 버튼을 누르면 그에 맞는 메시지를 서버로 보냅니다.
> 수신 예시
```C#
var jsonData = JsonConvert.DeserializeObject<Dictionary<string, object>>(message);

try
{
  if (jsonData.ContainsKey("ErrorResponse"))
  {
    // error 응답
    string errorType = jsonData["type"].ToString();
    string errorMessage = jsonData["message"].ToString();
    serverMsg.text += errorType + ": " + errorMessage + Environment.NewLine;
    verticalScrollbar.value = 0f;
  }
  else if (jsonData.ContainsKey("greetingMessage"))
  {
    // greeting 응답
    string greetingMessage = jsonData["greetingMessage"].ToString();
    string userId = "";
    foreach (char c in greetingMessage)
    {
      if (char.IsDigit(c))
      {
        userId += c;
      }
    }
    if (int.TryParse(userId, out int number) && UserDataManager.Instance.UserId == 0)
    {
      UserDataManager.Instance.UserId = number;
    }
    serverMsg.text += greetingMessage + Environment.NewLine;
    verticalScrollbar.value = 0f;
  }
  //중략
}
catch (Exception ex)
{
  popupManager.ShowPopup("JSON 파싱 오류: " + ex.Message);
}
```
> 발신 예시
```C#
async void Turn(long skillId)
{
  TurnJson requestData = new TurnJson();
  requestData.command = "TURN_GAME";
  requestData.matchId = UserDataManager.Instance.MatchRoomID;
  requestData.request.skillId = skillId;
  await webSocketManager.SendJsonRequest(requestData);
}
```
## 라이브러리
- Newtonsoft.Json
- System.Net.WebSockets
- UnityEngine.Networking

## 참고 자료
- [ClientWebSocket 클래스](https://learn.microsoft.com/ko-kr/dotnet/api/system.net.websockets.clientwebsocket?view=net-8.0)
- [.NET 클라이언트에서 Web API 호출(C#)](https://learn.microsoft.com/ko-kr/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client)
