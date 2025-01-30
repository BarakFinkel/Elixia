using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private ItemData itemData;

    private void SetupVisuals()
    {
        if (itemData == null)
        {
            return;
        }

        GetComponent<SpriteRenderer>().sprite = itemData.icon;
        gameObject.name = "Item Object - " + itemData.itemName;
    }

    public void SetupItem(ItemData _itemData, Vector2 _velocity)
    {
        itemData = _itemData;
        rb.linearVelocity = _velocity;

        SetupVisuals();
    }

    public void PickUpItem()
    {
        // If we're about to pick an equipment item but are out of inventory room, we won't pickup the item.
        if (!Inventory.instance.CanAddItem() && itemData.itemType == ItemType.Equipment)
        {
            rb.linearVelocity =
                new Vector2(0,
                    Random.Range(5,
                        7)); // Just a number to simulate trying to pick up the item but failing, won't be changed.
            return;
        }

        AudioManager.instance.PlaySFX(21, 0, null);
        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }
}