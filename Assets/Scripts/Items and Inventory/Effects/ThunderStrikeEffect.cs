using UnityEngine;

[CreateAssetMenu(fileName = "Thunder Strike Effect", menuName = "Data/Item effect/Thunder Strike")]
public class ThunderStrikeEffect : ItemEffect
{
    [SerializeField]
    private GameObject thhunderStrikePrefab;

    public override void ExecuteEffect(Transform _respawnPos)
    {
        var newThunderStrike = Instantiate(thhunderStrikePrefab, _respawnPos.position, Quaternion.identity);

        Destroy(newThunderStrike, 1);
    }
}