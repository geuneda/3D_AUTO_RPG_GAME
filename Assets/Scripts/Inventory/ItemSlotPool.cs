using System.Collections.Generic;

public class ItemSlotPool
{
    private Queue<ItemSlot> pool = new Queue<ItemSlot>();
    private int defaultSize = 10;

    public ItemSlotPool(int size = 20)
    {
        defaultSize = size;
        for (int i = 0; i < size; i++)
        {
            CreateNewSlot();
        }
    }

    private void CreateNewSlot()
    {
        pool.Enqueue(new ItemSlot());
    }

    public ItemSlot Get()
    {
        if (pool.Count == 0)
        {
            CreateNewSlot();
        }
        return pool.Dequeue();
    }

    public void Return(ItemSlot slot)
    {
        slot.item = null;
        slot.amount = 0;
        pool.Enqueue(slot);
    }
} 