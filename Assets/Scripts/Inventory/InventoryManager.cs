using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    private ItemSlotPool slotPool;

    public List<ItemSlot> items = new List<ItemSlot>();
    public ItemSlot equippedWeapon { get; private set; }
    public ItemSlot equippedArmor { get; private set; }
    
    public event System.Action<ItemSlot> OnItemAdded;
    public event System.Action<ItemSlot> OnItemRemoved;
    public event System.Action<ItemSlot> OnItemEquipped;
    public event System.Action<ItemSlot> OnItemUnequipped;

    private PlayerStats playerStats;

    protected override void Awake()
    {
        base.Awake();
        slotPool = new ItemSlotPool();
    }

    private void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStats>();
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
                OnItemAdded?.Invoke(existingSlot);
                return;
            }
        }

        var newSlot = slotPool.Get();
        newSlot.item = item;
        newSlot.amount = 1;
        items.Add(newSlot);
        OnItemAdded?.Invoke(newSlot);
    }

    public void RemoveItem(ItemSlot slot)
    {
        if (!items.Contains(slot)) return;
        
        items.Remove(slot);
        slotPool.Return(slot);
        OnItemRemoved?.Invoke(slot);
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
                    OnItemAdded?.Invoke(slot);
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