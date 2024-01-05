using UnityEngine;
using UnityEngine.UI;

public class ItemBtnClick : MonoBehaviour
{
    public Button originalButtonPrefab;
    public Transform buttonParent; // ��ư�� ������ �θ� Transform
    public Vector3 newButtonPosition; // ���ο� ��ư�� ��ġ

    void Start()
    {
        // ���� ��ư ����
        Button originalButton = Instantiate(originalButtonPrefab, buttonParent);

        // ���� ��ư�� Ŭ�� �̺�Ʈ ������ �߰�
        originalButton.onClick.AddListener(OnOriginalButtonClick);
    }

    void OnOriginalButtonClick()
    {
        // ���� ��ư Ŭ���� ������ �۾�

        // ���ο� ��ư ���� �� ��ġ ����
        Button newButton = Instantiate(originalButtonPrefab, buttonParent);
        newButton.transform.position = newButtonPosition;

        // ���ο� ��ư�� Ŭ�� �̺�Ʈ ������ �߰� (�ʿ��)
        newButton.onClick.AddListener(OnNewButtonClick);
    }

    void OnNewButtonClick()
    {
        // ���ο� ��ư Ŭ���� ������ �۾�
        Debug.Log("New Button Clicked");
    }
}
