using System;
using Unity.VisualScripting;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    public DashSkill dash {get; private set;}

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.GameObject());
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        dash = GetComponent<DashSkill>();
    }
}
