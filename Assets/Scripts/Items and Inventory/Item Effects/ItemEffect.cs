using UnityEngine;

public class ItemEffect : ScriptableObject
{
    [TextArea]
    public string itemEffectDescription;

    public virtual void ExecuteEffect(Transform _enemyPosition)
    {
        Debug.Log("Effect executed.");
    }
}