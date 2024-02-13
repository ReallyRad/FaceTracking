using System;
using System.Collections;
using System.Collections.Generic;
using RenderHeads.Media.AVProVideo;
using UnityEngine;

public class MediaPlayerControl : MonoBehaviour
{
   [SerializeField] private MediaPlayer _mediaPlayer;
   
   private void OnEnable()
   {
      PostProcessingControl.PostProcessingCompleted += PauseVideoAndFadeOutVolume;
   }

   private void OnDisable()
   {
      PostProcessingControl.PostProcessingCompleted -= PauseVideoAndFadeOutVolume;
   }

   private void PauseVideoAndFadeOutVolume()
   {
      LeanTween
         .value(1, 0, 5)
         .setOnUpdate(val =>
         {
            _mediaPlayer.AudioVolume = val;
         })
         .setOnComplete(() =>
         {
            _mediaPlayer.Pause();
         });
   }
}
