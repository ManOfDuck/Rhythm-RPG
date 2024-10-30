using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Lane
{
    public Vector2 startPos;
    public Vector2 endPos;
    public float despawnPercent = 0.5f;
    public GameObject prefab;
    public KeyCode code;
    
    public Vector2 DespawnPosition => ((endPos - startPos) * despawnPercent) + startPos;
    public int Id { get; set; }
}
