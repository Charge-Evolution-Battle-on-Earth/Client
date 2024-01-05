using UnityEngine;
using UnityEngine.UI;

public class ItemBtnClick : MonoBehaviour
{
    public Button originalButtonPrefab;
    public Transform buttonParent; // 버튼을 생성할 부모 Transform
    public Vector3 newButtonPosition; // 새로운 버튼의 위치

    void Start()
    {
        // 기존 버튼 생성
        Button originalButton = Instantiate(originalButtonPrefab, buttonParent);

        // 기존 버튼에 클릭 이벤트 리스너 추가
        originalButton.onClick.AddListener(OnOriginalButtonClick);
    }

    void OnOriginalButtonClick()
    {
        // 기존 버튼 클릭시 수행할 작업

        // 새로운 버튼 생성 및 위치 조정
        Button newButton = Instantiate(originalButtonPrefab, buttonParent);
        newButton.transform.position = newButtonPosition;

        // 새로운 버튼에 클릭 이벤트 리스너 추가 (필요시)
        newButton.onClick.AddListener(OnNewButtonClick);
    }

    void OnNewButtonClick()
    {
        // 새로운 버튼 클릭시 수행할 작업
        Debug.Log("New Button Clicked");
    }
}
