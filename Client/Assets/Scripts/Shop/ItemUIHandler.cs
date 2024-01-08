using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUIHandler : MonoBehaviour
{
    public Image itemImage; // 아이템 이미지를 표시할 Image 컴포넌트
    public TMP_Text itemNameText_Prefab;
    public TMP_Text itemCostText_Prefab;

    public TMP_Text itemNameText;
    public TMP_Text itemStatText;
    public TMP_Text itemDescriptionText;

    public Shop.ShopItemGetResponse item;

    public void SetItemInfo(Shop.ShopItemGetResponse newItem)
    {
        item = newItem;

        itemNameText_Prefab.text = item.itemNm;
        itemCostText_Prefab.text = item.cost.ToString();
    }

    public void OnButtonClick()
    {
        itemNameText.text = item.itemNm;
        itemStatText.text = $"HP: {item.stat.hp}\tMP: {item.stat.mp}\nATK: {item.stat.atk}\tSPD: {item.stat.spd}";
        itemDescriptionText.text = item.description;
    }
}
