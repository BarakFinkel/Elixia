using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillTreeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISaveManager
{
    [SerializeField]
    private string skillName;

    [SerializeField]
    private Color lockedSkillColor;

    [SerializeField]
    private int skillPrice;

    [TextArea]
    [SerializeField]
    private string skillDescription;

    [SerializeField]
    private UI_SkillTreeSlot[] shouldBeUnlocked;

    [SerializeField]
    private UI_SkillTreeSlot[] shouldBeLocked;

    [HideInInspector]
    public bool unlocked;

    private Image skillImage;
    private UI ui;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => UnlockSkillSlot());
    }

    private void Start()
    {
        skillImage = GetComponent<Image>();
        ui = GetComponentInParent<UI>();

        skillImage.color = lockedSkillColor;

        if (unlocked)
        {
            skillImage.color = Color.white;
        }
    }

    private void OnValidate()
    {
        gameObject.name = "SkillTreeSlot_UI: " + skillName;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillTooltip.ShowToolTip(skillName, skillDescription, skillPrice);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillTooltip.HideTooltip();
    }

    public void LoadData(GameData _data)
    {
        if (_data.skillTree.TryGetValue(skillName, out var value))
        {
            unlocked = value;
        }
    }

    public void SaveData(ref GameData _data)
    {
        if (_data.skillTree.TryGetValue(skillName, out var value))
        {
            _data.skillTree.Remove(skillName);
            _data.skillTree.Add(skillName, unlocked);
        }
        else
        {
            _data.skillTree.Add(skillName, unlocked);
        }
    }

    public void UnlockSkillSlot()
    {
        if (unlocked || !PlayerManager.instance.HaveEnoughCurrency(skillPrice))
        {
            return;
        }

        for (var i = 0; i < shouldBeUnlocked.Length; i++)
        {
            if (shouldBeUnlocked[i].unlocked == false)
            {
                return;
            }
        }

        for (var i = 0; i < shouldBeLocked.Length; i++)
        {
            if (shouldBeLocked[i].unlocked)
            {
                return;
            }
        }

        unlocked = true;
        skillImage.color = Color.white;
    }
}