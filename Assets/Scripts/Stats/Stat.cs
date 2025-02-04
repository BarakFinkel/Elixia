using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField]
    private int baseValue;

    public List<int> modifiers = new();

    public int GetValue()
    {
        var finalValue = baseValue;

        foreach (var modifier in modifiers) finalValue += modifier;

        return finalValue;
    }

    public void SetValue(int _value)
    {
        baseValue = _value;
    }

    public void AddModifier(int _modifier)
    {
        modifiers.Add(_modifier);
    }

    public void RemoveModifier(int _modifier)
    {
        modifiers.Remove(_modifier);
    }
}