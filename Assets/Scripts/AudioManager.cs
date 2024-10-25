using System;
using System.Collections;
using System.Collections.Generic;
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
    private int m_AddedSampleDelay = 18000;

    private int m_ActiveCount = -1;
    private float m_Score = 0.0f;
    private int m_KeyPresses = 0;
    private int m_ExpectedPresses = 0;

    struct Drop
    {
        private GameObject m_DropInstance;
        private int m_DropCode;
        private int m_StartSample;
        private int m_EndSample;
        private float m_FadeCounter;

        public Drop(GameObject dropInstance, int dropCode, int startSample, int endSample)
        {
            m_DropInstance = dropInstance;
            m_DropCode = dropCode;
            m_StartSample = startSample;
            m_EndSample = endSample;
            m_FadeCounter = 0.0f;
        }

        /* Updates the drop based on where we are in the song, returns false if the drop is finished fading out */ 
        public bool Update(ref Drop d, int currentSample, float upperBound, float lowerBound)
        {
            // We have hit the bottom bound
            if (currentSample >= m_EndSample)
            {
                m_DropInstance.transform.position = new Vector2(m_DropInstance.transform.position.x, lowerBound);

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
            float newYPos = Mathf.Lerp(upperBound, lowerBound, Margin(currentSample));
            m_DropInstance.transform.position = new Vector2(m_DropInstance.transform.position.x, newYPos);
            return true;
        }

        /* Returns the percent of how close this drop is to being done, this can be over 100% if the drop has already finished */
        public float Margin(int currentSample)
        {
            return (float)(currentSample - m_StartSample) / (float)(m_EndSample - m_StartSample);
        }

        public int GetDropCode()
        {
            return m_DropCode;
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
        Invoke("TestActivate", 1.0f);
        OnSessionFinished += AudioManager_OnSessionFinished;
    }

    // TESTING ONLY
    private void AudioManager_OnSessionFinished(object sender, float e)
    {
        Debug.Log(e);
        Invoke("TestActivate", 1.0f);
    }

    // TESTING ONLY
    private void TestActivate()
    {
        Activate(8);
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
                GameObject instance = Instantiate(m_DropPrefab, spawnPoint, Quaternion.identity);
                m_Drops.Add(new Drop(instance, m_SongCodes[m_CodeIndex], m_LastBeatSample, m_LastBeatSample + (m_SamplesPerBeat * 4)));
                m_ActiveCount--;
            }

            m_CodeIndex++;
        }

        // Update the positions of all drops
        int i = 0;
        while (i < m_Drops.Count)
        {
            Drop d = m_Drops[i];
            if (!d.Update(ref d, currentSample, transform.position.y + 6, transform.position.y - 4))
                m_Drops.RemoveAt(i);
            else
            {
                m_Drops[i] = d;
                i++;
            }
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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_KeyPresses++;
            if (next.GetDropCode() == 1)
            {
                m_Score += next.Margin(currentSample);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_KeyPresses++;
            if (next.GetDropCode() == 2)
            {
                m_Score += next.Margin(currentSample);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            m_KeyPresses++;
            if (next.GetDropCode() == 3)
            {
                m_Score += next.Margin(currentSample);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            m_KeyPresses++;
            if (next.GetDropCode() == 4)
            {
                m_Score += next.Margin(currentSample);
            }
        }
    }

    public void Activate(int beats)
    {
        m_ActiveCount = beats;
        m_Score = 0.0f;
        m_KeyPresses = 0;
        m_ExpectedPresses = beats;
    }
}
