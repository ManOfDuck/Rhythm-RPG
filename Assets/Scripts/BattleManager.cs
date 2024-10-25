using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class BattleManager : MonoBehaviour
{
    public enum Phase
    {
        PlayerTurn,
        EnemyTurn,
        BattleOver
    }

    [SerializeField] public float maxPlayerHealth = 100;
    [SerializeField] public float maxEnemyHealth = 1000;

    public static BattleManager Instance { get; private set; } = null;

    public float playerHealth;
    public float enemyHealth;
    public List<StatusEffect> playerStatuses = new();
    public List<StatusEffect> enemyStatuses = new();

    public UnityEvent<Phase> phaseUpdated;
    public UnityEvent<float> playerHealed;
    public UnityEvent<float> playerHurt;
    public UnityEvent<float> enemyHealed;
    public UnityEvent<float> enemyHurt;
    public UnityEvent playerStatusesUpdated;
    public UnityEvent enemyStatusesUpdated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        playerHealth = maxPlayerHealth;
        enemyHealth = maxEnemyHealth;
    }

    public void HealPlayer(float amount)
    {
        playerHealth += amount;
        playerHealed.Invoke(amount);
        print("Player healed for " + amount + ", new health at " + playerHealth);
    }

    public void HurtPlayer(float amount)
    {
        playerHealth -= amount;
        playerHurt.Invoke(amount);
        print("Player hurt for " + amount + ", new health at " + playerHealth);
    }

    public void HealEnemy(float amount)
    {
        enemyHealth += amount;
        enemyHealed.Invoke(amount);
        print("Enemy healed for " + amount + ", new health at " + playerHealth);
    }

    public void HurtEnemy(float amount)
    {
        enemyHealth -= amount;
        print("Enemy hurt for " + amount + ", new health at " + playerHealth);
    }

    public void InflictPlayerStatus(StatusEffect status)
    {
        if (status == null) return;

        foreach(StatusEffect existingStatus in playerStatuses)
        {
            if (existingStatus.effect == status.effect)
            {
                existingStatus.TurnsRemaining = Mathf.Max(existingStatus.TurnsRemaining, status.length);
                existingStatus.intensity = Mathf.Max(existingStatus.intensity, status.intensity);
                playerStatusesUpdated.Invoke();
                return;
            }
        }
        playerStatuses.Add(status);
        playerStatusesUpdated.Invoke();
    }

    public void ClearPlayerStatus(StatusEffect status)
    {
        if (playerStatuses.Contains(status)){
            playerStatuses.Remove(status);
            playerStatusesUpdated.Invoke();
        }
    }

    public void InflictEnemyStatus(StatusEffect status)
    {
        if (status == null) return;

        foreach (StatusEffect existingStatus in enemyStatuses)
        {
            if (existingStatus.effect == status.effect)
            {
                existingStatus.TurnsRemaining = Mathf.Max(existingStatus.TurnsRemaining, status.length);
                existingStatus.intensity = Mathf.Max(existingStatus.intensity, status.intensity);
                enemyStatusesUpdated.Invoke();
                return;
            }
        }
        enemyStatuses.Add(status);
        enemyStatusesUpdated.Invoke();
    }

    public void ClearEnemyStatus(StatusEffect status)
    {
        if (enemyStatuses.Contains(status))
        {
            enemyStatuses.Remove(status);
            enemyStatusesUpdated.Invoke();
        }
    }
}
