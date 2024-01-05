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
        // �ʱ� �̹��� �÷� ����
        originalColor = imageButton1.color;
    }

    public void OnButtonClick(Image clickedButton)
    {
        // ���õ� ��ư�� �̹��� ��ο����� �����
        clickedButton.color = originalColor * 0.7f;

        // ������ ��ư�� �̹��� ������� ������
        if (clickedButton != imageButton1)
            imageButton1.color = originalColor;

        if (clickedButton != imageButton2)
            imageButton2.color = originalColor;

        if (clickedButton != imageButton3)
            imageButton3.color = originalColor;
    }
}
