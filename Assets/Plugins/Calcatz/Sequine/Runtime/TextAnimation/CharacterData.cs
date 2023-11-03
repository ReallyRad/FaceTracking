using System;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Text's character data, used to track the text animation.
    /// </summary>
    [Serializable]
    public struct CharacterData {

        private float m_progress;
        public float progress {
            get { return m_progress; }
        }

        private float m_startingTime;
        private float m_duration;

        private int m_materialIndex;
        public int materialIndex {
            get {
                return m_materialIndex;
            }
        }

        private int m_vertexIndex;
        public int vertexIndex {
            get {
                return m_vertexIndex;
            }
        }

        private int m_index;
        public int index {
            get {
                return m_index;
            }
        }

        public CharacterData(int _index, float _startingTime, float _duration, int _materialIndex, int _vertexIndex) {
            m_index = _index;
            m_progress = 0.0f;
            m_startingTime = _startingTime;
            m_duration = _duration;
            m_vertexIndex = _vertexIndex;
            m_materialIndex = _materialIndex;
        }

        public void UpdateTime(float _time) {
            if (_time < m_startingTime) {
                m_progress = 0;
                return;
            }

            float currentProgress = (_time - m_startingTime) / m_duration;
            currentProgress = Mathf.Clamp01(currentProgress);

            m_progress = currentProgress;
        }

    }
}