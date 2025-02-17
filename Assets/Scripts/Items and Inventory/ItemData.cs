using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum ItemType
{
    Material,
    Equipment
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public string itemID;
    public ItemType itemType;
    public string itemName;
    public Sprite iconTransparent;
    public Sprite iconBackground;

    [Range(0, 100)]
    public float dropChance;

    protected StringBuilder sb = new();

    private void OnValidate()
    {
#if UNITY_EDITOR

        var path = AssetDatabase.GetAssetPath(this);
        itemID = AssetDatabase.AssetPathToGUID(path);

#endif
    }

    public virtual string GetDescription(int minDescriptionLength)
    {
        return "";
    }
}