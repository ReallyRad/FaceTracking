using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Sequine Text is a TextMeshPro animation tool with stackable behaviour.
    /// Using the stackable behaviour, you can deicide whether you want to animate it per character or the whole texts, the timings, the colors, etc.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(TMP_Text))]
    [AddComponentMenu("Sequine/Sequine Text")]
    public class SequineText : MonoBehaviour {

        public event System.Action onAnimationCompleted;

        #region PROPERTIES
        [SerializeField, HideInInspector] private TMP_Text m_textComponent;
        [SerializeField] private UpdateMode m_updateMode = UpdateMode.NormalUpdate;
        /*[SerializeField]*/ private bool m_playImmediately = true;
        [Tooltip("When completed, the animation will not be stopped, hence keeping the last state of the animation.")]
        [SerializeField] private bool m_holdOnComplete = true;
        [SerializeField] private bool m_loop;

        private float m_time;

        private bool m_isPlaying;
        private List<TextAnimationSegment> m_segments = new List<TextAnimationSegment>();

        private string cachedText = string.Empty;
        private bool isDirty = true;
        private bool playTriggered;

        public TMP_Text textComponent => m_textComponent;

        public bool isPlaying => m_isPlaying;

        public string text {
            get => textComponent.text;
            set {
                textComponent.text = value;
                SetDirty();
                UpdateIfDirty();
            }
        }

        public float normalizedTime {
            get => m_time / totalDuration;
            set {
                time = value * totalDuration;
            }
        }

        public float time {
            get {
                return m_time;
            }
            set {
                m_time = value;
                for (int i = 0; i < m_segments.Count; i++) {
                    m_segments[i].time = value;
                }
                m_textComponent.havePropertiesChanged = true;
            }
        }

        public float totalDuration {
            get {
                float result = 0;
                for (int i = 0; i < m_segments.Count; i++) {
                    result += m_segments[i].totalDuration;
                }
                return result;
            }
        }

        public bool loop { get => m_loop; set => m_loop = value; }
        #endregion

        #region INITIALIZATION
        private void OnValidate() {
            cachedText = string.Empty;
            SetDirty();

            if (m_textComponent == null)
                m_textComponent = GetComponentInChildren<TMP_Text>();
        }

        private void Awake() {
            if (Application.isPlaying && m_updateMode != UpdateMode.Manual)
                time = 0;
        }

        private void Start() {
            playTriggered = false;
        }
        #endregion

        #region TICK
        private void Tick() {
            if (!IsValid())
                return;

            UpdateIfDirty();

            if (!playTriggered) {
                StartPlaying();
                playTriggered = true;
            }

            StepTime();
            if (m_isPlaying || m_updateMode == UpdateMode.Manual) {
                m_textComponent.ForceMeshUpdate(true);
                UpdateSegments();
            }
        }

        private void UpdateSegments() {
            for (int i = 0; i < m_segments.Count; i++) {
                m_segments[i].Update();
            }
        }

        protected virtual void Update() {
            if (m_updateMode == UpdateMode.FixedUpdate) return;
            Tick();
        }


        protected virtual void FixedUpdate() {
            if (m_updateMode != UpdateMode.FixedUpdate) return;
            Tick();
        }
        #endregion

        #region PUBLIC-ORDER
        public void Restart() {
            time = 0;
        }

        //[Sirenix.OdinInspector.Button]
        public void Play() {
            Play(true);
        }

        public void Play(bool _restart = true) {
            if (!IsValid()) {
                m_playImmediately = true;
                return;
            }

            playTriggered = true;
            if (_restart)
                Restart();

            m_isPlaying = true;
        }

        public void Complete() {
            if (isPlaying) {
                time = totalDuration;
                if (!m_loop) {
                    if (!m_holdOnComplete) {
                        Stop();
                    }
                }
                OnAnimationCompleted();
            }
        }

        public void Stop() {
            m_isPlaying = false;
        }

        public void SetDirty() {
            isDirty = true;
        }
        #endregion

        #region PRIVATE-ORDER
        private void StartPlaying() {
            if (!Application.isPlaying)
                return;

            if (m_playImmediately)
                Play();
        }

        private bool IsValid() {
            if (textComponent == null)
                return false;

            if (textComponent.textInfo == null)
                return false;

            if (textComponent.mesh == null)
                return false;

            if (textComponent.textInfo.meshInfo == null)
                return false;

            if (m_segments == null || m_segments.Count == 0)
                return false;

            return true;
        }

        private void StepTime() {
            if (isPlaying) {
                time += UpdateModeUtility.GetDeltaTime[m_updateMode]();
                float t = time;
                float dur = totalDuration;
                if (t < dur)
                    return;

                if (m_loop) {
                    time -= dur;
                }
                else {
                    time = dur;
                    if (!m_holdOnComplete) {
                        Stop();
                    }
                    OnAnimationCompleted();
                }
            }
        }

        private void OnAnimationCompleted() {
            onAnimationCompleted?.Invoke();
        }

        private void UpdateIfDirty() {
            if (!isDirty)
                return;

            if (!gameObject.activeInHierarchy || !gameObject.activeSelf)
                return;

            for (int i = 0; i < m_segments.Count; i++) {
                m_segments[i].UpdateIfDirty();
            }

            if (string.IsNullOrEmpty(cachedText) || !cachedText.Equals(textComponent.text)) {
                textComponent.ForceMeshUpdate();
                var textInfo = textComponent.textInfo;
                var cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

                float accumulativeTime = 0;
                for (int i = 0; i < m_segments.Count; i++) {
                    m_segments[i].SetReferences(textComponent, textInfo, cachedMeshInfo);

                    m_segments[i].RefreshCharactersData(accumulativeTime);
                    accumulativeTime += m_segments[i].totalDuration;
                }

                cachedText = textComponent.text;
            }

            isDirty = false;
        }

        /// <summary>
        /// Append text to the current text, and immediately play if it's currently not playing.
        /// </summary>
        /// <param name="_text"></param>
        /// <param name="_behaviourProfile"></param>
        /// <param name="_onComplete"></param>
        public void AppendText(string _text, TextBehaviourProfile _behaviourProfile, System.Action _onComplete = null) {
            AppendText(_text, _behaviourProfile, false, _onComplete);
        }

        /// <summary>
        /// Append text to the current text, and immediately play if it's currently not playing.
        /// </summary>
        /// <param name="_text"></param>
        /// <param name="_behaviourProfile"></param>
        /// <param name="_restartAnimation"></param>
        /// <param name="_onComplete"></param>
        public void AppendText(string _text, TextBehaviourProfile _behaviourProfile, bool _restartAnimation, System.Action _onComplete = null) {
            StartCoroutine(AppendTextCoroutine(_text, _behaviourProfile, _restartAnimation, _onComplete));
        }

        private IEnumerator AppendTextCoroutine(string _text, TextBehaviourProfile _behaviourProfile, bool _restartAnimation, System.Action _onComplete = null) {
            yield return new WaitForEndOfFrame();
            int startIndex = textComponent.text.Length;
            textComponent.text += _text;
            int endIndex = textComponent.text.Length - 1;
            m_segments.Add(new TextAnimationSegment(_behaviourProfile, startIndex, endIndex, totalDuration));
            SetDirty();
            if (_onComplete != null) {
                //Create new lambda so that we can remove the callback right after it's invoked.
                System.Action onComplete = null;
                onComplete = () => {
                    _onComplete.Invoke();
                    onAnimationCompleted -= onComplete;
                };
                onAnimationCompleted += onComplete;
            }
            if (!m_isPlaying || _restartAnimation) Play(_restartAnimation);
        }

        public void ResetText() {
            m_segments.Clear();
            text = "";
            time = 0;
            m_textComponent.text = text;
        }
        #endregion
    }
}