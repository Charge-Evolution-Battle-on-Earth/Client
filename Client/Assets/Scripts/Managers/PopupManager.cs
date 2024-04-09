using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupManager : MonoBehaviour
{
    public Image popup;
    public TMP_Text popupMessage;

    public void ShowPopup(string message)
    {
        popupMessage.text = message;
        popup.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
    }

    public void HidePopup()
    {
        popup.transform.position = new Vector3(Screen.width * 2f, Screen.height * 2f, 0);
    }
}
