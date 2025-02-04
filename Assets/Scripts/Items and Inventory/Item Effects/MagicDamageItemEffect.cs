using UnityEngine;

public class MagicDamageItemEffect : ItemEffect
{
    [SerializeField]
    private MagicType magicType;

    [SerializeField]
    private int baseDamage;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        var playerStats = PlayerManager.instance.player.GetComponent<CharacterStats>();
        var targetStats = _enemyPosition.GetComponent<CharacterStats>();

        if (playerStats != null && targetStats != null)
        {
            playerStats.DoMagicalDamage(targetStats, magicType, baseDamage);
        }
    }
}