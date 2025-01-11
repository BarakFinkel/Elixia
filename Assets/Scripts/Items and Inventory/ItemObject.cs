using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField]
    private ItemData itemData;

    [SerializeField]
    private Rigidbody2D rb;

    private SpriteRenderer sr;


    private void Start()
    {
        SetupVisuals();
    }

    public void SetupItem(ItemData _itemData, Vector2 _velocity)
    {
        itemData = _itemData;
        rb.linearVelocity = _velocity;

        SetupVisuals();
    }

    public void PickUpItem()
    {
        if (!Inventory.instance.CanAddItem() && itemData.itemType == ItemType.Equipment)
        {
            rb.linearVelocity = new Vector2(0, 7); // pop up the item
            return;
        }

        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }


    private void SetupVisuals()
    {
        if (itemData == null)
        {
            return;
        }

        GetComponent<SpriteRenderer>().sprite = itemData.icon;
        gameObject.name = "Item object - " + itemData.itemName;
    }
}