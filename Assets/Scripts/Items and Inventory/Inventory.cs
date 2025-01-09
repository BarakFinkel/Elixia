using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    [FormerlySerializedAs("startingEquip")]
    public List<ItemData> startingItems = new();

    public List<InventoryItem> equipment;

    public List<InventoryItem> inventory;

    public List<InventoryItem> stash;

    [Header("Inventory UI")]
    [SerializeField]
    private Transform inventorySlotParent;

    [SerializeField]
    private Transform stashSlotParent;

    [SerializeField]
    private Transform equipmentSlotParent;

    private float armorCooldown;

    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;
    private UI_EquipmentSlot[] equipmentItemSlots;

    [Header("Items cooldown")]
    private float flaskCooldown;

    public Dictionary<ItemData, InventoryItem> inventoryDictionary;

    private UI_ItemSlot[] inventoryItemSlots;
    private float lastTimeUsedArmor;
    private float lastTimeUsedFlask;
    public Dictionary<ItemData, InventoryItem> stashDictionary;
    private UI_ItemSlot[] stashItemSlots;

    public void Awake()
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

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();

        inventoryItemSlots = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        stashItemSlots = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        equipmentItemSlots = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();

        AddStartingItems();
    }

    private void AddStartingItems()
    {
        for (var i = 0; i < startingItems.Count; i++)
        {
            AddItem(startingItems[i]);
        }
    }

    public void EquipItem(ItemData _item)
    {
        var newEquipment = _item as ItemData_Equipment;
        var newItem = new InventoryItem(newEquipment);

        ItemData_Equipment oldEquip = null;

        foreach (var item in equipmentDictionary)
            if (item.Key.equipmentType == newEquipment.equipmentType)
            {
                oldEquip = item.Key;
            }

        if (oldEquip != null)
        {
            UnequipItem(oldEquip);
            AddItem(oldEquip);
        }

        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.AddModifiers();

        RemoveItem(_item);
        UpdateSlotUI();
    }

    public void UnequipItem(ItemData_Equipment toDelete)
    {
        if (equipmentDictionary.TryGetValue(toDelete, out var value))
        {
            equipment.Remove(value);
            equipmentDictionary.Remove(toDelete);
            toDelete.RemoveModifiers();
        }
    }

    private void UpdateSlotUI()
    {
        for (var i = 0; i < equipmentItemSlots.Length; i++)
        {
            foreach (var item in equipmentDictionary)
                if (item.Key.equipmentType == equipmentItemSlots[i].slotType)
                {
                    equipmentItemSlots[i].UpdateSlot(item.Value);
                }
        }

        for (var i = 0; i < inventoryItemSlots.Length; i++)
        {
            inventoryItemSlots[i].CleanUpSlot();
        }

        for (var i = 0; i < stashItemSlots.Length; i++)
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
    }

    public void AddItem(ItemData _item)
    {
        if (_item.itemType == ItemType.Equipment)
        {
            AddToInventory(_item);
        }
        else if (_item.itemType == ItemType.Material)
        {
            AddToStash(_item);
        }

        UpdateSlotUI();
    }

    private void AddToStash(ItemData _item)
    {
        if (stashDictionary.TryGetValue(_item, out var value))
        {
            value.AddStack();
        }
        else
        {
            var newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictionary.Add(_item, newItem);
        }
    }

    private void AddToInventory(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out var value))
        {
            value.AddStack();
        }
        else
        {
            var newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
        }
    }

    public void RemoveItem(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out var value))
        {
            if (value.stackSize <= 1)
            {
                inventory.Remove(value);
                inventoryDictionary.Remove(_item);
            }
            else
            {
                value.RemoveStack();
            }
        }

        if (stashDictionary.TryGetValue(_item, out var stashValue))
        {
            if (stashValue.stackSize <= 1)
            {
                stash.Remove(value);
                stashDictionary.Remove(_item);
            }
            else
            {
                stashValue.RemoveStack();
            }
        }

        UpdateSlotUI();
    }

    public bool CanCraft(ItemData_Equipment _toCraftl, List<InventoryItem> _requiredMaterials)
    {
        var materialsToRemove = new List<InventoryItem>();
        for (var i = 0; i < _requiredMaterials.Count; i++)
        {
            if (stashDictionary.TryGetValue(_requiredMaterials[i].data, out var stashValue))
            {
                if (stashValue.stackSize < _requiredMaterials[i].stackSize)
                {
                    Debug.Log("Not enough materials in stash");
                    return false;
                }

                materialsToRemove.Add(stashValue);
            }
            else
            {
                Debug.Log("Not enough materials in stash");
                return false;
            }
        }

        for (var i = 0; i < materialsToRemove.Count; i++)
        {
            RemoveItem(materialsToRemove[i].data);
        }

        AddItem(_toCraftl);
        return true;
    }

    public List<InventoryItem> GetEquippedList()
    {
        return equipment;
    }

    public List<InventoryItem> GetStashedList()
    {
        return stash;
    }

    public ItemData_Equipment GetEquipment(EquipmentType _type)
    {
        ItemData_Equipment equipedItem = null;
        foreach (var item in equipmentDictionary)
            if (item.Key.equipmentType == _type)
            {
                equipedItem = item.Key;
            }

        return equipedItem;
    }

    public void UseFlask()
    {
        var currFlask = GetEquipment(EquipmentType.Flask);

        if (currFlask == null)
        {
            return;
        }

        var canUseFlask = Time.time > lastTimeUsedFlask + flaskCooldown;

        if (canUseFlask)
        {
            flaskCooldown = currFlask.itemCooldown;
            currFlask.Effect(null);
            lastTimeUsedFlask = Time.time;
        }
        else
        {
            Debug.Log("Flask on cooldown");
        }
    }

    public bool CanUseArmor()
    {
        var currArmor = GetEquipment(EquipmentType.Armor);
        if (Time.time > lastTimeUsedArmor + armorCooldown)
        {
            armorCooldown = currArmor.itemCooldown;
            lastTimeUsedArmor = Time.time;
            return true;
        }

        return false;
    }
}