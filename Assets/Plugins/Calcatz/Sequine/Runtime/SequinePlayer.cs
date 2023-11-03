using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using System.Collections;
using System.Linq;

namespace Calcatz.Sequine {

    /// <summary>
    /// Sequine Player is a component that can play Animation Clip on demand without relying on Animator Controller.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [AddComponentMenu("Sequine/Sequine Player")]
    public class SequinePlayer : MonoBehaviour {

        #region STATICS
        private static readonly Dictionary<SequineAnimationData, SequinePlayer> instances = new Dictionary<SequineAnimationData, SequinePlayer>();
        public static SequinePlayer GetInstance(SequineAnimationData _binder) {
            if (_binder == null) return null;
            if (instances.TryGetValue(_binder, out SequinePlayer result)) {
                return result;
            }
            return null;
        }

        private static AnimationConfig m_defaultAnimationConfig = new AnimationConfig(1f, 0.25f, false);
        public static AnimationConfig defaultAnimationConfig => m_defaultAnimationConfig;
        #endregion STATICS


        #region PROPERTIES
        [SerializeField, HideInInspector] private Animator m_animator;
        [SerializeField] private SequineAnimationData m_animationData;
        [SerializeField] private UpdateMode m_updateMode = UpdateMode.NormalUpdate;
        [SerializeField, HideInInspector] private float m_weight = 1f;
        [SerializeField, HideInInspector] private LayerRegistry[] m_animationLayers = new LayerRegistry[1] { new LayerRegistry() { weight = 1 } };

        private PlayableGraph m_playableGraph;
        private AnimationMixerPlayable m_rootAnimationMixer;
        private AnimatorControllerPlayable m_animatorControllerPlayable;
        private RuntimeAnimatorController m_animatorController;
        private AnimationMixerPlayable m_sequineAnimationMixer;
        private AnimationLayerMixerPlayable m_layerMixerPlayable;
        private List<SequineLayer> m_layers = new List<SequineLayer>();
        private bool m_paused = false;

        private Dictionary<int, AnimationClip> m_actionClipDict;

        private Coroutine updateCoroutine;

        public SequineAnimationData animationData => m_animationData;
        public SequineState currentState => m_layers[0].currentState;
        public IEnumerable<SequineState> states => m_layers[0].states;
        public List<SequineLayer> layers => m_layers;

        /// <summary>
        /// Try get the clip of the current state in base layer if not null.
        /// </summary>
        /// <param name="_clip"></param>
        /// <returns></returns>
        public bool TryGetCurrentClip(out AnimationClip _clip) {
            if (m_layers.Count > 0) {
                if (m_layers[0].TryGetCurrentClip(out _clip)) {
                    return true;
                }
                return false;
            }
            _clip = null;
            return false;
        }

        public UpdateMode updateMode { 
            get => m_updateMode;
            set {
                if (value == UpdateMode.Manual && m_updateMode != UpdateMode.Manual) {
                    StopUpdate();
                }
                else if (value != UpdateMode.Manual && m_updateMode == UpdateMode.Manual) {
                    StartUpdate();
                }
                m_updateMode = value;
            }
        }

        public float weight { 
            get => m_weight;
            set {
                m_weight = value;
                m_rootAnimationMixer.SetInputWeight(0, 1-value);
                m_rootAnimationMixer.SetInputWeight(1, value);
            }
        }

        public RuntimeAnimatorController animatorController { get => m_animatorController; set => m_animatorController = value; }

        #endregion PROPERTIES


        protected virtual void OnValidate() {
            if (m_animator == null) m_animator = GetComponent<Animator>();
        }

        protected virtual void Awake() {
            m_actionClipDict = new Dictionary<int, AnimationClip>();
            if (m_animationData != null) {
                if (!instances.ContainsKey(m_animationData)) {
                    instances.Add(m_animationData, this);
                }
                foreach (var a in m_animationData.actionClips) {
                    m_actionClipDict.Add(a.id, a.clip);
                }
            }

            m_playableGraph = PlayableGraph.Create(name);
            m_playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

            // Create playable output node.
            var playableOutput = AnimationPlayableOutput.Create(m_playableGraph, "Animation", m_animator);

            m_rootAnimationMixer = AnimationMixerPlayable.Create(m_playableGraph, 2);
            {
                m_animatorController = m_animator.runtimeAnimatorController;
                m_animator.runtimeAnimatorController = null;
                m_animatorControllerPlayable = AnimatorControllerPlayable.Create(m_playableGraph, animatorController);
                m_playableGraph.Connect(m_animatorControllerPlayable, 0, m_rootAnimationMixer, 0);
                m_rootAnimationMixer.SetInputWeight(0, 1-m_weight);

                m_sequineAnimationMixer = AnimationMixerPlayable.Create(m_playableGraph, 1);
                m_rootAnimationMixer.ConnectInput(1, m_sequineAnimationMixer, 0);
                m_rootAnimationMixer.SetInputWeight(1, m_weight);

                m_layerMixerPlayable = AnimationLayerMixerPlayable.Create(m_playableGraph, 1);
                m_sequineAnimationMixer.ConnectInput(0, m_layerMixerPlayable, 0);
                m_sequineAnimationMixer.SetInputWeight(0, 1.0f);
                {
                    m_layers.Clear();
                    for (int i=0; i<m_animationLayers.Length; i++) {
                        var registry = m_animationLayers[i];
                        var layer = new SequineLayer(m_playableGraph, m_layerMixerPlayable, i, registry);
                        m_layers.Add(layer);
                    }
                }
            }

            // Connect the mixer to an output.
            playableOutput.SetSourcePlayable(m_rootAnimationMixer);

            // Plays the playable graph.
            m_playableGraph.Play();

            StartUpdate();
        }

        private void StartUpdate() {
            if (m_updateMode != UpdateMode.Manual) {
                if (updateCoroutine == null) {
                    updateCoroutine = StartCoroutine(UpdateCoroutine());
                }
            }
        }

        private void StopUpdate() {
            if (updateCoroutine != null) StopCoroutine(updateCoroutine);
            updateCoroutine = null;
        }

        protected virtual void OnEnable() {
            StartUpdate();
        }


        protected virtual void OnDisable() {
            StopUpdate();
        }

        protected virtual void OnDestroy() {
            if (m_animationData != null && instances.ContainsKey(m_animationData)) {
                instances.Remove(m_animationData);
            }
            // Destroys all Playables and Outputs created by the graph.
            m_playableGraph.Destroy();
        }

        /// <summary>
        /// Remove a state in base layer.
        /// </summary>
        /// <param name="_clip"></param>
        public void RemoveState(AnimationClip _clip) {
            if (m_layers.Count > 0) {
                m_layers[0].RemoveState(_clip);
            }
        }

        public SequineState PlayAnimationClip(AnimationClip _clip, AnimationConfig _config, bool _restart = false, Action _onComplete = null) {
            return PlayAnimationClip(0, _clip, _config, _restart, _onComplete);
        }
        public SequineState PlayAnimationClip(AnimationClip _clip, AnimationConfig _config, Action _onComplete) {
            return PlayAnimationClip(0, _clip, _config, false, _onComplete);
        }

        public SequineState PlayAnimationClip(int _layer, AnimationClip _clip, AnimationConfig _config, bool _restart = false, Action _onComplete = null) {
            if (_layer < m_layers.Count) {
                m_paused = false;
                return m_layers[_layer].PlayAnimationClip(_clip, _config, _restart, _onComplete);
            }
            Debug.LogError("The specified layer index exceeds the number of the layers.");
            return null;
        }

        /// <summary>
        /// Plays an animation clip, and only play within the percentaged range.
        /// 
        /// Note that you can't get this state using TryGetState method as they're not bound to clip.
        /// One clip might have multiple SequineRangedStates to enable blending between the same clip at different time.
        /// </summary>
        /// <param name="_layer"></param>
        /// <param name="_clip"></param>
        /// <param name="_config"></param>
        /// <param name="minTimePercentage"></param>
        /// <param name="_maxTimePercentage"></param>
        /// <param name="_restart"></param>
        /// <param name="_onComplete"></param>
        /// <returns>Returns a SequineRangedState.</returns>
        public SequineState PlayRangedAnimationClip(int _layer, AnimationClip _clip, AnimationConfig _config, float minTimePercentage, float _maxTimePercentage, bool _restart = false, Action _onComplete = null) {
            if (_layer < m_layers.Count) {
                m_paused = false;
                return m_layers[_layer].PlayRangedAnimationClip(_clip, _config, minTimePercentage, _maxTimePercentage, _restart, _onComplete);
            }
            Debug.LogError("The specified layer index exceeds the number of the layers.");
            return null;
        }

        /// <summary>
        /// Stop all layers.
        /// </summary>
        public void Stop() {
            for (int i=0; i<m_layers.Count; i++) {
                m_layers[i].Stop();
            }
        }

        public void Pause() {
            m_paused = true;
        }

        public void Resume() {
            m_paused = false;
        }

        public void ExitCurrentState(int _layer) {
            if (_layer < m_layers.Count) {
                m_layers[_layer].ExitCurrentState();
            }
        }

        public SequineState PlayAction(int _id, AnimationConfig _config, bool _restart = false, Action _onComplete = null) {
            return PlayAction(0, _id, _config, _restart, _onComplete);
        }

        public SequineState PlayAction(int _id, AnimationConfig _config, Action _onComplete) {
            return PlayAction(0, _id, _config, false, _onComplete);
        }

        public SequineState PlayAction(int _layer, int _id, AnimationConfig _config, bool _restart = false, Action _onComplete = null) {
            if (m_actionClipDict.TryGetValue(_id, out AnimationClip clip)) {
                return PlayAnimationClip(_layer, clip, _config, _restart, _onComplete);
            }
            Debug.LogError("Can't find an action animation in " + animationData + " with id " + _id, this);
            return null;
        }

        public SequineState PlayActionWithTranslation(int _id, Vector3 _translation, AnimationConfig _config, bool _restart = false, Action _onComplete = null, EasingFunction.Ease _easing = EasingFunction.Ease.Linear) {
            return PlayActionWithTranslation(0, _id, _translation, _config, _restart, _onComplete, _easing);
        }

        public SequineState PlayActionWithTranslation(int _layer, int _id, Vector3 _translation, AnimationConfig _config, bool _restart = false, Action _onComplete = null, EasingFunction.Ease _easing = EasingFunction.Ease.Linear) {
            if (m_actionClipDict.TryGetValue(_id, out AnimationClip clip)) {
                var state = PlayAnimationClip(_layer, clip, _config, _restart, _onComplete);

                Vector3 initialPos = transform.position;
                Vector3 targetPos = initialPos + _translation;
                OnStateUpdateCallback onUpdate = null;
                var easingFunction = EasingFunction.GetEasingFunction(_easing);
                onUpdate = _normalizedLength => {
                    _normalizedLength = Mathf.Clamp01(_normalizedLength);
                    float t = state.completed ? 1 : _normalizedLength;
                    transform.position = Vector3.Lerp(initialPos, targetPos, easingFunction.Invoke(0f, 1f, t));
                    if (state.completed) {
                        state.onUpdate -= onUpdate;
                    }
                };

                state.onUpdate += onUpdate;
                return state;
            }
            Debug.LogError("Can't find an action animation in " + animationData + " with id " + _id, this);
            return null;
        }

        /// <summary>
        /// Try get state in base layer.
        /// </summary>
        /// <param name="_clip"></param>
        /// <param name="_state"></param>
        /// <returns></returns>
        public bool TryGetState(AnimationClip _clip, out SequineState _state) {
            if (m_layers.Count > 0) {
                if (m_layers[0].TryGetState(_clip, out _state)) {
                    return true;
                }
                return false;
            }
            _state = null;
            return false;
        }

        public bool TryGetActionClip(int _actionId, out AnimationClip _clip) {
            if (m_actionClipDict == null) {
                _clip = null;
                return false;
            }
            if (m_actionClipDict.TryGetValue(_actionId, out _clip)) {
                return true;
            }
            else {
                _clip = null;
                return false;
            }
        }

        private IEnumerator UpdateCoroutine() {
            while (true) {
                Tick(UpdateModeUtility.GetDeltaTime[m_updateMode]());
                yield return UpdateModeUtility.GetYieldReturn[m_updateMode]();
            }
        }

        public void Tick(float _deltaTime) {
            if (m_paused) return;

            for (int i=0; i<m_layers.Count; i++) {
                m_layers[i].Tick(_deltaTime);
            }

            m_playableGraph.Evaluate(_deltaTime);
        }

        /// <summary>
        /// Pauses an animation (if the clip is currently playing) by specifying the animation clip asset, and sample a certain pose at the specified time.
        /// This will also automatically plays the animation clip if it's not yet played.
        /// Since AnimationConfig parameter is not specified, the default animation config will be used as the configuration to play when it's not yet played.
        /// </summary>
        /// <param name="_clip"></param>
        /// <param name="_time"></param>
        /// <param name="_useNormalizedTime"></param>
        public void PauseAnimationClipAtTime(AnimationClip _clip, float _time, bool _useNormalizedTime) {
            PauseAnimationClipAtTime(_clip, _time, _useNormalizedTime, defaultAnimationConfig);
        }

        /// <summary>
        /// Pauses an animation (if the clip is currently playing) by specifying the animation clip asset, and sample a certain pose at the specified time.
        /// This will also automatically plays the animation clip if it's not yet played.
        /// </summary>
        /// <param name="_clip"></param>
        /// <param name="_time"></param>
        /// <param name="_useNormalizedTime"></param>
        /// <param name="_config"></param>
        public void PauseAnimationClipAtTime(AnimationClip _clip, float _time, bool _useNormalizedTime, AnimationConfig _config) {
            if (TryGetState(_clip, out var state)) {
                if (currentState != state) {
                    state = PlayAnimationClip(_clip, _config);
                }
            }
            else {
                state = PlayAnimationClip(_clip, _config);
            }
            if (_useNormalizedTime) {
                state.normalizedTime = _time;
            }
            else {
                state.time = _time;
            }
            state.Pause();
        }


        [System.Serializable]
        public class LayerRegistry {
            public string name = "New Layer";
            [Range(0, 1)]
            public float weight = 1;
            public AvatarMask mask;
            public bool additive = false;
#if UNITY_EDITOR
            public bool editor_isDebuggerExpanded = false;
#endif
        }

    }
}