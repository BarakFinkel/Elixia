using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftWindow : MonoBehaviour
{
    [SerializeField]
    private Image itemImage;

    [SerializeField]
    private TextMeshProUGUI itemName;

    [SerializeField]
    private TextMeshProUGUI itemDescription;

    [SerializeField]
    private Image[] materialsImages;

    [SerializeField]
    private Button craftButton;

    public void SetupCraftWindow(ItemData_Equipment _data)
    {
        craftButton.onClick.RemoveAllListeners();

        // We first make the icons and the stack ammount text translucent.
        for (var i = 0; i < materialsImages.Length; i++)
        {
            materialsImages[i].color = Color.clear;
            materialsImages[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
        }

        // We update the materials visuals.
        for (var i = 0; i < _data.craftingMaterials.Count; i++)
        {
            if (_data.craftingMaterials.Count > materialsImages.Length)
            {
                Debug.LogWarning("You cannot apply more than 4 different material types for crafting an item!");
            }

            // We update the current needed material's icon.
            materialsImages[i].sprite = _data.craftingMaterials[i].data.icon;
            materialsImages[i].color = Color.white;

            // We update the current needed material's text.
            var materialSlotText = materialsImages[i].GetComponentInChildren<TextMeshProUGUI>();
            materialSlotText.text = _data.craftingMaterials[i].stackSize.ToString();
            materialSlotText.color = Color.white;
        }

        itemImage.sprite = _data.icon;
        itemName.text = _data.itemName;
        itemDescription.text = _data.GetDescription(0);

        craftButton.onClick.AddListener(() => Inventory.instance.CanCraft(_data, _data.craftingMaterials));
    }
}