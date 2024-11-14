using System.Collections.Generic;
using UnityEngine;

public class ItemSlotUIPool : MonoBehaviour
{
    private ItemSlotUI prefab;
    private Transform container;
    private Queue<ItemSlotUI> pool = new Queue<ItemSlotUI>();
    private const int InitialPoolSize = 10;

    public void Initialize(ItemSlotUI prefab, Transform container)
    {
        this.prefab = prefab;
        this.container = container;
        
        for (int i = 0; i < InitialPoolSize; i++)
        {
            CreateNewSlotUI();
        }
    }

    private void CreateNewSlotUI()
    {
        var slotUI = Instantiate(prefab, container);
        slotUI.gameObject.SetActive(false);
        pool.Enqueue(slotUI);
    }

    public ItemSlotUI Get()
    {
        if (pool.Count == 0)
        {
            CreateNewSlotUI();
        }

        var slotUI = pool.Dequeue();
        slotUI.gameObject.SetActive(true);
        return slotUI;
    }

    public void Return(ItemSlotUI slotUI)
    {
        slotUI.ClearSlot();
        slotUI.gameObject.SetActive(false);
        pool.Enqueue(slotUI);
    }
} 