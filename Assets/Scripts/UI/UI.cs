using System.Collections;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [Header("Game Over Screen")]
    [SerializeField] private UI_ScreenFade screenFade;
    [SerializeField] private GameObject youDiedMessage;
    [SerializeField] private float youDiedDisplayDelay = 1.0f;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private float restartButtonDisplayDelay = 2.0f;
    [Space]
    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject settingsUI;
    [SerializeField] private GameObject inGameUI;

    public UI_ItemTooltip itemTooltip;
    public UI_StatTooltip statTooltip;
    public UI_SkillTooltip skillTooltip;
    public UI_CraftWindow craftWindow;

    // Helps assign events to the skill tree slots before we assign events on the skill scripts
    private void Awake()
    {
        SwitchTo(skillTreeUI);
        screenFade.gameObject.SetActive(true);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        SwitchTo(inGameUI);

        itemTooltip.gameObject.SetActive(false);
        statTooltip.gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchWithKeyTo(characterUI);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            SwitchWithKeyTo(craftUI);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            SwitchWithKeyTo(skillTreeUI);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            SwitchWithKeyTo(settingsUI);
        }


    }

    // Responsible for switching to the desired menu tab on-click
    public void SwitchTo(GameObject _menu)
    {      
        for (var i = 0; i < transform.childCount; i++)
        {
            bool screenFade = transform.GetChild(i).GetComponent<UI_ScreenFade>() != null;

            if (!screenFade)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        if (_menu != null)
        {
            _menu.SetActive(true);
        }
    }

    public void SwitchWithKeyTo(GameObject _menu)
    {
        if (_menu != null && _menu.activeSelf)
        {
            _menu.SetActive(false);
            CheckForInGameUI();
            return;
        }
        else
        {
            SwitchTo(_menu);
        }
    }

    private void CheckForInGameUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<UI_ScreenFade>() == null && transform.GetChild(i).gameObject.activeSelf)
            {
                return;
            }
        }

        SwitchTo(inGameUI);
    }

    public void SwitchOnEndScreen()
    {
        screenFade.FadeOut();
        StartCoroutine(EndScreenCoroutine());
    }

    private IEnumerator EndScreenCoroutine()
    {
        // Wait and display the end message.
        yield return new WaitForSeconds(youDiedDisplayDelay);
        youDiedMessage.SetActive(true);

        // Wait and display the restart button.
        yield return new WaitForSeconds(restartButtonDisplayDelay);
        restartButton.SetActive(true);
    }

    public void RestartGameButton() => GameManager.instance.RestartScene();
}