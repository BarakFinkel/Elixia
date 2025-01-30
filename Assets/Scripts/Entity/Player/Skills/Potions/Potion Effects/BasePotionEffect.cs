using UnityEngine;

public class BasePotionEffect : MonoBehaviour
{
    [Header("Potion Information")]
    [SerializeField] public GameObject potionPrefab;
    [SerializeField] public Color potionColor = new(1, 1, 1);
    [SerializeField] public float cooldown;

    public virtual void ActivatePotionEffect(GameObject potion)
    {
    }
}