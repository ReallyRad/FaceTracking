using UnityEngine;
using System.Collections;

// Basic demonstration of a music system that uses PlayScheduled to preload and sample-accurately
// stitch two AudioClips in an alternating fashion.  The code assumes that the music pieces are
// each 16 bars (4 beats / bar) at a tempo of 140 beats per minute.
// To make it stitch arbitrary clips just replace the line
//   nextEventTime += (60.0 / bpm) * numBeatsPerSegment
// by
//   nextEventTime += clips[flip].length;

public class SeamlessLoop : MonoBehaviour
{
    public float bpm;
    public int numBeatsPerSegment = 16;

    private double nextEventTime;
    [SerializeField] private AudioSource[] audioSources;
    
    private int currentClipIndex = 0;

    void Start()
    {
        nextEventTime = AudioSettings.dspTime + 4.0f;
    }

    void Update()
    {
        double time = AudioSettings.dspTime;

        if (time + 0.5f > nextEventTime)
        {
            // We are now approx. 1 second before the time at which the sound should play,
            // so we will schedule it now in order for the system to have enough time
            // to prepare the playback at the specified time. This may involve opening
            // buffering a streamed file and should therefore take any worst-case delay into account.
            audioSources[(currentClipIndex + 1) % audioSources.Length].PlayScheduled(nextEventTime);
            audioSources[currentClipIndex].SetScheduledEndTime(nextEventTime); 
            
            Debug.Log("Scheduled source " + currentClipIndex + " to start at time " + nextEventTime);

            // Place the next event 16 beats from here at a rate of 140 beats per minute
            nextEventTime += 60.0f / bpm * numBeatsPerSegment;

            // Flip between two audio sources so that the loading process of one does not interfere with the one that's playing out
            currentClipIndex = (currentClipIndex + 1) % audioSources.Length;
        }
    }
}