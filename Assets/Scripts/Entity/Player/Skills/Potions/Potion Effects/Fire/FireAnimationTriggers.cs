using UnityEngine;

public class FireAnimationTriggers : MonoBehaviour
{
    private FireEffectController fireEffectController;

    private void Start()
    {
        fireEffectController = GetComponentInParent<FireEffectController>();
    }

    public void turnOnDamage()
    {
        fireEffectController.startDamage();
    }

    public void turnOffDamage()
    {
        fireEffectController.stopDamage();
    }

    public void destroyFlameTrigger()
    {
        fireEffectController.destroyFlame();
    }
}