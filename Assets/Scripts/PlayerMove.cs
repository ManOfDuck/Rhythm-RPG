using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMove : MonoBehaviour
{
    [Header("Player-Facing")]
    [SerializeField] public string moveName = "";
    [SerializeField] public string moveDescription = "";

    [Header("Stats")]
    [SerializeField] private List<DifficultyLevel> difficulties = new();

    [SerializeField]
    private int _currentDifficultyIndex;
    public int CurrentDifficultyIndex
    {
        get => _currentDifficultyIndex;
        set
        {
            if (difficulties.Count < value)
            {
                _currentDifficultyIndex = value;
            }
            else print("JESSE: Invalid difficulty selected");
        }

    }

    public DifficultyLevel CurrentDifficulty => difficulties[CurrentDifficultyIndex];

    private BattleManager battleManager;

    // Start is called before the first frame update
    void Start()
    {
        battleManager = BattleManager.Instance;
        // TESTING
        GoFightMode();
    }

    public void GoFightMode()
    {
        print("Minigame Started");
        CurrentDifficulty.minigame.StartMinigame(this);
    }

    public void MinigameFinished(float score)
    {
        print("Minigame Finished with score: " + score);
        battleManager.HurtEnemy(CurrentDifficulty.enemyDamageRange.CalcDamage(score));
        battleManager.HurtPlayer(CurrentDifficulty.playerDamageRange.CalcDamage(score));
        battleManager.HealPlayer(CurrentDifficulty.playerHealRange.CalcDamage(score));

        foreach (StatusChance statusChance in CurrentDifficulty.playerStatusChances){
            if (statusChance.RollStatus(score))
            {
                battleManager.InflictPlayerStatus(statusChance.status);
            }
        }

        foreach (StatusChance statusChacne in CurrentDifficulty.enemyStatusChances)
        {
            if (statusChacne.RollStatus(score))
            {
                battleManager.InflictEnemyStatus(statusChacne.status);
            }
        }
    }

    [System.Serializable]
    public class DifficultyLevel
    {
        public Minigame minigame;
        public DamageRange enemyDamageRange;
        public DamageRange playerDamageRange;
        public DamageRange playerHealRange;
        public List<StatusChance> playerStatusChances = new();
        public List<StatusChance> enemyStatusChances = new();
    }


    [System.Serializable]
    public class DamageRange
    {
        public Vector2 range;
        public float minScore;
        public float maxScore;
        public float variance;

        public float CalcDamage(float score)
        {
            if(score < minScore || score > maxScore)
                return 0;

            float baseScore = ((score - minScore) / (maxScore - minScore) + minScore) * (range.y - range.x);
            return baseScore + Random.Range(-variance, variance);
        }
    }

    [System.Serializable]
    public class StatusChance
    {
        public StatusEffect status;
        public Vector2 chanceRange;
        public float minScore;
        public float maxScore;

        public bool RollStatus(float score)
        {
            if (score < minScore || score > maxScore)
                return false;

            float chance = ((score - minScore) / (maxScore - minScore) + minScore) * (chanceRange.y - chanceRange.x);
            float roll = Random.Range(0, 1);
            return roll >= chance;
        }
    }
}
