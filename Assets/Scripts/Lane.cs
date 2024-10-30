using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Lane
{
    public Vector2 startPos;
    public Vector2 endPos;
    public GameObject prefab;
    public KeyCode code;
    
    public Vector2 MovementTarget => ((endPos - startPos) * 2) + startPos;
    public int Id { get; set; }

    public Lane(Vector2 s, Vector2 e, GameObject p, KeyCode c, int i)
    {
        startPos = s;
        endPos = e;
        prefab = p;
        code = c;
    }
}
