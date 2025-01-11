using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StatSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private string statName;
    [SerializeField]
    private StatType statType;
    [SerializeField]
    private TextMeshProUGUI statValueText;
    [SerializeField]
    private TextMeshProUGUI statNameText;
    
    [TextArea]
    [SerializeField]
    private string statDescription;

    private UI ui;
    
    private void OnValidate()
    {
        gameObject.name = "Stat - " + statName;

        if (statNameText != null)
        {
            statNameText.text = statName;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ui = GetComponentInParent<UI>();
        UpdateStatValueUI();
    }

    public void UpdateStatValueUI()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if (playerStats != null)
        {
            statValueText.text = playerStats.GetStat(statType).GetValue().ToString();
        }

        if (statType == StatType.health)
        {
            statValueText.text = playerStats.GetMaxHealthValue().ToString();
        }else if (statType == StatType.damage)
        {
            statValueText.text = (playerStats.damage.GetValue() + playerStats.strength.GetValue()).ToString();
        }else if (statType == StatType.critPower)
        {
            statValueText.text = (playerStats.critPower.GetValue() + playerStats.strength.GetValue()).ToString();
        }else if (statType == StatType.critChance)
        {
            statValueText.text = (playerStats.critChance.GetValue() + playerStats.agility.GetValue()).ToString();
        }else if (statType == StatType.evasion)
        {
            statValueText.text = (playerStats.evasion.GetValue() + playerStats.agility.GetValue()).ToString();
        }else if (statType == StatType.magicResist)
        {
            statValueText.text = (playerStats.magicResist.GetValue() + playerStats.intelligence.GetValue() * 3).ToString();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (statDescription.Length > 0)
        {
            ui.statTooltip.ShowTooltip(statDescription);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.statTooltip.HideTooltip();
    }
}
