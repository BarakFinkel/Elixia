using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField]
    private ItemData[] possibleDrop;

    [SerializeField]
    private int dropAmount;

    [SerializeField]
    private GameObject dropPrefab;

    private readonly List<ItemData> dropList = new();

    public virtual void GenerateDrop()
    {
        for (var i = 0; i < possibleDrop.Length; i++)
        {
            if (Random.Range(0, 100) <= possibleDrop[i].dropChance)
            {
                dropList.Add(possibleDrop[i]);
            }
        }

        for (var i = 0; i < dropAmount; i++)
        {
            var randomItem = dropList[Random.Range(0, dropList.Count)];

            //dropList.Remove(randomItem);
            DropItem(randomItem);
        }
    }

    protected void DropItem(ItemData _itemData)
    {
        var newDrop = Instantiate(dropPrefab, transform.position, Quaternion.identity);
        var randomVelocity = new Vector2(Random.Range(-5, 5), Random.Range(15, 20));
        var dropScript = newDrop.GetComponent<ItemObject>();
        if (dropScript != null)
        {
            dropScript.SetupItem(_itemData, randomVelocity);
        }
    }
}