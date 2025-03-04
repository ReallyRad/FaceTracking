using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GardenPsychedeliaController : MonoBehaviour
{
   [SerializeField] private float _effectRange;
   [SerializeField] private float _pseudoMovementRange;
   [SerializeField] private float _psychoBlendAmount;

   private LTDescr _shiftTween;
   private LTDescr _psychoBlendTween;
   
   private void OnEnable()
   {
      PostProcessingControl.PostProcessingProgressiveProgress += SetProgressiveProgress;
   }

   private void OnDisable()
   {
      PostProcessingControl.PostProcessingProgressiveProgress -= SetProgressiveProgress;
   }

   private void OnDestroy()
   {
      LeanTween.cancel(gameObject);
   }

   private void Start()
   {
      LeanTween.value(gameObject,-1, 1, 10)
         .setOnUpdate( val =>
         {
            GetComponent<MeshRenderer>().material.SetFloat("_Shift", val); //Make shift value rotate constantly
         }).setLoopClamp();
      
       LeanTween.value(gameObject,0, 1, 7)
         .setOnUpdate( val => 
         {
            GetComponent<MeshRenderer>().material.SetFloat("_PsychoBlend", val * _psychoBlendAmount); //Make shift value rotate constantly
         }).setLoopPingPong();   
   }

   private void SetProgressiveProgress(float val)
   {
      Debug.Log("set psychedelia value " + val);
      GetComponent<MeshRenderer>().material.SetFloat("_Multiply", val * _pseudoMovementRange); //pseudomovement
      GetComponent<MeshRenderer>().material.SetFloat("_Blender", val * _effectRange); //hue-based brightnes shifting
      _psychoBlendAmount = val;
   }
   
}
