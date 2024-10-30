using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Minigame")]
public class Minigame : ScriptableObject
{
    private enum SpawnBehavior
    {
        SpawnOnBeat,
        ReachEndOnBeat
    }
    [SerializeField] GameObject stage;
    [SerializeField] public int beatLength;
    [SerializeField] public int beatsPerDrop = 4;
    [SerializeField] public float maxHitMargin = 0.15f;
    [SerializeField] private SpawnBehavior spawnBehavior = SpawnBehavior.ReachEndOnBeat;
    [SerializeField] private Lane lane1;
    [SerializeField] private Lane lane2;
    [SerializeField] private Lane lane3;
    [SerializeField] private Lane lane4;


    private PlayerMove linkedMove;
    private GameObject spawnedStage;

    public void StartMinigame(PlayerMove callback)
    {
        linkedMove = callback;
        bool spawnDropsEarly = spawnBehavior == SpawnBehavior.SpawnOnBeat;
        if (stage != null)
        {
            spawnedStage = Instantiate(stage);
        }
        AudioManager.Instance.Activate(beatLength, maxHitMargin, beatsPerDrop, spawnDropsEarly, lane1, lane2, lane3, lane4);
        AudioManager.Instance.OnSessionFinished += MinigameFinished;
    }

    public void MinigameFinished(object sender, float score)
    {
        if (spawnedStage != null)
        {
            Destroy(spawnedStage.gameObject);
        }
        linkedMove.MinigameFinished(score);
    }
}
