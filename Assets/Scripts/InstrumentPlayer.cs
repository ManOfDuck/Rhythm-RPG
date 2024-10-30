using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class InstrumentPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip song;
    private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.Stop();
        source.clip = song;
        Mute();
    }

    public void StartPlaying(double startTime)
    {
        source.PlayScheduled(startTime);
    }

    public void Mute()
    {
        source.mute = true;
    }

    public void Unmute()
    {
       // source.timeSamples = currentSample;
        source.mute = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
