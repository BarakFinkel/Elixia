using TMPro;
using UnityEngine;

public class UI_StatTooltip : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI statDescText;

    public void ShowTooltip(string _text)
    {
        statDescText.text = _text;
        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}