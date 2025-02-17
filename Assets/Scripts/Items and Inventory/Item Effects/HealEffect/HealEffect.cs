using UnityEngine;

[CreateAssetMenu(fileName = "Heal Effect", menuName = "Data/Item Effect/Heal Effect")]
public class HealEffect : ItemEffect
{
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float healPercent;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        var playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        var healAmount = Mathf.RoundToInt(playerStats.GetMaxHealthValue() * healPercent);
        playerStats.IncreaseHealthBy(healAmount);
    }
}