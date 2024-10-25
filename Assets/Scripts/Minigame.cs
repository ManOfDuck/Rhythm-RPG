using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Minigame")]
public class Minigame : ScriptableObject
{
    [SerializeField] public int beatLength;
    [SerializeField] private Lane lane1;
    [SerializeField] private Lane lane2;
    [SerializeField] private Lane lane3;
    [SerializeField] private Lane lane4;

    private PlayerMove linkedMove;

    public void StartMinigame(PlayerMove callback)
    {
        linkedMove = callback;
        AudioManager.Instance.Activate(beatLength, lane1.startPos, lane1.endPos, lane2.startPos, lane2.endPos,
            lane3.startPos, lane3.endPos, lane4.startPos, lane4.endPos, lane1.key, lane2.key, lane3.key, lane4.key);
        AudioManager.Instance.OnSessionFinished += MinigameFinished;
    }

    public void MinigameFinished(object sender, float score)
    {
        linkedMove.MinigameFinished(score);
    }

    [System.Serializable]
    private class Lane
    {
        public Vector2 startPos;
        public Vector2 endPos;
        public KeyCode key;
        public GameObject prefab;
    }
}
