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
                // JSON ���� �Ľ�
                string jsonResponse = www.downloadHandler.text;

                // �迭�� ��ü�� ���μ� ó��
                string wrappedJsonResponse = $"{{ \"data\": {jsonResponse} }}";

                // ��ü�� ó��
                NationsListWrapper nationsListWrapper = JsonUtility.FromJson<NationsListWrapper>(wrappedJsonResponse);

                // ���⿡�� ������ ó��
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