using UnityEngine;

public class IceAnimationTriggers : MonoBehaviour
{
    public IceEffectController iceEffectController;

    private void Start()
    {
        iceEffectController = GetComponentInParent<IceEffectController>();
    }
    
    private void TriggerDamage()
    {
        iceEffectController.IceBlastDamage();
    }

    private void TriggerIceShards()
    {
        iceEffectController.SpawnIceShards();
    }

    private void TriggerDestruction()
    {
        iceEffectController.DestroyBlast();
    }
}
