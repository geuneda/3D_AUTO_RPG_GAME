using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    private GameEventManager eventManager;
    private ItemSlotPool slotPool;

    public List<ItemSlot> items = new List<ItemSlot>();
    public ItemSlot equippedWeapon { get; private set; }
    public ItemSlot equippedArmor { get; private set; }
    
    protected override void Awake()
    {
        base.Awake();
        eventManager = GameEventManager.Instance;
        slotPool = new ItemSlotPool();
    }

    public void AddItem(ItemData item)
    {
        if(item == null) return;

        if(item.itemType == ItemType.Consumable)
        {
            var existingSlot = items.Find(slot => slot.item == item);
            
            if(existingSlot != null)
            {
                existingSlot.amount++;
                eventManager.TriggerItemAdded(existingSlot);
                return;
            }
        }

        var newSlot = slotPool.Get();
        newSlot.item = item;
        newSlot.amount = 1;
        items.Add(newSlot);
        eventManager.TriggerItemAdded(newSlot);
    }

    public void RemoveItem(ItemSlot slot)
    {
        if (!items.Contains(slot)) return;
        
        items.Remove(slot);
        slotPool.Return(slot);
        eventManager.TriggerItemRemoved(slot);
    }

    public void UseItem(ItemSlot slot)
    {
        if(slot == null || slot.item == null) return;

        if(slot.item.itemType == ItemType.Consumable)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if(player != null && player.TryGetComponent<PlayerHealth>(out var health))
            {
                health.Heal(slot.item.healthRestoreAmount);
                
                if(slot.amount <= 1)
                {
                    RemoveItem(slot);
                }
                else
                {
                    slot.amount--;
                    eventManager.TriggerItemAdded(slot);
                }
            }
        }
    }

    public void EquipItem(ItemSlot slot)
    {
        if(slot.item.itemType == ItemType.Weapon)
        {
            if(equippedWeapon != null)
                UnequipItem(equippedWeapon);
            equippedWeapon = slot;
            eventManager.TriggerItemEquipped(slot);
        }
        else if(slot.item.itemType == ItemType.Armor)
        {
            if(equippedArmor != null)
                UnequipItem(equippedArmor);
            equippedArmor = slot;
            eventManager.TriggerItemEquipped(slot);
        }

        items.Remove(slot);
    }

    public void UnequipItem(ItemSlot slot)
    {
        if(slot == equippedWeapon)
            equippedWeapon = null;
        else if(slot == equippedArmor)
            equippedArmor = null;

        items.Add(slot);
        eventManager.TriggerItemUnequipped(slot);
    }

    private void OnDestroy()
    {
        foreach(var slot in items.ToList())
        {
            RemoveItem(slot);
        }
        
        if(equippedWeapon != null)
            slotPool.Return(equippedWeapon);
        if(equippedArmor != null)
            slotPool.Return(equippedArmor);
    }
}

[System.Serializable]
public class ItemSlot
{
    public ItemData item;
    public int amount;
} 