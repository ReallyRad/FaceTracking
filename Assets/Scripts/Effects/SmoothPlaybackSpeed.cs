using UnityEngine;
using RenderHeads.Media.AVProVideo;
using System.Collections.Generic;
using System.Collections;

public class SmoothPlaybackSpeed : MonoBehaviour
{
    public MediaPlayer mediaPlayer;
    public float startPlaybackSpeed = 1.0f; // Initial playback speed
    public float targetPlaybackSpeed = 2.0f; // Target playback speed
    public float transitionDuration = 2.0f; // Duration of the transition in seconds
    public Camera videoCamera; // Camera that captures the video surface
    public int captureWidth = 1920; // Width of the captured frames
    public int captureHeight = 1080; // Height of the captured frames

    private List<Texture2D> frames = new List<Texture2D>();
    private bool isTransitioning = false;
    private int currentFrameIndex = 0;

    public float currentPlaybackSpeed;

    void Start()
    {
        videoCamera = Camera.main;
        if (videoCamera == null)
        {
            Debug.LogError("Video Camera is not assigned!");
            return;
        }
        else
        {
            Debug.Log("Video Camera is assigned: " + videoCamera.name);
        }

        // Make sure mediaPlayer and videoCamera are assigned properly
        if (mediaPlayer == null || videoCamera == null)
        {
            Debug.LogError("MediaPlayer or Video Camera is not assigned!");
            return;
        }

        // Set the initial playback speed
        mediaPlayer.Control.SetPlaybackRate(startPlaybackSpeed);

        // Start capturing frames from the video surface
        StartCoroutine(CaptureFrames());
    }

    void Update()
    {
        currentPlaybackSpeed = mediaPlayer.PlaybackRate;
        if (isTransitioning)
        {
            // Gradually change the playback speed over time
            float t = Mathf.Clamp01(Time.deltaTime / transitionDuration);
            float newPlaybackSpeed = Mathf.Lerp(mediaPlayer.Control.GetPlaybackRate(), targetPlaybackSpeed, t);
            mediaPlayer.Control.SetPlaybackRate(newPlaybackSpeed);

            // Check if the transition is complete
            if (Mathf.Approximately(newPlaybackSpeed, targetPlaybackSpeed))
            {
                isTransitioning = false;
            }
        }
        else
        {
            // Play frames in sequence
            if (frames.Count > 0)
            {
                videoCamera.targetTexture = new RenderTexture(frames[currentFrameIndex].width, frames[currentFrameIndex].height, 24);
                videoCamera.targetTexture.Create();

                // Display frames on a renderer
                videoCamera.targetTexture.Release();
                videoCamera.targetTexture.width = frames[currentFrameIndex].width;
                videoCamera.targetTexture.height = frames[currentFrameIndex].height;
                videoCamera.targetTexture.Create();
                Graphics.Blit(frames[currentFrameIndex], videoCamera.targetTexture);

                currentFrameIndex = (currentFrameIndex + 1) % frames.Count;
            }
        }
    }

    // Method to start the transition to a new playback speed
    public void ChangePlaybackSpeed(float targetSpeed)
    {
        targetPlaybackSpeed = targetSpeed;
        isTransitioning = true;
    }

    // Coroutine to capture frames from the video surface
    private IEnumerator CaptureFrames()
    {
        while (!mediaPlayer.Control.IsFinished())
        {
            yield return null; // Wait for the next frame

            RenderTexture rt = new RenderTexture(captureWidth, captureHeight, 24);
            videoCamera.targetTexture = rt;
            videoCamera.Render();

            Texture2D frameTexture = new Texture2D(captureWidth, captureHeight);
            RenderTexture.active = rt;
            frameTexture.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
            frameTexture.Apply();
            frames.Add(frameTexture);

            RenderTexture.active = null;
            videoCamera.targetTexture = null;

            Destroy(rt);
        }
    }
}
