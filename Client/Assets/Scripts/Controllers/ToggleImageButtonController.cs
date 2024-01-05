using UnityEngine;
using UnityEngine.UI;

public class ToggleImageButtonController : MonoBehaviour
{
    public Image imageButton1;
    public Image imageButton2;
    public Image imageButton3;

    private Color originalColor;

    private void Start()
    {
        // 초기 이미지 컬러 저장
        originalColor = imageButton1.color;
    }

    public void OnButtonClick(Image clickedButton)
    {
        // 선택된 버튼의 이미지 어두워지게 만들기
        clickedButton.color = originalColor * 0.7f;

        // 나머지 버튼의 이미지 원래대로 돌리기
        if (clickedButton != imageButton1)
            imageButton1.color = originalColor;

        if (clickedButton != imageButton2)
            imageButton2.color = originalColor;

        if (clickedButton != imageButton3)
            imageButton3.color = originalColor;
    }
}
