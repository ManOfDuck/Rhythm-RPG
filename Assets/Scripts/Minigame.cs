using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame : ScriptableObject
{
    [SerializeField] public int defaultBeatLength;
    [SerializeField] private Lane lane1;
    [SerializeField] private Lane lane2;
    [SerializeField] private Lane lane3;
    [SerializeField] private Lane lane4;

    private PlayerMove linkedMove;

    public void StartMinigame(PlayerMove callback)
    {
        linkedMove = callback;
        // Start minigame
        // Link relevant event to MinigameFinished
    }

    public void MinigameFinished(float score)
    {
        linkedMove.MinigameFinished(score);
    }

    [System.Serializable]
    private class Lane
    {
        Vector2 startPos;
        Vector2 endPos;
        KeyCode key;
        GameObject prefab;
    }
}
