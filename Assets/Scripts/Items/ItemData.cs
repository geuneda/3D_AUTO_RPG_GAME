using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "RPG/Items/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public Sprite icon;
    public int price;
    [TextArea] public string description;

    public float healthRestoreAmount; // 포션
    public float attackBonus; // 무기
    public float defenseBonus; // 방어구
}

public enum ItemType 
{
    Consumable,
    Weapon,
    Armor
} 