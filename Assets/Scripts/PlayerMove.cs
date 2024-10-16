using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class PlayerMove : MonoBehaviour
{
    [Header("Player-Facing")]
    [SerializeField] public string moveName = "";
    [SerializeField] public string moveDescription = "";

    [Header("Stats")]
    [SerializeField] public Vector2 enemyDamageRange = new(0, 100);
    [SerializeField] public Vector2 playerHealRange = new(0, 0);
    [SerializeField] public float inflictStatusAt = 50f;
    [SerializeField] public StatusEffect enemyStatus = null;
    [SerializeField] public StatusEffect playerStatus = null;

    private BattleManager battleManager;

    // Start is called before the first frame update
    void Start()
    {
        battleManager = BattleManager.Instance;
    }

    public abstract void StartMinigame();

    public void MinigameFinished(float score)
    {
        battleManager.HurtEnemy(MapScore(score, enemyDamageRange));
        battleManager.HealPlayer(MapScore(score, playerHealRange));
        if (score > inflictStatusAt)
        {
            battleManager.InflictEnemyStatus(enemyStatus);
            battleManager.InflictPlayerStatus(playerStatus);
        }
    }

    private float MapScore(float score, Vector2 outputRange)
    {
        return (score / 100) * outputRange.y - outputRange.x + outputRange.x;
    }
}
