using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider HPslider;

    [SerializeField] private Image syringeImage;
    [SerializeField] private Image dodgeImage;
    [SerializeField] private Image counterAttackImage;
    [SerializeField] private Image blackholeImage;
    [SerializeField] private Image swordImage;
    [SerializeField] private Image potionImage;

    [SerializeField] private TextMeshProUGUI currentSoulEssence;

    private SkillManager skills;
    private bool usingSword = true;

    private float swordCooldownTimer;
    private float potionCooldownTimer;
    private float currentPotionCooldown;

    private void Start()
    {
        if (playerStats != null)
        {
            playerStats.onHealthChanged += UpdateHealthUI;
        }

        skills = SkillManager.instance;
    }

    private void Update()
    {
        currentSoulEssence.text = PlayerManager.instance.GetCurrencyAmount().ToString("#,#");
        
        if (swordCooldownTimer > 0)
        {
            swordCooldownTimer = Mathf.Max(swordCooldownTimer - Time.deltaTime, 0);
        }

        if (potionCooldownTimer > 0)
        {
            potionCooldownTimer = Mathf.Max(potionCooldownTimer - Time.deltaTime, 0);
        }
        
        usingSword = PlayerManager.instance.player.canUseSwordSkill;
        if (usingSword)
        {
            potionImage.transform.parent.gameObject.SetActive(false);
            swordImage.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            swordImage.transform.parent.gameObject.SetActive(false);
            potionImage.transform.parent.gameObject.SetActive(true);                
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift) && skills.dodge.dodgeUnlocked)
        {
            SetCooldownOf(dodgeImage);
        }

        if (Input.GetKeyDown(KeyCode.F) && skills.counterAttack.counterAttackUnlocked)
        {
            SetCooldownOf(counterAttackImage);
        }

        if (Input.GetKeyDown(KeyCode.X) && skills.blackhole.blackholeUnlocked)
        {
            SetCooldownOf(blackholeImage);
        }

        CheckCooldownOf(syringeImage, Inventory.instance.syringeCooldown);
        CheckCooldownOf(dodgeImage, skills.dodge.cooldown);
        CheckCooldownOf(counterAttackImage, skills.counterAttack.cooldown);
        CheckCooldownOf(blackholeImage, skills.blackhole.cooldown);
        if (usingSword)
        {
            CheckCooldownOfSword(swordImage, skills.sword.cooldown);
        }
        else
        {
            CheckCooldownOfPotion(potionImage);
        }
    }

    private void UpdateHealthUI()
    {
        HPslider.maxValue = playerStats.GetMaxHealthValue();
        HPslider.value = playerStats.currentHealth;
    }

    private void SetCooldownOf(Image _image)
    {
        if (_image.fillAmount <= 0)
        {
            _image.fillAmount = 1;
        }
    }

    private void CheckCooldownOf(Image _image, float _cooldown)
    {
        if (_image.fillAmount > 0)
        {
            _image.fillAmount -= 1 / _cooldown * Time.deltaTime;
        }
    }

    private void CheckCooldownOfSword(Image _image, float _cooldown)
    {
        if (_image.fillAmount > 0)
        {
            _image.fillAmount = swordCooldownTimer / _cooldown;
        }
    }

    private void CheckCooldownOfPotion(Image _image)
    {
        if (_image.fillAmount > 0)
        {
            _image.fillAmount = potionCooldownTimer / currentPotionCooldown;
        }
    }

    public void SetCooldownForSyringe()
    {
        SetCooldownOf(syringeImage);
    }

    public void SetCooldownForSword()
    {
        SetCooldownOf(swordImage);
        swordCooldownTimer = skills.sword.cooldown;
    }

    public void SetCooldownForPotion(float _cooldown)
    {
        SetCooldownOf(potionImage);
        potionCooldownTimer = _cooldown;
        currentPotionCooldown = _cooldown;
    }
}
