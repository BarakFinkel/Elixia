using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("Players Drop")]
    [SerializeField]
    [Range(0, 100)]
    private float chanceToLooseItems;

    [SerializeField]
    [Range(0, 100)]
    private float chanceToLooseMaterials;

    public override void GenerateDrop()
    {
        var inventory = Inventory.instance;
        var currentStash = inventory.GetStashedList();
        var currentItems = inventory.GetEquippedList();
        var toUnequip = new List<InventoryItem>();
        var toLoose = new List<InventoryItem>();

        foreach (var item in currentItems)
            if (Random.Range(0, 100) <= chanceToLooseItems)
            {
                DropItem(item.data);
                toUnequip.Add(item);
            }

        for (var i = 0; i < toUnequip.Count; i++)
        {
            inventory.UnequipItem(toUnequip[i].data as ItemData_Equipment);
        }

        foreach (var item in currentStash)
            if (Random.Range(0, 100) <= chanceToLooseMaterials)
            {
                DropItem(item.data);
                toLoose.Add(item);
            }

        for (var i = 0; i < toLoose.Count; i++)
        {
            inventory.RemoveItem(toLoose[i].data);
        }
    }
}