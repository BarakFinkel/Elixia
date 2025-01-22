using UnityEngine;

[CreateAssetMenu(fileName = "Thunder Strike Effect", menuName = "Data/Item Effect/Thunder Strike")]
public class ThunderStrike_Effect : ItemEffect
{
    [SerializeField] private GameObject thunderStrikePrefab;
    [SerializeField] private float timeTillDestruction = 0.45f;
    [SerializeField] private int effectChance = 20;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        if (Random.Range(0,100) <= effectChance - 1)
        {
            GameObject newThunderStrike = Instantiate(thunderStrikePrefab, _enemyPosition.position, Quaternion.identity);
            Destroy(newThunderStrike, timeTillDestruction); 
        }  
    }
}
