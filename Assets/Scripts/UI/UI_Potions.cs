using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Potions : MonoBehaviour
{
    [SerializeField] private Transform potionOne;
    private List<Image> potionOneImages = new List<Image>();

    [SerializeField] private Transform potionTwo;
    private List<Image> potionTwoImages = new List<Image>();

    [SerializeField] private Transform potionThree;
    private List<Image> potionThreeImages = new List<Image>();

    [SerializeField] private Transform potionFour;
    private List<Image> potionFourImages = new List<Image>();

    private bool[] unlockedPotions = new bool[4];
    private bool[] currentPotions = new bool[4];

    private void Start()
    {
        SetupPotionImages();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && unlockedPotions[0])
        {
            resetOnPotions();
            currentPotions[0] = true;
        }
        else if (Input.GetKeyDown(KeyCode.E) && unlockedPotions[1])
        {
            resetOnPotions();
            currentPotions[1] = true;
        }
        else if (Input.GetKeyDown(KeyCode.R) && unlockedPotions[2])
        {
            resetOnPotions();
            currentPotions[2] = true;
        }
        else if (Input.GetKeyDown(KeyCode.T) && unlockedPotions[3])
        {
            resetOnPotions();
            currentPotions[3] = true;
        }

        UpdateVisuals();
    }

    public void UnlockPotionOne()
    {
        potionOne.gameObject.SetActive(true);
        unlockedPotions[0] = true;
        currentPotions[0] = true;
    }

    public void UnlockPotionTwo()
    {
        potionTwo.gameObject.SetActive(true);
        unlockedPotions[1] = true;
    }

    public void UnlockPotionThree()
    {
        potionThree.gameObject.SetActive(true);
        unlockedPotions[2] = true;
    }

    public void UnlockPotionFour()
    {
        potionFour.gameObject.SetActive(true);
        unlockedPotions[3] = true;
    }

    public void UpdateVisuals()
    {
        if (unlockedPotions[0])
        {
            potionOneImages[0].gameObject.SetActive(!currentPotions[0]);
            potionOneImages[1].gameObject.SetActive(currentPotions[0]);
        }

        if (unlockedPotions[1])
        {
            potionTwoImages[0].gameObject.SetActive(!currentPotions[1]);
            potionTwoImages[1].gameObject.SetActive(currentPotions[1]);
        }
        
        if (unlockedPotions[2])
        {        
            potionThreeImages[0].gameObject.SetActive(!currentPotions[2]);
            potionThreeImages[1].gameObject.SetActive(currentPotions[2]);
        }
        
        if (unlockedPotions[2])
        {
            potionFourImages[0].gameObject.SetActive(!currentPotions[3]);
            potionFourImages[1].gameObject.SetActive(currentPotions[3]);
        }
    }

    public void resetOnPotions()
    {
        for (int i = 0; i < unlockedPotions.Length; i++)
        {
            if (unlockedPotions[i])
            {
                currentPotions[i] = false;
            }
        }
    }

    public void SetupPotionImages()
    {
        for (int i = 0; i < potionOne.childCount; i++)
        {
            potionOneImages.Add(potionOne.GetChild(i).GetComponent<Image>());
        }

        for (int i = 0; i < potionTwo.childCount; i++)
        {
            potionTwoImages.Add(potionTwo.GetChild(i).GetComponent<Image>());
        }

        for (int i = 0; i < potionThree.childCount; i++)
        {
            potionThreeImages.Add(potionThree.GetChild(i).GetComponent<Image>());
        }

        for (int i = 0; i < potionFour.childCount; i++)
        {
            potionFourImages.Add(potionFour.GetChild(i).GetComponent<Image>());
        }
    }
}
