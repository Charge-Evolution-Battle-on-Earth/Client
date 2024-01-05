using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Choice : MonoBehaviour
{
    public DropdownController dropdownController;
    public string nation;
    private void Start()
    {
        dropdownController.dropdown.ClearOptions();
        StartCoroutine(GetNationList());
    }

    IEnumerator GetNationList()
    {
        string url = GameURL.DBServer.Server_URL + GameURL.DBServer.getNationsPath;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // JSON 응답 파싱
                string jsonResponse = www.downloadHandler.text;

                // 배열을 객체로 감싸서 처리
                string wrappedJsonResponse = $"{{ \"data\": {jsonResponse} }}";

                // 객체로 처리
                NationsListWrapper nationsListWrapper = JsonUtility.FromJson<NationsListWrapper>(wrappedJsonResponse);

                // 여기에서 데이터 처리
                dropdownController.AddItemsToDropdown(nationsListWrapper.data);
            }
            else
            {
                Debug.LogError("Error: " + www.error);
            }
        }
    }

    public void GoToLobby()
    {
        SceneController.LoadScene(Scenes.Lobby.ToString());
    }
}
[System.Serializable]
public class NationsListWrapper
{
    public List<NationGetListResponse> data;
}