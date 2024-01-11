using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Collections;

public class InventoryItemUIHandler : MonoBehaviour
{
    public Image itemImage; // 아이템 이미지를 표시할 Image 컴포넌트
    public TMP_Text itemNameText_Prefab;

    public TMP_Text itemNameText;
    public TMP_Text itemStatText;
    public TMP_Text itemDescriptionText;

    public TMP_Text MoneyText;

    public Shop.ShopItemGetResponse item;

    public void SetItemInfo(Shop.ShopItemGetResponse newItem)
    {
        item = newItem;

        itemNameText_Prefab.text = item.itemNm;
    }

    public void OnButtonClick()//아이템 선택
    {
        UserDataManager.Instance.ClickedItemId = item.itemId;
        UserDataManager.Instance.CharacterItemId = item.characterItemId;
        UserDataManager.Instance.ItemTypeId = item.itemTypeId;
        itemNameText.text = item.itemNm;
        itemStatText.text = $"HP: {item.stat.hp}\tMP: {item.stat.mp}\nATK: {item.stat.atk}\tSPD: {item.stat.spd}";
        itemDescriptionText.text = item.description;
    }
}


