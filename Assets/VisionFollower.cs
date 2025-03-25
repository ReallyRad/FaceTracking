using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionFollower : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform; //TODO use findobjectoftype
    [SerializeField] private float distance = 3f;

    private void Update()
    {
        Vector3 targetPosition = _cameraTransform.position + _cameraTransform.forward * distance;
        transform.position += (targetPosition - transform.position) * 0.025f;
        transform.LookAt(_cameraTransform);
        transform.Rotate(0, 180f, 0);
    }
    
}
