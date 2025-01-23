using UnityEngine;

public class BasePotionEffect : MonoBehaviour
{
    [SerializeField]
    public GameObject potionPrefab;

    [SerializeField]
    public Color potionColor = new(1, 1, 1);

    public virtual void ActivatePotionEffect(GameObject potion)
    {
    }
}