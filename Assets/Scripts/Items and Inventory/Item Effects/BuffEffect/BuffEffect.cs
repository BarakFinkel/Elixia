using UnityEngine;

[CreateAssetMenu(fileName = "Buff Effect", menuName = "Data/Item Effect/Buff Effect")]
public class BuffEffect : ItemEffect
{
    [SerializeField]
    private StatType buffType;

    [SerializeField]
    private int buffAmount;

    [SerializeField]
    private float buffDuration;

    private PlayerStats stats;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        stats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        stats.IncreaseStatBy(stats.GetStat(buffType), buffAmount, buffDuration);
    }
}