using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public event EventHandler<float> OnSessionFinished;

    [SerializeField]
    private AudioClip m_Clip;

    [SerializeField]
    private int m_BPM = 120;

    [SerializeField]
    private GameObject m_DropPrefab;

    private AudioSource m_Source;

    private int m_SamplesPerBeat = 0;
    private int m_LastBeatSample = 0;
    private int m_NextBeatSample = 0;
    private double m_StartTime = 0.0;

    private List<Drop> m_Drops = new List<Drop>();

    private int[] m_SongCodes =
    {
        0, 0, 0, 0,     1, 2, 3, 4,     1, 2, 3, 4,     1, 2, 3, 4,
        1, 2, 3, 4,     1, 2, 3, 4,     1, 2, 3, 4,     1, 2, 3, 4,
        1, 2, 3, 4,     1, 2, 3, 4,     1, 2, 3, 4,     1, 2, 3, 4,
        1, 2, 3, 4,     1, 2, 3, 4,     1, 2, 3, 4,     1, 2, 3, 4
    };
    private int m_CodeIndex = 4;

    /* If the rendering is ahead of the music, increase this number */
    [SerializeField] public int m_AddedSampleDelay = 2000;

    private int m_ActiveCount = -1;
    private float m_Score = 0.0f;
    private int m_KeyPresses = 0;
    private int m_ExpectedPresses = 0;
    private float m_MaxMargin = 0.1f;

    private Lane[] m_Lanes = new Lane[4];

    class Drop
    {
        private GameObject m_DropInstance;
        private Lane m_Lane;
        private int m_StartSample;
        private int m_EndSample;
        private bool m_HitYet = false;
        private float m_MaxMargin = 0.0f;
        private int FirstHittableSample => m_EndSample - (int) (m_MaxMargin * (m_EndSample - m_StartSample));
        private int LastHittableSample => m_EndSample + (int) (m_MaxMargin * (m_EndSample - m_StartSample));
        private int DespawnSample => m_StartSample + (int)(m_Lane.despawnPercent * (m_EndSample - m_StartSample));

        public Drop(GameObject dropInstance, Lane lane, int startSample, int endSample, float maxMargin = 0.15f)
        {
            m_DropInstance = dropInstance;
            m_Lane = lane;
            m_StartSample = startSample;
            m_EndSample = endSample;
            m_MaxMargin = maxMargin;
        }

        /* Updates the drop based on where we are in the song, returns false if the drop is finished fading out */ 
        public bool Update(int currentSample)
        {
            m_DropInstance.transform.position = Vector2.Lerp(m_Lane.startPos, m_Lane.DespawnPosition, Margin(currentSample) / m_Lane.despawnPercent);
            if (currentSample > DespawnSample){
                Hit(-1, -1);
            }
            return !m_HitYet;
        }

        /* Returns the percent of how close this drop is to being done, this can be over 100% if the drop has already finished */
        public float Margin(int currentSample)
        {
            return (float)(currentSample - m_StartSample) / (float)(m_EndSample - m_StartSample);
        }

        public Lane GetLane()
        {
            return m_Lane;
        }

        public bool DoesClickHit()
        {
            if (m_DropInstance.TryGetComponent<Collider2D>(out Collider2D collider))
            {
                Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                return collider.OverlapPoint(mousePoint);
            }
            else return true;
        }

        public bool CanBeHit(int currentSample)
        {
            return FirstHittableSample < currentSample && currentSample < LastHittableSample;
        }

        public void Hit(float dropPercentage, float score)
        {
            m_HitYet = true;
            if (m_DropInstance.TryGetComponent<DropInstance>(out DropInstance instance))
            {
                instance.Hit(dropPercentage, score);
            }
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set up the audio source component
        m_Source = GetComponent<AudioSource>();
        m_Source.Stop();
        m_Source.clip = m_Clip;

        // Calculate the samples per beat and handle delay
        m_SamplesPerBeat = (int)(m_Clip.frequency / (m_BPM / 60.0f));
        m_NextBeatSample = m_AddedSampleDelay;

        // Set a start time and schedule the song to start at that time
        m_StartTime = AudioSettings.dspTime + 1.0f;
        m_Source.PlayScheduled(m_StartTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (AudioSettings.dspTime < m_StartTime)
            return;

        int currentSample = m_Source.timeSamples;

        // Execute logic for hitting a beat
        if (currentSample >= m_NextBeatSample)
        {
            m_LastBeatSample = m_NextBeatSample;
            m_NextBeatSample += m_SamplesPerBeat;

            if (m_CodeIndex == m_SongCodes.Length)
                return;

            if (m_SongCodes[m_CodeIndex] == 0)
            {
                m_CodeIndex++;
                return;
            }

            // Spawn the beat in a position based on the value of the current song code
            if (m_ActiveCount > 0)
            {
                Vector2 spawnPoint = new Vector2(transform.position.x + ((m_SongCodes[m_CodeIndex] - 2.5f) * 2), transform.position.y);
                Lane lane = m_Lanes[m_SongCodes[m_CodeIndex] - 1];
                GameObject instance = Instantiate(lane.prefab, spawnPoint, Quaternion.identity);
                m_Drops.Add(new Drop(instance, lane, m_LastBeatSample, m_LastBeatSample + (m_SamplesPerBeat * 4), m_MaxMargin));
                m_ActiveCount--;
            }

            m_CodeIndex++;
        }

        // Update the positions of all drops
        int i = 0;
        while (i < m_Drops.Count)
        {
            if (!m_Drops[i].Update(currentSample))
                m_Drops.RemoveAt(i);
            else
                i++;
        }

        // Handle being out of active drops
        if (m_Drops.Count == 0)
        {
            // Fire the session score if all drops have been handeled
            if (m_ActiveCount == 0)
            {
                if (m_KeyPresses == 0)
                    OnSessionFinished?.Invoke(this, 0.0f);
                else
                {
                    print("Key Presses:" + m_KeyPresses);
                    print("Expected Presses:" + m_ExpectedPresses);
                    print("overall score: " + m_Score);
                    float percent = m_Score / ((m_KeyPresses > m_ExpectedPresses) ? m_KeyPresses : m_ExpectedPresses);
                    print("percent: " + percent);
                    //percent = (percent > 1.0f) ? (2.0f - percent) : percent;
                    OnSessionFinished?.Invoke(this, percent);
                }
                m_ActiveCount = -1;
            }
            return;
        }

        // Handle input
        List<KeyCode> codesCounted = new();
        for (int f = 0; f < 4; f++)
        {
            Lane lane = m_Lanes[f];
            if (Input.GetKeyDown(lane.code))
            {
                if (!codesCounted.Contains(lane.code))
                {
                    m_KeyPresses++;
                    codesCounted.Add(lane.code);
                }
            }
        }

        foreach (Drop drop in m_Drops)
        {
            if (drop.CanBeHit(currentSample) && Input.GetKeyDown(drop.GetLane().code))
            {
                if (drop.GetLane().code == KeyCode.Mouse0 && !drop.DoesClickHit())
                {
                    continue;
                }
                float percentDone = drop.Margin(currentSample);
                float dropScore = percentDone > 1.0f ? 2.0f - percentDone : percentDone;
                print("drop score: " + dropScore);
                m_Score += dropScore;
                drop.Hit(percentDone, dropScore);
                break;
            }
        }
    }

    public void Activate(int beats, float maxMargin, Lane lane1, Lane lane2, Lane lane3, Lane lane4)
    {
        m_ActiveCount = beats;
        m_Score = 0.0f;
        m_KeyPresses = 0;
        m_ExpectedPresses = beats;
        m_MaxMargin = maxMargin;

        m_Lanes[0] = lane1;
        lane1.Id = 0;
        m_Lanes[1] = lane2;
        lane2.Id = 1;
        m_Lanes[2] = lane3;
        lane3.Id = 2;
        m_Lanes[3] = lane4;
        lane4.Id = 3;
    }
}
