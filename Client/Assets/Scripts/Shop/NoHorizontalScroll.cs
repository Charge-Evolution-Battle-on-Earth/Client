using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NoHorizontalScroll : MonoBehaviour, IDragHandler
{
    private ScrollRect scrollRect;

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        scrollRect.horizontal = false;
    }
}
