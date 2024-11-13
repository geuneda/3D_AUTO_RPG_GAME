using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private List<ItemData> shopItems = new List<ItemData>();
    [SerializeField] private Transform shopItemContainer;
    [SerializeField] private ShopItemUI shopItemPrefab;
    
    private InventoryManager inventory;
    private GameCurrency currency;

    private void Start()
    {
        inventory = InventoryManager.Instance;
        currency = GameCurrency.Instance;
        InitializeShop();
    }

    private void InitializeShop()
    {
        foreach (var item in shopItems)
        {
            var shopItem = Instantiate(shopItemPrefab, shopItemContainer);
            shopItem.Initialize(item, OnPurchaseItem);
        }
    }

    private void OnPurchaseItem(ItemData item)
    {
        if (currency.HasEnoughGold(item.price))
        {
            currency.SpendGold(item.price);
            inventory.AddItem(item);
        }
    }
} 