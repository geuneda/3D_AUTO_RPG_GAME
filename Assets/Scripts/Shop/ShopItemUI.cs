using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button purchaseButton;

    private ItemData item;
    private System.Action<ItemData> onPurchase;

    public void Initialize(ItemData itemData, System.Action<ItemData> purchaseCallback)
    {
        item = itemData;
        onPurchase = purchaseCallback;

        iconImage.sprite = item.icon;
        itemNameText.text = item.itemName;
        priceText.text = $"{item.price}G";

        purchaseButton.onClick.AddListener(() => onPurchase?.Invoke(item));
    }

    private void OnDestroy()
    {
        purchaseButton.onClick.RemoveAllListeners();
    }
} 