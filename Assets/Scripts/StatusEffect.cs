using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusEffect : MonoBehaviour
{
    public enum Effect
    {
        Poisoned,
        Frozen,
        AttackUp
    }

    public Effect effect;
    public int length;
    public int turnsRemaining;

    public void Start()
    {
        turnsRemaining = length;
    }
}
