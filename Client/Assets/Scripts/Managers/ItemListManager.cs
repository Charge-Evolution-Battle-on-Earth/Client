using UnityEngine;
using UnityEngine.UI;

public class ItemListManager : MonoBehaviour
{
    public GameObject itemPrefab; // �̹��� ��ư Prefab
    public Transform contentPanel; // Scroll View�� Content �κ�

    void Start()
    {
        // ���÷� 9���� �������� �������� �߰�
        for (int i = 0; i < 10; i++)
        {
            CreateItem();
        }
    }

    void CreateItem()
    {
        // ������ Prefab�� �����Ͽ� Content �г��� �ڽ����� �߰�
        GameObject newItem = Instantiate(itemPrefab, contentPanel);

        // ������ ���� ���� �߰� ������ �ʿ��ϴٸ� �̰����� ó��
    }
}
