using UnityEngine.EventSystems;

public class UI_CraftSlot : UI_ItemSlot
{
    private void OnEnable()
    {
        UpdateSlot(item);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        var craftData = item.data as ItemData_Equipment;

        Inventory.instance.CanCraft(craftData, craftData.craftMaterials);
    }
}