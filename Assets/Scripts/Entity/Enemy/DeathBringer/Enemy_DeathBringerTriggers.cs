using UnityEngine;

public class Enemy_DeathBringerTriggers : EnemyAnimationTriggers
{
    private Enemy_DeathBringer deathBringer => GetComponentInParent<Enemy_DeathBringer>();

    private void Relocate() => deathBringer.FindPosition();

    private void MakeInvisible() => deathBringer.fx.MakeTransparent(true);
    
    private void MakeVisible() => deathBringer.fx.MakeTransparent(false);

    private void TeleportOutSFX() => AudioManager.instance.PlaySFX(51, 0, transform);

    private void TeleportInSFX() => AudioManager.instance.PlaySFX(50, 0, transform);

    private void FootStepOneSFX() => AudioManager.instance.PlaySFX(47, 0 , transform);

    private void FootStepTwoSFX() => AudioManager.instance.PlaySFX(48, 0 , transform);   
}
