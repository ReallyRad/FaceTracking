using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Calcatz.Sequine {

    /// <summary>
    /// A specific type of SequineState where it plays an animation clip within a certain time range.
    /// 
    /// Note that in script, you can't get nor modify the SequineRangedState as they're not bound to clip.
    /// One clip might have multiple states to enable blending between the same clip at different time.
    /// </summary>
    public class SequineRangedState : SequineState {

        public event System.Action onStateFinished;

        private float m_minTimePercentage = 0f;
        private float m_maxTimePercentage = 1f;


        public SequineRangedState(AnimationMixerPlayable _animationMixer, int _inputPort) : base(_animationMixer, _inputPort) {
        }

        public SequineRangedState(AnimationMixerPlayable _animationMixer, int _inputPort, float _minTimePercentage, float _maxTimePercentage) : base(_animationMixer, _inputPort) {
            m_minTimePercentage = _minTimePercentage;
            m_maxTimePercentage = _maxTimePercentage;
        }

        public void ClampTime() {
            if (time < m_minTimePercentage * duration || time > m_maxTimePercentage * duration) {
                time = (m_minTimePercentage * duration);
            }
        }

        protected override void OnTimeIncremented() {
            float maxTime = m_maxTimePercentage * duration;
            if (time > maxTime) {
                float minTime = m_minTimePercentage * duration;
                float timeRange = maxTime - minTime;
                if (!m_completed) {
                    m_completed = true;
                    onComplete?.Invoke();
                }
                if (m_clipPlayable.IsValid()) {
                    time = maxTime;
                }
            }
        }

        public override void Finish() {
            base.Finish();
            onStateFinished?.Invoke();
        }

    }
}