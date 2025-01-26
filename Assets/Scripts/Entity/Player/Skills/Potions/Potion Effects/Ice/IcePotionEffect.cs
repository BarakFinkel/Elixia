using UnityEngine;

public class IcePotionEffect : BasePotionEffect
{
    [Header("Ice Blast Information")]
    [SerializeField] public GameObject iceBlastPrefab;
    [SerializeField] private float iceBlastYOffset;

    [Header("Ice Shard Information")]
    [SerializeField] public GameObject iceShardPrefab;
    [SerializeField] private float iceShardOffset;
    [SerializeField] private float iceShardVelocity;
    [SerializeField] private float iceShardSmallestScale;
    [SerializeField] private float iceShardDuration;
    [SerializeField] private float enemyFreezeTime;

    private GameObject currentIceBlast;
    private GameObject currentIceShard;

    public override void ActivatePotionEffect(GameObject potion)
    {
        CreateIceBlast(potion);
    }

    public void CreateIceBlast(GameObject obj)
    {
        Vector3 offset = new Vector3(0, iceBlastYOffset, 0);
        currentIceBlast = Instantiate(iceBlastPrefab, obj.transform.position + offset, Quaternion.identity);

        IceEffectController currentIceBlastScript = currentIceBlast.GetComponent<IceEffectController>();
        currentIceBlastScript.SetupIceBlast(enemyFreezeTime);
    }

    // Spawns 8 ice shards around the game object that spawns them.
    public void CreateIceShards(Transform _transfrom)
    {
        float angleStep = 360.0f / 8.0f;
        Vector3 centerPosition = _transfrom.position;
        
        for (int i = 0; i < 8; i++)
        {
            // Calculate the angle for this shard
            float angle = i * angleStep;

            // Convert the angle to a direction vector
            Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0).normalized;

            // Calculate the position of the shard
            Vector3 shardPosition = centerPosition + direction * iceShardOffset;

            // Adjust the rotation based on the direction (angle in degrees)
            float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Instantiate ice shard prefab
            currentIceShard = Instantiate(iceShardPrefab, shardPosition, Quaternion.Euler(0,0,rotationZ));

            // Setup the ice shard controller
            IceShardController currentIceShardScript = currentIceShard.GetComponent<IceShardController>();
            currentIceShardScript.SetupIceShard(direction * iceShardVelocity, iceShardSmallestScale, enemyFreezeTime, iceShardDuration);
        }
    }
}