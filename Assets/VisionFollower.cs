using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionFollower : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float distance = 3f;

    public bool isCentered;
    
    private void Update()
    {
        Vector3 targetPosition = _cameraTransform.position + _cameraTransform.forward * distance;
        transform.position += (targetPosition - transform.position) * 0.025f;
        transform.LookAt(_cameraTransform);
    }
    
}
