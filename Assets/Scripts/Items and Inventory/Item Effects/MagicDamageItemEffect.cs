using UnityEngine;

public class MagicDamageItemEffect : ItemEffect
{
    [SerializeField] private MagicType magicType;
    [SerializeField] private int baseDamage;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        CharacterStats playerStats = PlayerManager.instance.player.GetComponent<CharacterStats>();
        CharacterStats targetStats = _enemyPosition.GetComponent<CharacterStats>();

        if (playerStats != null && targetStats != null)
        {
            playerStats.DoMagicalDamage(targetStats, magicType, baseDamage);
        }
    }
}