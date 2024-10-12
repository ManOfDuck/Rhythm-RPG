using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMove : ScriptableObject
{
    [SerializeField] public Vector2 enemyDamageRange = new(0, 100);
    [SerializeField] public Vector2 playerHealRange = new(0, 100);
    [SerializeField] public float inflictStatusAt = 50f;
    [SerializeField] public StatusEffect enemyStatus = null;
    [SerializeField] public StatusEffect playerStatus = null;
    [SerializeField] public PlayerMoveMinigame minigame = null;

    // Start is called before the first frame update
    void Start()
    {
        if(minigame != null)
        {
            
        } 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
