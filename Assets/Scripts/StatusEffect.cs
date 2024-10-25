using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusEffect : MonoBehaviour
{
    public string displayName = "";
    public string description = "";

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
    public int turnsRemaining;

    public void Start()
    {
        turnsRemaining = length;
    }
}
