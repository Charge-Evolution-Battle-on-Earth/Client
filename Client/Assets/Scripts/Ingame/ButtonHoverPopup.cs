using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonHoverPopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int skillIndex;
    public GameObject popupWindow;
    public TMP_Text description;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (UserDataManager.Instance.HostSkillList.Count > skillIndex)
        {
            description.text = UserDataManager.Instance.HostSkillList[skillIndex].description;
            SetPopupPosition();
            popupWindow.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        popupWindow.SetActive(false);
    }

    private void SetPopupPosition()
    {
        RectTransform buttonRectTransform = GetComponent<RectTransform>();
        RectTransform popupRectTransform = popupWindow.GetComponent<RectTransform>();

        // ��ư�� ��ġ�� ũ�⸦ �������� �˾� â�� ��ġ�� �����մϴ�.
        float buttonHeight = buttonRectTransform.rect.height;
        popupRectTransform.position = new Vector3(buttonRectTransform.position.x, buttonRectTransform.position.y + buttonHeight / 2f + popupRectTransform.rect.height / 2f, buttonRectTransform.position.z);
    }
}
