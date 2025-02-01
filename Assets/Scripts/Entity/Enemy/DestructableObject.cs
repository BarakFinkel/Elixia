using UnityEngine;

public class DestructibleObject : Enemy
{

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
    }

    public override void Die()
    {
        base.Die();
        Destroy(gameObject);
    }
    

    protected override void OnDrawGizmos()
    {
    }
}