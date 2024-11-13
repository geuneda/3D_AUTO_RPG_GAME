using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform itemSlotContainer;
    [SerializeField] private ItemSlotUI itemSlotPrefab;
    [SerializeField] private ItemSlotUI weaponSlotUI;
    [SerializeField] private ItemSlotUI armorSlotUI;
    
    private InventoryManager inventory;
    private List<ItemSlotUI> itemSlots = new List<ItemSlotUI>();

    private void Start()
    {
        inventory = InventoryManager.Instance;
        
        inventory.OnItemAdded += AddItemUI;
        inventory.OnItemRemoved += RemoveItemUI;
        inventory.OnItemEquipped += EquipItemUI;
        inventory.OnItemUnequipped += UnequipItemUI;
    }

    private void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnItemAdded -= AddItemUI;
            inventory.OnItemRemoved -= RemoveItemUI;
            inventory.OnItemEquipped -= EquipItemUI;
            inventory.OnItemUnequipped -= UnequipItemUI;
        }
    }

    private void AddItemUI(ItemSlot slot)
    {
        var slotUI = Instantiate(itemSlotPrefab, itemSlotContainer);
        slotUI.SetItem(slot);
        itemSlots.Add(slotUI);
    }

    private void RemoveItemUI(ItemSlot slot)
    {
        var slotUI = itemSlots.Find(ui => ui.ItemSlot == slot);
        if(slotUI != null)
        {
            itemSlots.Remove(slotUI);
            Destroy(slotUI.gameObject);
        }
    }

    private void EquipItemUI(ItemSlot slot)
    {
        if(slot.item.itemType == ItemType.Weapon)
            weaponSlotUI.SetItem(slot);
        else if(slot.item.itemType == ItemType.Armor)
            armorSlotUI.SetItem(slot);
    }

    private void UnequipItemUI(ItemSlot slot)
    {
        if(slot.item.itemType == ItemType.Weapon)
            weaponSlotUI.ClearSlot();
        else if(slot.item.itemType == ItemType.Armor)
            armorSlotUI.ClearSlot();
    }
} 