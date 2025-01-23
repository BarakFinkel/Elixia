using System;

[Serializable]
public class InventoryItem
{
    public ItemData data;
    public int stackSize;

    // Constructor for new types of itemData.
    public InventoryItem(ItemData _newItemData)
    {
        data = _newItemData; // assign
        AddToStack(); // increment stack
    }

    public void AddToStack()
    {
        stackSize++;
    }

    public void RemoveFromStack()
    {
        stackSize--;
    }
}