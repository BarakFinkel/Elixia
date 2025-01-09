using UnityEngine;

[CreateAssetMenu(fileName = "Ice&Fire Effect", menuName = "Data/Item effect/Ice&Fire Strike")]
public class IceAndFireEffect : ItemEffect
{
    [SerializeField]
    private GameObject iceAndFirePrefab;

    [SerializeField]
    private float xVelocity;

    public override void ExecuteEffect(Transform _respawnPos)
    {
        var player = PlayerManager.instance.player;

        var thirdAttack = player.primaryAttack.comboCount == 2;

        if (thirdAttack)
        {
            var newIceAndFire = Instantiate(iceAndFirePrefab, _respawnPos.position, player.transform.rotation);
            newIceAndFire.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(xVelocity * player.facingDir, 0);

            Destroy(newIceAndFire, 10);
        }
    }
}