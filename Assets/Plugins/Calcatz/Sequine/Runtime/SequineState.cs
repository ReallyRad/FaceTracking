using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Calcatz.Sequine {

    public delegate void OnStateUpdateCallback(float _normalizedLength);

    /// <summary>
    /// Manages how an animation clip behaves upon being played.
    /// </summary>
    public class SequineState {

        public Action onComplete;
        public OnStateUpdateCallback onUpdate;

        private AnimationMixerPlayable animationMixer;
        private AnimationClip m_clip;
        protected AnimationClipPlayable m_clipPlayable;
        private int m_inputPort;
        private float m_lengthToPlay = 1f;
        private float m_speed = 1.0f;

        private float m_fadeInDuration = 0.0f;
        private float m_fadeOutDuration = 0.0f;
        private float currentFadeInDuration;
        private float currentFadeOutDuration;

        private bool m_isExiting;
        private bool m_exited;

        private float m_time = 0.0f;
        private float m_duration = 0.0f;
        private float m_weight = 0.0f;
        protected bool m_completed;
        private bool m_paused;

        public float lengthToPlay { get => m_lengthToPlay; set => m_lengthToPlay = value; }
        public float speed { 
            get => m_speed;
            set {
                m_speed = value;
                //m_clipPlayable.SetSpeed(m_speed);
            }
        }
        public float time { 
            get => m_time;
            set {
                m_time = value;
                m_clipPlayable.SetTime(m_time);
            }
        }
        public float duration => m_duration;
        public float weight => m_weight;
        public bool isExiting => m_isExiting;
        /// <summary>
        /// Whether the state has been fully transitioned to the next state or not.
        /// </summary>
        public bool exited => m_exited;
        /// <summary>
        /// Whether the clip has been fully played according to the length to play or not.
        /// </summary>
        public bool completed => m_completed;

        public float fadeInDuration {
            get => m_fadeInDuration;
            set {
                if (value > m_duration) {
                    //Debug.LogWarning(m_clip.name + " has fadeInDuration that exceeds the total duration and is now automatically changed using the total duration value.");
                    value = m_duration;
                }
                m_fadeInDuration = value;
                currentFadeInDuration = value;
            }
        }
        public float fadeOutDuration {
            get => m_fadeOutDuration;
            set {
                if (value > m_duration) {
                    //Debug.LogWarning(m_clip.name + " has fadeOutDuration that exceeds the total duration and is now automatically changed using the total duration value.");
                    value = m_duration;
                }
                m_fadeOutDuration = value;
                currentFadeOutDuration = value;
            }
        }

        public float fadeInDurationNormalized {
            get => m_fadeInDuration / duration;
            set => fadeInDuration = value * duration;
        }
        public float fadeOutDurationNormalized {
            get => m_fadeOutDuration / duration;
            set => fadeOutDuration = value * duration;
        }

        public float normalizedTime {
            get => m_time / duration;
            set => time = value * duration;
        }

        /// <summary>
        /// Current played length according to lengthToPlay
        /// </summary>
        public float normalizedLength {
            get => m_time / duration * lengthToPlay;
        }

        public AnimationClip clip => m_clip;
        internal AnimationClipPlayable clipPlayable => m_clipPlayable;
        internal int inputPort => m_inputPort;

        public SequineState(AnimationMixerPlayable _animationMixer, int _inputPort) {
            animationMixer = _animationMixer;

            m_inputPort = _inputPort;
            m_clipPlayable = (AnimationClipPlayable)_animationMixer.GetInput(_inputPort);
            m_clip = m_clipPlayable.GetAnimationClip();
            m_duration = m_clip.length;
            m_clipPlayable.SetTime(0);
            //m_clipPlayable.SetSpeed(m_speed);
        }

        public void Reset() {
            Restart();
            ResetExitStates();
        }

        private void ResetExitStates() {
            m_isExiting = false;
            m_exited = false;
        }

        public void Restart() {
            time = 0.0f;
            m_completed = false;
        }

        public void Play() {
            if (m_fadeInDuration <= 0.0f) {
                // Move weight to 1 directly.
                m_weight = 1.0f;
            }
            m_completed = false;
            m_paused = false;
            ResetExitStates();
            m_clipPlayable.Play();
        }

        public void Stop() {
            m_weight = 0f;
            UpdateWeight(m_weight);
        }

        public void Pause() {
            if (m_paused) return;
            m_paused = true;
            m_clipPlayable.Pause();
        }

        public void Resume() {
            if (!m_paused) return;
            m_paused = false;
            m_clipPlayable.Play();
        }

        public void Exit() {
            m_isExiting = true;
        }

        public virtual void Finish() {
            m_exited = true;
            m_isExiting = false;
            m_weight = 0f;
            time = 0;
            UpdateWeight(m_weight);
        }

        internal void UpdateWeight(float _finalWeight) {
            animationMixer.SetInputWeight(m_clipPlayable, _finalWeight);
        }

        public void Tick(float _deltaTime) {
            float timeStep = _deltaTime * m_speed;

            if (!m_paused) {
                m_time += timeStep;
                OnTimeIncremented();
            }

            if (m_isExiting) {
                if (currentFadeOutDuration > 0) {
                    currentFadeOutDuration -= timeStep;
                    //Debug.Log(clip.name + ": " + currentFadeOutDuration);
                    float delta = (1 - currentFadeOutDuration / m_fadeOutDuration);
                    m_weight = 1.0f - delta;
                    if (m_weight < 0) m_weight = 0;
                }
                else {
                    Finish();
                }
            }
            else {
                if (m_exited) {
                    m_weight = 0;
                }
                else if (currentFadeInDuration > 0) {
                    currentFadeInDuration -= timeStep;
                    //Debug.Log(clip.name + ": " + currentFadeInDuration);
                    m_weight = 1 - currentFadeInDuration / m_fadeInDuration;
                    if (m_weight > 1) m_weight = 1;
                }
                else if (m_time >= m_fadeInDuration) {
                    m_weight = 1.0f;
                }
            }

            onUpdate?.Invoke(normalizedLength);

            if (normalizedTime >= lengthToPlay) {
                if (!m_completed) {
                    m_completed = true;
                    onComplete?.Invoke();
                }
                m_time = m_time % m_duration;
            }


            if (m_clipPlayable.IsValid()) {
                m_clipPlayable.SetSpeed(m_weight > 0 ? m_speed : 0);
            }
        }

        protected virtual void OnTimeIncremented() {
            
        }
    }

}