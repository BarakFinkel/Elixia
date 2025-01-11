using TMPro;
using UnityEngine;

public class UI_ItemTooltip : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI itemNameText;
    [SerializeField]
    private TextMeshProUGUI itemTypeText;
    [SerializeField]
    private TextMeshProUGUI itemDescriptionText;

    void Start()
    {
        
    }

    public void ShowTooltip(ItemData_Equipment data)
    {
        itemNameText.text = data.itemName;
        itemTypeText.text = data.equipmentType.ToString();
        itemDescriptionText.text = data.GetDescription();
        
        gameObject.SetActive(true);
    }

    public void HideTooltip() => gameObject.SetActive(false);

   
}