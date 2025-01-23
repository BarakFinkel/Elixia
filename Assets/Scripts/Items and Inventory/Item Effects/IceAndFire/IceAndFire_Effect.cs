using UnityEngine;

[CreateAssetMenu(fileName = "Ice & Fire Effect", menuName = "Data/Item Effect/Ice and Fire")]
public class IceAndFire_Effect : ItemEffect
{
    [SerializeField]
    private GameObject iceAndFirePrefab;

    [SerializeField]
    private float xVelocity = 15.0f;

    [SerializeField]
    private float timeTillDestruction = 5.0f;

    public override void ExecuteEffect(Transform _spawnPosition)
    {
        var player = PlayerManager.instance.player;

        var thirdAttack = player.primaryAttackState.comboCounter == 2;

        if (thirdAttack)
        {
            var newIceAndFire = Instantiate(iceAndFirePrefab, _spawnPosition.position, player.transform.rotation);
            newIceAndFire.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(xVelocity * player.facingDir, 0);

            Destroy(newIceAndFire, timeTillDestruction);
        }
    }
}