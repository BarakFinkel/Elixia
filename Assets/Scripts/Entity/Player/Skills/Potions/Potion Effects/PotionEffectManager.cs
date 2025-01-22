using Unity.VisualScripting;
using UnityEngine;

// Created as a singleton - since we don't want more than 1 to be active.
public class PotionEffectManager : MonoBehaviour
{
    public static PotionEffectManager instance;

    // Skills
    public FirePotionEffect fire { get; private set; }
    public PoisonPotionEffect poison { get; private set; }
    public IcePotionEffect ice { get; private set; }
    public ArcanePotionEffect arcane { get; private set; }
    private KeyCode lastUsed = KeyCode.Q;

    private void Awake()
    {
        if(instance != null)
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
        else if(Input.GetKeyDown(KeyCode.E))
        {
            lastUsed = KeyCode.E;
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            lastUsed = KeyCode.R;
        }
        else if(Input.GetKeyDown(KeyCode.T))
        {
            lastUsed = KeyCode.T;
        }
    }

    public BasePotionEffect CurrentEffect()
    {
        if(lastUsed == KeyCode.Q)
        {
            return fire;
        }
        if(lastUsed == KeyCode.E)
        {
            return poison;
        }
        if(lastUsed == KeyCode.R)
        {
            return ice;
        }
        if(lastUsed == KeyCode.T)
        {
            return arcane;
        }

        return null;
    }
}