using UnityEngine;
using UnityEngine.UI;

public class ItemListManager : MonoBehaviour
{
    public GameObject itemPrefab; // 이미지 버튼 Prefab
    public Transform contentPanel; // Scroll View의 Content 부분

    void Start()
    {
        // 예시로 9개의 아이템을 동적으로 추가
        for (int i = 0; i < 10; i++)
        {
            CreateItem();
        }
    }

    void CreateItem()
    {
        // 아이템 Prefab을 복제하여 Content 패널의 자식으로 추가
        GameObject newItem = Instantiate(itemPrefab, contentPanel);

        // 아이템 설정 등의 추가 로직이 필요하다면 이곳에서 처리
    }
}
