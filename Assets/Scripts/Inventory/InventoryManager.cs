using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public List<ItemSlot> items = new List<ItemSlot>();
    public ItemSlot equippedWeapon;
    public ItemSlot equippedArmor;
    
    public event System.Action<ItemSlot> OnItemAdded;
    public event System.Action<ItemSlot> OnItemRemoved;
    public event System.Action<ItemSlot> OnItemEquipped;
    public event System.Action<ItemSlot> OnItemUnequipped;

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStats>();
    }

    public void AddItem(ItemData item)
    {
        if(item.itemType == ItemType.Consumable)
        {
            // 수량증가
            var existingSlot = items.Find(slot => slot.item == item);
            if(existingSlot != null)
            {
                existingSlot.amount++;
                OnItemAdded?.Invoke(existingSlot);
                return;
            }
        }

        var newSlot = new ItemSlot { item = item, amount = 1 };
        items.Add(newSlot);
        OnItemAdded?.Invoke(newSlot);
    }

    public void UseItem(ItemSlot slot)
    {
        if(slot.item.itemType == ItemType.Consumable)
        {
            // 포션 사용
            var player = GameObject.FindGameObjectWithTag("Player");
            if(player.TryGetComponent<PlayerHealth>(out var health))
            {
                health.Heal(slot.item.healthRestoreAmount);
                slot.amount--;
                if(slot.amount <= 0)
                {
                    items.Remove(slot);
                    OnItemRemoved?.Invoke(slot);
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
            playerStats?.AddEquipmentBonus(slot.item.attackBonus, 0);
        }
        else if(slot.item.itemType == ItemType.Armor)
        {
            if(equippedArmor != null)
                UnequipItem(equippedArmor);
            equippedArmor = slot;
            playerStats?.AddEquipmentBonus(0, slot.item.defenseBonus);
        }

        items.Remove(slot);
        OnItemEquipped?.Invoke(slot);
    }

    public void UnequipItem(ItemSlot slot)
    {
        if(slot == equippedWeapon)
        {
            equippedWeapon = null;
            playerStats?.RemoveEquipmentBonus(slot.item.attackBonus, 0);
        }
        else if(slot == equippedArmor)
        {
            equippedArmor = null;
            playerStats?.RemoveEquipmentBonus(0, slot.item.defenseBonus);
        }

        items.Add(slot);
        OnItemUnequipped?.Invoke(slot);
    }
}

[System.Serializable]
public class ItemSlot
{
    public ItemData item;
    public int amount;
} 