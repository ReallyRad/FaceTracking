using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Calcatz.Sequine {

    /// <summary>
    /// A segment of the text within the range of the appended text.
    /// </summary>
    [System.Serializable]
    public class TextAnimationSegment {


        [SerializeField] private float m_durationPerCharacter = 0.1f;
        [SerializeField] private float m_delayPerCharacter = 0.05f;
        [Range(0.0f, 1.0f)]
        [SerializeField] private float m_normalizedTime;

        private TMP_Text m_textComponent;
        private float m_startingTime;

        private TextBehaviourComponent[] vertexBehaviours;

        private CharacterData[] charactersData;
        private TMP_MeshInfo[] cachedMeshInfo;
        private TMP_TextInfo textInfo;
        private int startIndex;
        private int endIndex;

        private bool updateGeometry;
        private bool updateVertexData;

        private float m_time;
        private float m_totalDuration;

        public TextAnimationSegment(TextBehaviourProfile _behaviourProfile, int _startIndex, int _endIndex, float _startingTime) {
            m_durationPerCharacter = _behaviourProfile.durationPerCharacter;
            m_delayPerCharacter = _behaviourProfile.delayPerCharacter;
            vertexBehaviours = new TextBehaviourComponent[_behaviourProfile.components.Count];
            for (int i = 0; i < vertexBehaviours.Length; i++) {
                //vertexBehaviours[i] = ScriptableObject.Instantiate(_behaviourProfile.components[i]);
                vertexBehaviours[i] = _behaviourProfile.components[i];
            }
            startIndex = _startIndex;
            endIndex = _endIndex;
            m_startingTime = _startingTime;
        }

        public float normalizedTime { 
            get => m_normalizedTime; 
            set {
                m_normalizedTime = value;
                m_normalizedTime = Mathf.Clamp01(m_normalizedTime);
                time = m_startingTime + (m_normalizedTime * m_totalDuration);
            }
        }

        public float time { 
            get => m_time; 
            set {
                m_time = value;
                UpdateTime();
                HandleBehaviours();
            } 
        }

        public float totalDuration { get => m_totalDuration; set => m_totalDuration = value; }

        public void SetReferences(TMP_Text _textComponent, TMP_TextInfo _textInfo, TMP_MeshInfo[] _cachedMeshInfo) {
            m_textComponent = _textComponent;
            textInfo = _textInfo;
            cachedMeshInfo = _cachedMeshInfo;
        }

        public void Update() {
            UpdateTime();
            HandleBehaviours();
        }

        public void UpdateIfDirty() {
            for (int i = 0; i < vertexBehaviours.Length; i++) {
                TextBehaviourComponent vertexBehaviour = vertexBehaviours[i];

                if (!updateGeometry && vertexBehaviour.overrideGeometry)
                    updateGeometry = true;

                if (!updateVertexData && vertexBehaviour.overrideVertexData)
                    updateVertexData = true;
            }
        }

        public void RefreshCharactersData(float _currentAccumulativeTime) {
            List<CharacterData> newCharacterDataList = new List<CharacterData>();
            int indexCountFrom0 = 0;
            int indexCount = startIndex;
            for (int i = startIndex; i <= endIndex; i++) {
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                CharacterData characterData = new CharacterData(indexCount,
                                                                 _currentAccumulativeTime + m_delayPerCharacter * indexCountFrom0,
                                                                 m_durationPerCharacter,
                                                                 textInfo.characterInfo[i]
                                                                         .materialReferenceIndex,
                                                                 textInfo.characterInfo[i].vertexIndex);
                newCharacterDataList.Add(characterData);
                indexCount += 1;
                indexCountFrom0 += 1;
            }

            charactersData = newCharacterDataList.ToArray();
            m_totalDuration = m_durationPerCharacter + (charactersData.Length * m_delayPerCharacter);
        }

        private void UpdateTime() {
            m_normalizedTime = (m_time - m_startingTime) / m_totalDuration;
            m_normalizedTime = Mathf.Clamp01(m_normalizedTime);
            if (charactersData == null)
                return;

            float t = time;
            for (int i = 0; i < charactersData.Length; i++)
                charactersData[i].UpdateTime(t);
        }

        private void HandleBehaviours() {
            if (charactersData == null) return;

            var meshBounds = GetMeshBoundsForCharacterRange();

            for (int i = 0; i < charactersData.Length; i++)
                HandleCharacterBehaviour(i, meshBounds);

            if (updateGeometry) {
                for (int i = 0; i < textInfo.meshInfo.Length; i++) {
                    textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                    m_textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                }
            }

            if (updateVertexData)
                m_textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }

        private void HandleCharacterBehaviour(int _charIndex, Bounds _meshBounds) {
            for (int i = 0; i < vertexBehaviours.Length; i++) {
                if (!vertexBehaviours[i].active) continue;
                vertexBehaviours[i].HandleCharacterBehaviour(charactersData[_charIndex], m_textComponent, textInfo, cachedMeshInfo, _meshBounds, m_normalizedTime);
            }
        }

        private Bounds GetMeshBoundsForCharacterRange() {
            if (startIndex < 0 || endIndex >= textInfo.characterCount) {
                Debug.LogError("Character range is out of bounds!");
                return default(Bounds);
            }

            Bounds bounds = new Bounds();

            for (int i = startIndex; i <= endIndex; i++) {
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                if (materialIndex >= 0 && materialIndex < cachedMeshInfo.Length) {
                    Vector3[] vertices = cachedMeshInfo[materialIndex].vertices;

                    bounds.Encapsulate(vertices[vertexIndex]);
                    bounds.Encapsulate(vertices[vertexIndex + 1]);
                    bounds.Encapsulate(vertices[vertexIndex + 2]);
                    bounds.Encapsulate(vertices[vertexIndex + 3]);
                }
            }

            return bounds;
        }

    }

}