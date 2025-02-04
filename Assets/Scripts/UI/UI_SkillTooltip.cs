using TMPro;
using UnityEngine;

public class UI_SkillTooltip : UI_Tooltip
{
    [SerializeField]
    private TextMeshProUGUI skillName;

    [SerializeField]
    private TextMeshProUGUI skillCost;

    [SerializeField]
    private TextMeshProUGUI skillDescription;

    public void ShowToolTip(string _skillName, string _skillDescription, int _price)
    {
        skillName.text = _skillName;
        skillCost.text = "(Soul Essence Cost: " + _price.ToString("#,#") + ")";
        skillDescription.text = _skillDescription;
        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}