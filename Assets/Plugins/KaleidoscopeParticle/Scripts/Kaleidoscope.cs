// -----------------------------------------------------------------------
//  <copyright file="Kaleidoscope.cs.cs" company="DAIKI">
//      Copyright (c) DAIKI. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace KaleidoscopeParticle
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Control kaleidoscope particle.
    /// </summary>
    public class Kaleidoscope : MonoBehaviour
    {
        public Transform baseObject;
        public int circleValue = 16;
        public bool xMirror = true;
        public Gradient color;
        public bool freezeRotation = false;
        public int ringValue = 1;
        public Vector3 ringShift;
        public float ringAngle = 0f;
        public float ringSize = 0f;

        ParticleSystem pe;
        ParticleSystem.Particle[] particles;
        ParticleSystemRenderer per;
        int mirror_mult = 1;
        int circleValueOld = 0;
        bool xMirrorOld = false;
        int ringValueOld = 0;

        // Use this for initialization
        void Start()
        {
            if (baseObject == null)
            {
                Debug.LogError("baseObject does not exist. Please assign GameObject to baseObject!");
            }

            pe = GetComponent<ParticleSystem>();
            per = GetComponent<ParticleSystemRenderer>();
            ResetParticles();

            if (baseObject)
            {
                ParticleUpdate();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (baseObject)
            {
                if (circleValue != circleValueOld || xMirror != xMirrorOld || ringValue != ringValueOld)
                {
                    ResetParticles();
                }
                ParticleUpdate();
            }
        }

        void ParticleUpdate()
        {
            if (circleValue <= 0)
            {
                return;
            }

            // circle
            float angle_dist = 360f / (float)circleValue;
            float ringAnglePlus = 0f;
            float ringSizePlus = 0f;
            Vector3 localBasePosition = baseObject.localPosition;
            for (int i = 0; i < ringValue; i++)
            {
                for (int j = 0; j < circleValue; j++)
                {
                    int n = i * circleValue + j;
                    float myangle = angle_dist * (float)j + ringAnglePlus;
                    particles[n].position = Vector3.Scale(MoveCircle(localBasePosition, myangle), transform.lossyScale);
                    particles[n].startSize = baseObject.lossyScale.magnitude + ringSizePlus;
                    if (freezeRotation)
                    {
                        float angle;
                        Vector3 axis;
                        baseObject.localRotation.ToAngleAxis(out angle, out axis);
                        particles[n].rotation = angle;
                        particles[n].axisOfRotation = axis;
                    }
                    else
                    {
                        float angle;
                        Vector3 axis;
                        Quaternion rot_origin = Quaternion.AngleAxis(myangle, Vector3.forward) * baseObject.localRotation;
                        rot_origin.ToAngleAxis(out angle, out axis);
                        particles[n].rotation = angle;
                        particles[n].axisOfRotation = axis;
                    }
                    // color
                    particles[n].startColor = color.Evaluate(pe.time / pe.main.duration);
                }
                localBasePosition += ringShift;
                ringAnglePlus += ringAngle;
                ringSizePlus += ringSize;
            }

            // mirror
            if (xMirror)
            {
                ringAnglePlus = 0f;
                localBasePosition = baseObject.localPosition;
                ringSizePlus = 0f;
                for (int i = 0; i < ringValue; i++)
                {
                    for (int j = 0; j < circleValue; j++)
                    {
                        int n = i * circleValue + j + circleValue * ringValue;
                        float myangle = angle_dist * (float)j + ringAnglePlus;
                        Vector3 c_pos = MoveCircle(localBasePosition, myangle);
                        c_pos.x *= -1;
                        particles[n].position = Vector3.Scale(c_pos, transform.lossyScale);
                        particles[n].startSize = baseObject.lossyScale.magnitude + ringSizePlus;
                        Quaternion rot_origin = Quaternion.AngleAxis(myangle, Vector3.forward) * baseObject.localRotation;
                        Vector3 angles = rot_origin.eulerAngles;
                        angles.x = -angles.x;
                        angles.y = -angles.y + 180f;

                        if (freezeRotation)
                        {
                            float angle;
                            Vector3 axis;
                            baseObject.localRotation.ToAngleAxis(out angle, out axis);
                            particles[n].rotation = angle;
                            particles[n].axisOfRotation = axis;
                        }
                        else
                        {
                            float angle;
                            Vector3 axis;
                            rot_origin = Quaternion.Euler(angles);
                            rot_origin.ToAngleAxis(out angle, out axis);
                            particles[n].rotation = angle;
                            particles[n].axisOfRotation = axis;
                        }
                        particles[n].startColor = particles[0].startColor;
                    }
                    localBasePosition += ringShift;
                    ringAnglePlus += ringAngle;
                    ringSizePlus += ringSize;
                }
            }

            pe.SetParticles(particles, particles.Length);
        }

        void ResetParticles()
        {
            if (circleValue < 0)
            {
                circleValue = 0;
            }

            if (ringValue < 1)
            {
                ringValue = 1;
            }

            circleValueOld = circleValue;
            xMirrorOld = xMirror;
            ringValueOld = ringValue;
            // create
            if (xMirror)
            {
                mirror_mult = 2;
            }
            else
            {
                mirror_mult = 1;
            }
            pe.Emit(circleValue * mirror_mult * ringValue);
            particles = new ParticleSystem.Particle[circleValue * mirror_mult * ringValue];
            pe.GetParticles(particles);
            pe.SetParticles(particles, particles.Length);
        }

        Vector3 MoveCircle(Vector3 before, float angle)
        {
            Vector3 after = before;
            after.x = before.x * Mathf.Cos(Mathf.PI / 180f * angle) - before.y * Mathf.Sin(Mathf.PI / 180f * angle);
            after.y = before.x * Mathf.Sin(Mathf.PI / 180f * angle) + before.y * Mathf.Cos(Mathf.PI / 180f * angle);
            return after;
        }

        public void SetMesh(Mesh mesh)
        {
            per.mesh = mesh;
        }

        public void SetMaterial(Material material)
        {
            per.material = material;
        }

    }
}