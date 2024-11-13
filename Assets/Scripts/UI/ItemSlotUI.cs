using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Button button;

    public ItemSlot ItemSlot { get; private set; }

    private void Awake()
    {
        button.onClick.AddListener(OnSlotClicked);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnSlotClicked);
    }

    public void SetItem(ItemSlot slot)
    {
        ItemSlot = slot;
        iconImage.sprite = slot.item.icon;
        iconImage.enabled = true;
        
        if(slot.item.itemType == ItemType.Consumable)
        {
            amountText.text = slot.amount.ToString();
            amountText.enabled = true;
        }
        else
        {
            amountText.enabled = false;
        }
    }

    public void ClearSlot()
    {
        ItemSlot = null;
        iconImage.sprite = null;
        iconImage.enabled = false;
        amountText.enabled = false;
    }

    private void OnSlotClicked()
    {
        if(ItemSlot == null) return;

        if(ItemSlot.item.itemType == ItemType.Consumable)
            InventoryManager.Instance.UseItem(ItemSlot);
        else
            InventoryManager.Instance.EquipItem(ItemSlot);
    }
} 