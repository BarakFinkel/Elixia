using UnityEngine;

[CreateAssetMenu(fileName = "Buff effect", menuName = "Data/Item effect/Buff effect")]
public class BuffEffect : ItemEffect
{
    [SerializeField]
    private StatType buffType;

    [SerializeField]
    private int buffAmount;

    [SerializeField]
    private float buffDuration;

    private PlayerStats stats;


    public override void ExecuteEffect(Transform _respawnPos)
    {
        stats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        stats.IncreaseStatBy(buffAmount, buffDuration, stats.GetStat(buffType));
    }
}