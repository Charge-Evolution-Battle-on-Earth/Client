using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUIHandler : MonoBehaviour
{
    public Image itemImage; // 아이템 이미지를 표시할 Image 컴포넌트
    public TMP_Text itemNameText_Prefab;
    public TMP_Text itemCostText_Prefab;

    public TMP_Text itemNameText;
    public TMP_Text itemStatText;
    public TMP_Text itemDescriptionText;

    public Shop.ShopItemGetResponse item;
    public Button purchaseButton;
    private PurchaseItem purchaseItem;

    public void SetItemInfo(Shop.ShopItemGetResponse newItem)
    {
        item = newItem;

        itemNameText_Prefab.text = item.itemNm;
        itemCostText_Prefab.text = item.cost.ToString();
    }

    public void OnButtonClick()
    {
        ImageOpaque();
        UserDataManager.Instance.ClickedItemId = item.itemId;
        Image clickedImage = GetComponent<Image>();
        itemImage.sprite = clickedImage.sprite;
        itemNameText.text = item.itemNm;
        itemStatText.text = $"HP: {item.stat.hp}\tMP: {item.stat.mp}\nATK: {item.stat.atk}\tSPD: {item.stat.spd}";
        itemDescriptionText.text = item.description;
        purchaseButton.interactable = true;
    }

    public void purchaseOnButtonClick()
    {
        ImageTransparency();

        purchaseItem = FindObjectOfType<PurchaseItem>();
        StartCoroutine(purchaseItem.BuyItem());
    }
    

    void ImageTransparency()
    {
        Color color = new Color();
        color.a = 0f;
        itemImage.color = color;
        itemNameText.text = "";
        itemStatText.text = "";
        itemDescriptionText.text = "";
    }

    void ImageOpaque()
    {
        Color color = new Color();
        color.a = 1f;
        color.r = 1f;
        color.g = 1f;
        color.b = 1f;
        itemImage.color = color;
    }
}

[System.Serializable]
public class BuyGetResponse
{
    public long itemId;
    public string itemNm;
    public int money;
}
