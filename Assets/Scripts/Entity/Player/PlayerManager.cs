using UnityEngine;

// Created as a singleton - since we don't want more than 1 to be active.
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    [SerializeField]
    public Player player;
    public int currency;


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

    public bool HaveEnoughCurrency(int _price)
    {
        if (_price > currency)
        {
            return false;
        }
        
        currency -= _price;
        return true;
    }

    public int GetCurrencyAmount() => currency;
}