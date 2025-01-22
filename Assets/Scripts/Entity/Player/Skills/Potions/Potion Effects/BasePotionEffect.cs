using System.Runtime.CompilerServices;
using UnityEngine;

public class BasePotionEffect : MonoBehaviour
{
    [SerializeField] public GameObject potionPrefab;
    [SerializeField] public Color potionColor = new Color(1,1,1);

    public virtual void ActivatePotionEffect(GameObject potion) {}
}
