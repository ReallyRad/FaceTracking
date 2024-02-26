using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PseudomovementController : MonoBehaviour
{
   [SerializeField] private float _effectRange;
   
   private void OnEnable()
   {
      PostProcessingControl.PostProcessingProgress += SetMultiplierValue;
   }

   private void OnDisable()
   {
      PostProcessingControl.PostProcessingProgress -= SetMultiplierValue;
   }

   private void SetMultiplierValue(float progress)
   {
      Debug.Log("set multiplier value " + progress);
      GetComponent<MeshRenderer>().material.SetFloat("_Multiply", progress * _effectRange);
   }
}
