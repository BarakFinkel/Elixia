using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [Header("General Item Drop Prefab")]
    [SerializeField] private GameObject dropPrefab;
    
    [Header("Drop List Information")]
    [SerializeField] private int possibleAmountOfItems;
    [SerializeField] private ItemData[] possibleDrop;
    private List<ItemData> dropList = new List<ItemData>();

    [Header("Drop Velocity Information")]
    [SerializeField] private int xVelocity = 5;
    [SerializeField] private int minYVelocity = 15;
    [SerializeField] private int maxYVelocity = 20; 

    // This method generates drops from a possible drops list, and maximises it with the given amountOfItems value.
    // Each enemy death will cause this function to execute.
    public virtual void GenerateDrop()
    {
        for (int i = 0; i < possibleDrop.Length; i++)
        {
            if(Random.Range(0,100) <= possibleDrop[i].dropChance)
            {
                dropList.Add(possibleDrop[i]);
            }
        }

        // We drop the maximal ammount of items to drop.
        int itemsToDrop = Mathf.Min(possibleAmountOfItems, dropList.Count);

        for (int i = 0; i < itemsToDrop; i++)
        {
            ItemData randomItem = dropList[Random.Range(0, dropList.Count-1)];

            dropList.Remove(randomItem);
            DropItem(randomItem);
        }
    }

    public void DropItem(ItemData _item)
    {
        GameObject newDrop = Instantiate(dropPrefab, transform.position, Quaternion.identity);

        Vector2 randomVelocity = new Vector2(Random.Range(-xVelocity, xVelocity), Random.Range(minYVelocity, maxYVelocity));

        newDrop.GetComponent<ItemObject>().SetupItem(_item, randomVelocity);
    }
}
