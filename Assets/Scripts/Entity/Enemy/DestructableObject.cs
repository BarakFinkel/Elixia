public class DestructibleObject : Enemy
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
    }


    protected override void OnDrawGizmos()
    {
    }

    public override void Die()
    {
        base.Die();
        Destroy(gameObject);
    }
}