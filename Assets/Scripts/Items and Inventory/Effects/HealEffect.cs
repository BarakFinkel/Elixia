using UnityEngine;

[CreateAssetMenu(fileName = "Heal effect", menuName = "Data/Item effect/Heal effect")]
public class HealEffect : ItemEffect
{
    [Range(0f, 1f)]
    [SerializeField]
    private float healPrecent;

    public override void ExecuteEffect(Transform _respawnPos)
    {
        var playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        var healAmount = Mathf.RoundToInt(playerStats.GetMaxHealthValue() * healPrecent);

        playerStats.IncreaseHealthBy(healAmount);
    }
}