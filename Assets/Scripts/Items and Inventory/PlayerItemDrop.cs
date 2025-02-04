using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("Player's Drop")]
    [SerializeField]
    private float chanceToLoseItems;

    [SerializeField]
    private float chanceToLoseMaterials;

    public override void GenerateDrop()
    {
        var inventory = Inventory.instance;
        var itemsToUnequip = new List<InventoryItem>();
        var materialsToLose = new List<InventoryItem>();

        foreach (var item in inventory.GetEquipmentList())
            if (Random.Range(0, 100) <= chanceToLoseItems)
            {
                DropItem(item.data);
                itemsToUnequip.Add(item);
            }

        for (var i = 0; i < itemsToUnequip.Count; i++)
        {
            inventory.UnequipItem(itemsToUnequip[i].data as ItemData_Equipment);
        }

        foreach (var item in inventory.GetStashList())
            if (Random.Range(0, 100) <= chanceToLoseItems)
            {
                DropItem(item.data);
                materialsToLose.Add(item);
            }

        for (var i = 0; i < materialsToLose.Count; i++)
        {
            inventory.RemoveItem(materialsToLose[i].data);
        }
    }
}