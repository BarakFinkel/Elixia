using UnityEngine;

public class IcePotionEffect : BasePotionEffect
{
    [SerializeField] private float enemyFreezeTime;

    public override void ActivatePotionEffect(GameObject potion)
    {
        Debug.Log($"Freeze blast made, freezing nearby enemies for {enemyFreezeTime}s!");
    }
}
