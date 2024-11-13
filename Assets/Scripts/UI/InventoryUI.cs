using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform itemSlotContainer;
    [SerializeField] private ItemSlotUI slotPrefab;
    [SerializeField] private ItemSlotUI weaponSlotUI;
    [SerializeField] private ItemSlotUI armorSlotUI;
    
    private InventoryManager inventory;
    private GameEventManager eventManager;
    private Dictionary<ItemSlot, ItemSlotUI> slotUIMap = new Dictionary<ItemSlot, ItemSlotUI>();
    private ItemSlotUIPool slotUIPool;

    private void Awake()
    {
        slotUIPool = gameObject.AddComponent<ItemSlotUIPool>();
        slotUIPool.Initialize(slotPrefab, itemSlotContainer);
    }

    private void Start()
    {
        inventory = InventoryManager.Instance;
        eventManager = GameEventManager.Instance;
        
        eventManager.OnItemAdded += AddItemUI;
        eventManager.OnItemRemoved += RemoveItemUI;
        eventManager.OnItemEquipped += EquipItemUI;
        eventManager.OnItemUnequipped += UnequipItemUI;
    }

    private void AddItemUI(ItemSlot slot)
    {
        if (slotUIMap.TryGetValue(slot, out var existingUI))
        {
            existingUI.SetItem(slot);
        }
        else
        {
            var slotUI = slotUIPool.Get();
            slotUI.SetItem(slot);
            slotUIMap[slot] = slotUI;
        }
    }

    private void RemoveItemUI(ItemSlot slot)
    {
        if (slotUIMap.TryGetValue(slot, out var slotUI))
        {
            slotUIPool.Return(slotUI);
            slotUIMap.Remove(slot);
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

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.OnItemAdded -= AddItemUI;
            eventManager.OnItemRemoved -= RemoveItemUI;
            eventManager.OnItemEquipped -= EquipItemUI;
            eventManager.OnItemUnequipped -= UnequipItemUI;
        }

        // 모든 UI 슬롯 반환
        foreach (var slotUI in slotUIMap.Values)
        {
            slotUIPool.Return(slotUI);
        }
        slotUIMap.Clear();
    }
} 