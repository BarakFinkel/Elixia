using UnityEngine;

public class FirePotionEffect : BasePotionEffect
{
    [SerializeField]
    private GameObject flamePrefab;

    [SerializeField]
    private int damage;

    [SerializeField]
    private float tickDuration;

    [SerializeField]
    private float spawnYOffset;

    private GameObject currentFlame;

    public override void ActivatePotionEffect(GameObject potion)
    {
        CreateFlame(potion);
    }

    public void CreateFlame(GameObject obj)
    {
        var offset = new Vector3(0, spawnYOffset, 0);
        currentFlame = Instantiate(flamePrefab, obj.transform.position + offset, Quaternion.identity);

        var currentFlameScript = currentFlame.GetComponent<FireEffectController>();
        currentFlameScript.SetupFlame(damage, tickDuration);
    }
}