// -----------------------------------------------------------------------
//  <copyright file="AutoMove.cs" company="DAIKI">
//      Copyright (c) DAIKI. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace KaleidoscopeParticle
{
    using UnityEngine;
    using System.Collections;

    public class AutoMove : MonoBehaviour
    {
        public Vector3 moveSpeed;
        public Vector3 rotSpeed;
        public float lessSpeed = 0.0f;
    	
        // Update is called once per frame
        void Update()
        {
            transform.localPosition += moveSpeed * Time.deltaTime;
            transform.localEulerAngles += rotSpeed * Time.deltaTime;

            moveSpeed -= moveSpeed * lessSpeed * Time.deltaTime;
            rotSpeed -= rotSpeed * lessSpeed * Time.deltaTime;
            //moveSpeed *= (1f-lessSpeed);
            //rotSpeed *= (1f-lessSpeed);
        }
    }
}
