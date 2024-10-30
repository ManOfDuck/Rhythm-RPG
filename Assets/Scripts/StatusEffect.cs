using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusEffect
{
    public enum Effect
    {
        Poisoned,
        Frozen,
        AttackUp,
        Attackdown
    }

    public Effect effect;
    public float intensity;
    public int length;
    public int TurnsRemaining { get; set; }

    public void Start()
    {
        TurnsRemaining = length;
    }
}
