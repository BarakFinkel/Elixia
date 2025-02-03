using UnityEngine;

public class Enemy_DeathBringerTriggers : EnemyAnimationTriggers
{
    private Enemy_DeathBringer deathBringer => GetComponentInParent<Enemy_DeathBringer>();

    private void Relocate() => deathBringer.FindPosition();

    private void MakeInvisible() => deathBringer.fx.MakeTransparent(true);
    
    private void MakeVisible() => deathBringer.fx.MakeTransparent(false);
}
