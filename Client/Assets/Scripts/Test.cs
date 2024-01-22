using UnityEngine;
using TMPro;

public class ScrollViewController : MonoBehaviour
{
    public GameObject listItemPrefab;
    public Transform content;

    void Start()
    {
        // 예시로 20개의 자식을 생성하여 동적으로 패널 크기를 조절
        for (int i = 0; i < 20; i++)
        {
            GameObject listItem = Instantiate(listItemPrefab, content);
            listItem.GetComponentInChildren<TMP_Text>().text = "Item " + i;
        }
    }
}
