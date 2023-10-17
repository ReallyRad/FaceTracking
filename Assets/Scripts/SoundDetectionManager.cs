using Oculus.Platform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDetectionManager : MonoBehaviour
{
    public delegate void OnExhaleSoundDetected();
    public static OnExhaleSoundDetected newExhaleSoundDetected;

    public delegate void OnSlightExhaleSoundDetected();
    public static OnSlightExhaleSoundDetected newSlightExhaleSoundDetected;

    public delegate void OnQuiet();
    public static OnQuiet newQuiet;

    private bool exhaleSound = false;
    private bool slightExhaleSound = false;
    private bool quiet = false;

    private bool wasExhaleSound = false;
    private bool wasSlightExhaleSound = false;
    private bool wasQuiet = false;

    private float loudness;
    private float loudnessMultiplier = 500f;
    private float loudnessThreshold = 0.005f;
    private float loudnessSlightThreshold = 0.001f;

    void Start()
    {
        if (UnityEngine.Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            StartMicrophone();
        }
        else
        {
            UnityEngine.Application.RequestUserAuthorization(UserAuthorization.Microphone);
        }
    }

    void Update()
    {
        wasSlightExhaleSound = slightExhaleSound;
        wasExhaleSound = exhaleSound;
        wasQuiet = quiet;

        loudness = GetLoudness();
        loudness = loudness * loudnessMultiplier;

        if (loudness >= loudnessThreshold) 
        {
            exhaleSound = true;
            slightExhaleSound = false;
            quiet = false;
        }
        else if (loudness < loudnessThreshold && loudness > loudnessSlightThreshold)
        {
            exhaleSound = false;
            slightExhaleSound = true;
            quiet = false;
        }
        else
        {
            exhaleSound = false;
            slightExhaleSound = false;
            quiet = true;
        }

        if (!wasSlightExhaleSound && slightExhaleSound) newSlightExhaleSoundDetected();
        if (!wasExhaleSound && exhaleSound) newExhaleSoundDetected();
        if (!wasQuiet && quiet) newQuiet();

    }

    float GetLoudness()
    {
        float[] spectrumData = new float[128];
        AudioSource audioSource = GetComponent<AudioSource>();

        // Get the spectrum data from the microphone input
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.Hamming);

        // Calculate the average loudness from the spectrum data
        float sum = 0f;
        for (int i = 0; i < spectrumData.Length; i++)
        {
            sum += spectrumData[i];
        }
        float averageLoudness = sum / spectrumData.Length;

        return averageLoudness;
    }
    void StartMicrophone()
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        audioSource.clip = Microphone.Start(null, true, 1, AudioSettings.outputSampleRate);
        audioSource.loop = true;
        while (!(Microphone.GetPosition(null) > 0)) { } // Wait until microphone is recording

        audioSource.Play();
    }
}
