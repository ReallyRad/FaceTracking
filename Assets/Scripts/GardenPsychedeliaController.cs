using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GardenPsychedeliaController : MonoBehaviour
{
   [SerializeField] private float _effectRange;
   [SerializeField] private float _pseudoMovementRange;
   [SerializeField] private float _psychoBlendAmount;
   
   private void OnEnable()
   {
      PostProcessingControl.PostProcessingProgress += SetInteractivePsychedelism;
   }

   private void OnDisable()
   {
      PostProcessingControl.PostProcessingProgress -= SetInteractivePsychedelism;
   }

   private void Start()
   {
      LeanTween.value(-1, 1, 10)
         .setOnUpdate( val =>
         {
            GetComponent<MeshRenderer>().material.SetFloat("_Shift", val); //Make shift value rotate constantly
         }).setLoopClamp();
      
      LeanTween.value(0, _psychoBlendAmount, 13)
         .setOnUpdate( val => 
         {
            GetComponent<MeshRenderer>().material.SetFloat("_PsychoBlend", val); //Make shift value rotate constantly
         }).setLoopPingPong();
   }
   
   private void SetInteractivePsychedelism(float progress)
   {
      Debug.Log("set psychedelia value " + progress);
      GetComponent<MeshRenderer>().material.SetFloat("_Multiply", progress * _pseudoMovementRange); //pseudomovement
      GetComponent<MeshRenderer>().material.SetFloat("_Blender", progress * _effectRange); //hue-based brightnes shifting
   }

}
