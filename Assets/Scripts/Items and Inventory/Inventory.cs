using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    [SerializeField]
    public List<ItemData> startingItems;
    public List<InventoryItem> inventory; // A list containing the current items.
    public List<InventoryItem> equipment;
    public List<InventoryItem> stash; // A list containing the current materials in our inventory.

    [Header("Inventory UI")]
    [SerializeField]
    private Transform inventorySlotsParent;

    [SerializeField]
    private Transform equipmentSlotsParent;

    [SerializeField]
    private Transform stashSlotsParent;

    [SerializeField]
    private Transform statSlotsParent;

    [SerializeField] UI_InGame uiInGame;

    private float armorCooldown;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;
    private UI_EquipmentSlot[] equipmentItemSlots;

    public Dictionary<ItemData, InventoryItem>
        inventoryDictionary; // A dictionary containing the Item Data objects as keys to the Inventory Items constructed by them.

    private UI_ItemSlot[] inventoryItemSlots;
    private float lastTimeUsedArmor;
    private float lastTimeUsedSyringe;
    public Dictionary<ItemData, InventoryItem> stashDictionary;
    private UI_ItemSlot[] stashItemSlots;
    private UI_StatSlot[] statSlots;

    [Header("Items Cooldown")]
    public float syringeCooldown { get; private set; }

    // Singleton behaviour
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();
        inventoryItemSlots = inventorySlotsParent.GetComponentsInChildren<UI_ItemSlot>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();
        equipmentItemSlots = equipmentSlotsParent.GetComponentsInChildren<UI_EquipmentSlot>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();
        stashItemSlots = stashSlotsParent.GetComponentsInChildren<UI_ItemSlot>();
        statSlots = statSlotsParent.GetComponentsInChildren<UI_StatSlot>();

        AddStartingItems();
    }

    private void AddStartingItems()
    {
        for (var i = 0; i < startingItems.Count; i++)
        {
            if (startingItems[i] != null)
            {
                AddItem(startingItems[i]);
            }
        }
    }

    public void EquipItem(ItemData _item)
    {
        var newEquipment = _item as ItemData_Equipment; // Converts the item to ItemData_Equipment type.
        var newItem =
            new InventoryItem(
                newEquipment); // Constructs a new inventory item with the ItemData_Equipment object we just created.

        // If we already have an equipment item of the same type already equipped, we'll remove from the equipment.
        var oldEquipment = GetEquipmentOfSameType(newEquipment);
        if (oldEquipment != null)
        {
            UnequipItem(oldEquipment); // Unequip old item.
            AddItem(oldEquipment); // Add it back to the inventory.
        }

        // We add the item to our equipment and it's dictionary.
        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.AddModifiers();

        // Lastly, we remove the now equipped item from the inventory.
        RemoveItem(_item);

        UpdateUISlots();
    }

    public ItemData_Equipment GetEquipmentOfSameType(ItemData_Equipment _newEquipment)
    {
        // We now check if there's already an item in the equipment with the same type, and 
        foreach (var item in equipmentDictionary)
            if (item.Key.equipmentType == _newEquipment.equipmentType)
            {
                return item.Key;
            }

        return null;
    }

    public ItemData_Equipment GetEquipmentOfType(EquipmentType _type)
    {
        // We now check if there's already an item in the equipment with the same type, and 
        foreach (var item in equipmentDictionary)
            if (item.Key.equipmentType == _type)
            {
                return item.Key;
            }

        return null;
    }

    // Removes an item from the equipment.
    public void UnequipItem(ItemData_Equipment itemToRemove)
    {
        if (equipmentDictionary.TryGetValue(itemToRemove, out var value))
        {
            equipment.Remove(value);
            equipmentDictionary.Remove(itemToRemove);
            itemToRemove.RemoveModifiers(); // Remove it's modifiers from the player's stats.
        }
    }

    // Takes all current Inventory Items and assign them to the Item Slots accordingly by the same sequence.
    public void UpdateUISlots()
    {
        // Update the UI Item slots in the equipment tab
        for (var i = 0; i < equipmentItemSlots.Length; i++)
        {
            // For pair in our dictionary
            foreach (var item in equipmentDictionary)
                // We check if the key's equipment type matches the one item we currently iterating over.
                if (item.Key.equipmentType == equipmentItemSlots[i].slotType)
                {
                    // If so, we update the current slot's variables with the InventoryItem value's.
                    equipmentItemSlots[i].UpdateSlot(item.Value);
                }
        }

        for (var i = 0; i < inventoryItemSlots.Length; i++)
        {
            inventoryItemSlots[i].CleanUpSlot();
        }

        for (var i = 0; i < inventoryItemSlots.Length; i++)
        {
            stashItemSlots[i].CleanUpSlot();
        }

        for (var i = 0; i < inventory.Count; i++)
        {
            inventoryItemSlots[i].UpdateSlot(inventory[i]);
        }

        for (var i = 0; i < stash.Count; i++)
        {
            stashItemSlots[i].UpdateSlot(stash[i]);
        }

        UpdateStatsUI();
    }

    public void AddItem(ItemData _item)
    {
        if (_item.itemType == ItemType.Equipment && CanAddItem())
        {
            AddToInventory(_item);
        }
        else if (_item.itemType == ItemType.Material)
        {
            AddToStash(_item);
        }

        UpdateUISlots();
    }

    public void RemoveItem(ItemData _item)
    {
        // If the item type exists in our inventory, we have 2 choices:
        if (inventoryDictionary.TryGetValue(_item, out var value))
        {
            // If it's the last item of the itemData type, we remove it from both the list and the dictionary
            if (value.stackSize == 1)
            {
                inventory.Remove(value);
                inventoryDictionary.Remove(_item);
            }
            // Otherwise, we decrement the stack value.
            else
            {
                value.RemoveFromStack();
            }
        }

        // If the item type exists in our stash, we have 2 choices:
        if (stashDictionary.TryGetValue(_item, out var stashValue))
        {
            // If it's the last item of the itemData type, we remove it from both the list and the dictionary
            if (stashValue.stackSize == 1)
            {
                stash.Remove(stashValue);
                stashDictionary.Remove(_item);
            }
            // Otherwise, we decrement the stack value.
            else
            {
                stashValue.RemoveFromStack();
            }
        }

        UpdateUISlots();
    }

    private void AddToInventory(ItemData _item)
    {
        // If the item type is already in our inventory, we just increment it's stack value.
        if (inventoryDictionary.TryGetValue(_item, out var value))
        {
            value.AddToStack();
        }
        // If the item does not exist in our inventory yet, we construct a new inventory item and add it to our inventory list, 
        // and also add it to the dictionary as a key with the newly created inventory item object.
        else
        {
            var newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
        }
    }

    private void AddToStash(ItemData _item)
    {
        // If the item type is already in our inventory, we just increment it's stack value.
        if (stashDictionary.TryGetValue(_item, out var value))
        {
            value.AddToStack();
        }
        // If the item does not exist in our inventory yet, we construct a new inventory item and add it to our inventory list, 
        // and also add it to the dictionary as a key with the newly created inventory item object.
        else
        {
            var newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictionary.Add(_item, newItem);
        }
    }

    public bool CanAddItem()
    {
        if (inventory.Count >= inventoryItemSlots.Length)
        {
            return false;
        }

        return true;
    }

    public bool CanCraft(ItemData_Equipment _itemToCraft, List<InventoryItem> _requiredMaterials)
    {
        var materialsToRemove = new List<InventoryItem>();

        for (var i = 0; i < _requiredMaterials.Count; i++)
        {
            // If we can find this type of item in our inventory by looking at our stash's dictionary
            if (stashDictionary.TryGetValue(_requiredMaterials[i].data, out var stashValue))
            {
                // If we don't have enough of the required type, we return false;
                if (stashValue.stackSize < _requiredMaterials[i].stackSize)
                {
                    Debug.Log("Not enough materials.");
                    return false;
                }
                // If we do, we'll add them to our temporary list of items to remove from the stash.

                materialsToRemove.Add(stashValue);
            }
            // If we couldn't, we don't have enough materials so we return false
            else
            {
                Debug.Log("Not enough materials.");
                return false;
            }
        }

        // If we reached here, we have all items required to craft the item, so we remvoe them from the stash.
        for (var i = 0; i < materialsToRemove.Count; i++)
        {
            RemoveItem(materialsToRemove[i].data);
        }

        // Add the newly crafted item.
        AddItem(_itemToCraft);
        Debug.Log("Item created: " + _itemToCraft.name);

        return true;
    }

    public ItemData_Equipment CanUseSyringe()
    {
        var currentSyringe = GetEquipmentOfType(EquipmentType.Syringe);

        if (currentSyringe != null)
        {
            if (Time.time > lastTimeUsedSyringe + syringeCooldown)
            {
                return currentSyringe;
            }

            Debug.Log("Syringe on cooldown.");
            return null;
        }

        Debug.Log("No Syringe Equipped.");
        return null;
    }

    public void UseSyringe(ItemData_Equipment _syringe)
    {
        _syringe.Effect(null);
        syringeCooldown = _syringe.itemCooldown;
        lastTimeUsedSyringe = Time.time;
        uiInGame.SetCooldownForSyringe();
    }

    public bool CanUseArmor()
    {
        var currentArmor = GetEquipmentOfType(EquipmentType.Armor);

        if (currentArmor != null)
        {
            if (Time.time > lastTimeUsedArmor + armorCooldown)
            {
                lastTimeUsedArmor = Time.time;
                armorCooldown = currentArmor.itemCooldown;
                return true;
            }

            Debug.Log("Armor on cooldown.");
            return false;
        }

        Debug.Log("No Armor Equipped.");
        return false;
    }

    public List<InventoryItem> GetEquipmentList()
    {
        return equipment;
    }

    public List<InventoryItem> GetStashList()
    {
        return stash;
    }

    public void UpdateStatsUI()
    {
        for (var i = 0; i < statSlots.Length; i++)
        {
            statSlots[i].UpdateStatValueUI();
        }        
    }
}