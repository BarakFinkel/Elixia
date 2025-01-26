using UnityEngine;
using UnityEngine.UI;

// Created as a singleton - since we don't want more than 1 to be active.
public class PotionEffectManager : MonoBehaviour
{
    public static PotionEffectManager instance;
    private KeyCode lastUsed = KeyCode.Q;

    [Header("Unlock Information")]
    [SerializeField] private UI_SkillTreeSlot secondPotionUnlockButton;
    [SerializeField] private UI_SkillTreeSlot thirdPotionUnlockButton;
    [SerializeField] private UI_SkillTreeSlot fourthPotionUnlockButton;
    public bool secondUnlocked { get; private set; }
    public bool thirdUnlocked { get; private set; }
    public bool fourthUnlocked { get; private set; }  

    // Skills
    public BasePotionEffect basePotion { get; private set; }
    public FirePotionEffect fire { get; private set; }
    public PoisonPotionEffect poison { get; private set; }
    public IcePotionEffect ice { get; private set; }
    public ArcanePotionEffect arcane { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        secondPotionUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSecondPotion);
        thirdPotionUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockThirdPotion);
        fourthPotionUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockFourthPotion);

        fire = GetComponent<FirePotionEffect>();
        poison = GetComponent<PoisonPotionEffect>();
        ice = GetComponent<IcePotionEffect>();
        arcane = GetComponent<ArcanePotionEffect>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            lastUsed = KeyCode.Q;
        }
        else if (Input.GetKeyDown(KeyCode.E) && secondUnlocked)
        {
            lastUsed = KeyCode.E;
        }
        else if (Input.GetKeyDown(KeyCode.R) && thirdUnlocked)
        {
            lastUsed = KeyCode.R;
        }
        else if (Input.GetKeyDown(KeyCode.T) && fourthUnlocked)
        {
            lastUsed = KeyCode.T;
        }
    }

    public BasePotionEffect CurrentEffect()
    {
        if (lastUsed == KeyCode.Q)
        {
            return fire;
        }

        if (lastUsed == KeyCode.E)
        {
            return poison;
        }

        if (lastUsed == KeyCode.R)
        {
            return ice;
        }

        if (lastUsed == KeyCode.T)
        {
            return arcane;
        }

        return null;
    }

    private void UnlockSecondPotion()
    {
        if (secondPotionUnlockButton.unlocked && !secondUnlocked)
        {
            secondUnlocked = true;
            SkillManager.instance.potion.potionsUI.UnlockPotionTwo();
        }
    }

    private void UnlockThirdPotion()
    {
        if (thirdPotionUnlockButton.unlocked && !thirdUnlocked)
        {
            thirdUnlocked = true;
            SkillManager.instance.potion.potionsUI.UnlockPotionThree();
        }
    }

    private void UnlockFourthPotion()
    {
        if (fourthPotionUnlockButton.unlocked && !fourthUnlocked)
        {
            fourthUnlocked = true;
            SkillManager.instance.potion.potionsUI.UnlockPotionFour();
        }
    }
}