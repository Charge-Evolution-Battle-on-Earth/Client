using UnityEngine;
using TMPro;

public class ScrollViewController : MonoBehaviour
{
    public GameObject listItemPrefab;
    public Transform content;

    void Start()
    {
        // ���÷� 20���� �ڽ��� �����Ͽ� �������� �г� ũ�⸦ ����
        for (int i = 0; i < 20; i++)
        {
            GameObject listItem = Instantiate(listItemPrefab, content);
            listItem.GetComponentInChildren<TMP_Text>().text = "Item " + i;
        }
    }
}
