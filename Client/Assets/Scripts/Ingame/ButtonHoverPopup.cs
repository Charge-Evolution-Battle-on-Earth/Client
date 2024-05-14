using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonHoverPopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int skillIndex;
    public GameObject popupWindow;
    public TMP_Text description;

    void Update()
    {
        if(UserDataManager.Instance.MatchStatus != MatchStatus.IN_PROGRESS)
        {
            description.text = "";
            popupWindow.gameObject.SetActive(false);
        }
        else
        {
            popupWindow.gameObject.SetActive(true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (UserDataManager.Instance.HostSkillList.Count > skillIndex)
        {
            if (UserDataManager.Instance.PlayerType == PlayerType.HOST)
            {
                description.text = UserDataManager.Instance.HostSkillList[skillIndex].description;
            }
            else if(UserDataManager.Instance.PlayerType == PlayerType.ENTRANT)
            {
                description.text = UserDataManager.Instance.EntrantSkillList[skillIndex].description;
            }
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

        // 버튼의 위치와 크기를 기준으로 팝업 창의 위치를 설정합니다.
        float buttonHeight = buttonRectTransform.rect.height;
        popupRectTransform.position = new Vector3(buttonRectTransform.position.x, buttonRectTransform.position.y + buttonHeight / 2f + popupRectTransform.rect.height / 2f, buttonRectTransform.position.z);
    }
}
