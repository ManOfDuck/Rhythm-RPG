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
    private int m_AddedSampleDelay = 2000;

    private int m_ActiveCount = -1;
    private float m_Score = 0.0f;
    private int m_KeyPresses = 0;
    private int m_ExpectedPresses = 0;

    private Lane[] m_Lanes = new Lane[4];

    struct Lane
    {
        public Vector2 startPos;
        public Vector2 endPos;
        public KeyCode code;

        public Lane(Vector2 s, Vector2 e, KeyCode c)
        {
            startPos = s;
            endPos = e;
            code = c;
        }
    }

    class Drop
    {
        private GameObject m_DropInstance;
        private Lane m_Lane;
        private int m_StartSample;
        private int m_EndSample;
        private float m_FadeCounter;

        public Drop(GameObject dropInstance, Lane lane, int startSample, int endSample)
        {
            m_DropInstance = dropInstance;
            m_Lane = lane;
            m_StartSample = startSample;
            m_EndSample = endSample;
            m_FadeCounter = 0.0f;
        }

        /* Updates the drop based on where we are in the song, returns false if the drop is finished fading out */ 
        public bool Update(int currentSample)
        {
            // We have hit the bottom bound
            if (currentSample >= m_EndSample)
            {
                m_DropInstance.transform.position = m_Lane.endPos;

                // We are still fading out
                if (m_FadeCounter < 0.25f)
                {
                    float alpha = Mathf.Lerp(1.0f, 0.0f, m_FadeCounter / 0.25f);
                    m_DropInstance.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, alpha);
                    m_FadeCounter += Time.deltaTime;
                    return true;
                }

                // We are done fading out
                Destroy(m_DropInstance);
                return false;
            }

            // We have not hit the bottom bound and should move down
            m_DropInstance.transform.position = Vector2.Lerp(m_Lane.startPos, m_Lane.endPos, Margin(currentSample));
            return true;
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

        // TESTING ONLY
        //Invoke("TestActivate", 0.0f);
        //OnSessionFinished += AudioManager_OnSessionFinished;
    }

    //// TESTING ONLY
    //private void AudioManager_OnSessionFinished(object sender, float e)
    //{
    //    Debug.Log(e);
    //    Invoke("TestActivate", 0.0f);
    //}

    //// TESTING ONLY
    //private void TestActivate()
    //{
    //    Activate(8, new Vector2(-3, 6), new Vector2(-3, -4f), new Vector2(-1, 6), new Vector2(-1, -4f), new Vector2(1, 6), new Vector2(1, -4f), new Vector2(3, 6), new Vector2(3, -4f), KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4);
    //}

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
                GameObject instance = Instantiate(m_DropPrefab, spawnPoint, Quaternion.identity);
                m_Drops.Add(new Drop(instance, m_Lanes[m_SongCodes[m_CodeIndex] - 1], m_LastBeatSample, m_LastBeatSample + (m_SamplesPerBeat * 4)));
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
                    float percent = m_Score / ((m_KeyPresses > m_ExpectedPresses) ? m_KeyPresses : m_ExpectedPresses);
                    percent = (percent > 1.0f) ? (2.0f - percent) : percent;
                    OnSessionFinished?.Invoke(this, percent);
                }
                m_ActiveCount = -1;
            }
            return;
        }

        // Handle input
        Drop next = m_Drops[0];
        for (int f = 0; f < 4; f++)
        {
            if (Input.GetKeyDown(m_Lanes[f].code))
            {
                m_KeyPresses++;
                if (next.GetLane().code == m_Lanes[f].code)
                    m_Score += next.Margin(currentSample);
            }
        }
    }

    public void Activate(int beats, Vector2 s1, Vector2 e1, Vector2 s2, Vector2 e2, Vector2 s3, Vector2 e3, Vector2 s4, Vector2 e4, KeyCode c1, KeyCode c2, KeyCode c3, KeyCode c4)
    {
        m_ActiveCount = beats;
        m_Score = 0.0f;
        m_KeyPresses = 0;
        m_ExpectedPresses = beats;

        m_Lanes[0] = new Lane(s1, e1, c1);
        m_Lanes[1] = new Lane(s2, e2, c2);
        m_Lanes[2] = new Lane(s3, e3, c3);
        m_Lanes[3] = new Lane(s4, e4, c4);
    }
}
