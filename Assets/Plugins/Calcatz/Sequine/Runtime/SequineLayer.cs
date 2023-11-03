using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Calcatz.Sequine {

    /// <summary>
    /// Manages animation clips mixing in a certain layer.
    /// </summary>
    public class SequineLayer {

        private string m_name;
        private AnimationLayerMixerPlayable m_layerMixerPlayable;
        private AnimationMixerPlayable m_mixerPlayable;
        private float m_weight;

        private Dictionary<AnimationClip, SequineState> m_states = new Dictionary<AnimationClip, SequineState>();
        private List<SequineState> m_temporaryStates = new List<SequineState>();
        private SequineState m_currentState;
        private HashSet<SequineState> m_exitingStates = new HashSet<SequineState>();
        private PlayableGraph m_playableGraph;
        private List<int> pooledInputPorts = new List<int>();
        private int m_layerIndex;

        public string name => m_name;
        public SequineState currentState => m_currentState;
        public IEnumerable<SequineState> states => m_states.Values;
        internal IEnumerable<SequineState> temporaryStates => m_temporaryStates;

        public float weight { 
            get => m_weight;
            set {
                m_weight = value;
                m_layerMixerPlayable.SetInputWeight(m_layerIndex, value);
            }
        }


        public SequineLayer(PlayableGraph _playableGraph, AnimationLayerMixerPlayable _layerMixerPlayable, int _layerIndex, SequinePlayer.LayerRegistry _layerRegistry) {
            m_name = _layerRegistry.name;
            m_layerIndex = _layerIndex;
            m_playableGraph = _playableGraph;
            m_layerMixerPlayable = _layerMixerPlayable;
            if (_layerIndex >= m_layerMixerPlayable.GetInputCount()) {
                m_layerMixerPlayable.SetInputCount(_layerIndex + 1);
            }
            {
                m_mixerPlayable = AnimationMixerPlayable.Create(_playableGraph);
                m_layerMixerPlayable.ConnectInput(_layerIndex, m_mixerPlayable, 0);
                m_layerMixerPlayable.SetInputWeight(_layerIndex, _layerRegistry.weight);
                m_layerMixerPlayable.SetLayerAdditive((uint)_layerIndex, _layerRegistry.additive);
                if (_layerRegistry.mask != null) {
                    m_layerMixerPlayable.SetLayerMaskFromAvatarMask((uint)_layerIndex, _layerRegistry.mask);
                }
            }

            weight = _layerRegistry.weight;
        }

        private delegate SequineState CreateStateDelegate(AnimationClip _clip, int inputPort);

        private SequineState CreateSequineState(AnimationClip _clip, int _inputPort) {
            var state = new SequineState(m_mixerPlayable, _inputPort);
            m_states.Add(_clip, state);
            return state;
        }

        public SequineState PlayAnimationClip(AnimationClip _clip, AnimationConfig _config, bool _restart = false, Action _onComplete = null) {
            return HandlePlayAnimationClip(_clip, _config, _restart, _onComplete, CreateSequineState);
        }

        public SequineState PlayRangedAnimationClip(AnimationClip _clip, AnimationConfig _config, float _minTimePercentage, float _maxTimePercentage, bool _restart = false, Action _onComplete = null) {
            if (_minTimePercentage > _maxTimePercentage) _minTimePercentage = _maxTimePercentage;
            var resultState = (SequineRangedState)HandlePlayAnimationClip(_clip, _config, false, _onComplete, (_clip2, _inputPort) => {
                var state = new SequineRangedState(m_mixerPlayable, _inputPort, _minTimePercentage, _maxTimePercentage);
                m_temporaryStates.Add(state);
                state.onStateFinished += () => {
                    pooledInputPorts.Add(state.inputPort);
                    m_exitingStates.Remove(state);
                    m_playableGraph.DestroyPlayable(state.clipPlayable);
                    m_temporaryStates.Remove(state);
                };
                return state;
            });
            resultState.ClampTime();
            return resultState;
        }

        private SequineState HandlePlayAnimationClip(AnimationClip _clip, AnimationConfig _config, bool _restart, Action _onComplete, CreateStateDelegate _onCreateState) {
            SequineState state;
            if (_restart) {
                RemoveState(_clip);
            }

            if (!m_playableGraph.IsValid()) {
                return null;
            }

            if (!m_states.TryGetValue(_clip, out state)) {
                var clipPlayable = AnimationClipPlayable.Create(m_playableGraph, _clip);

                int inputPort;// = m_states.Count;
                if (pooledInputPorts.Count > 0) {
                    inputPort = pooledInputPorts[0];
                    pooledInputPorts.RemoveAt(0);
                }
                else {
                    inputPort = m_states.Count + m_temporaryStates.Count;
                }

                if (inputPort >= m_mixerPlayable.GetInputCount()) {
                    m_mixerPlayable.SetInputCount(inputPort + 1);
                }
                m_playableGraph.Disconnect(m_mixerPlayable, inputPort);
                m_playableGraph.Connect(clipPlayable, 0, m_mixerPlayable, inputPort);
                m_mixerPlayable.SetInputWeight(inputPort, 0.0f);

                state = _onCreateState.Invoke(_clip, inputPort);
            }

            if (m_currentState != null) {
                ChangeState(state, _config.transitionDuration, _config.normalizedTransition);
            }
            else {
                state.Play();
                m_currentState = state;
                //Debug.Log("C " + m_currentState.clip.name + " --> " + state.clip.name);
                m_currentState.UpdateWeight(m_weight);
            }

            //state.time = 0f;
            state.lengthToPlay = _config.lengthToPlay;
            state.speed = _config.speed;
            state.onComplete = () => {
                _onComplete?.Invoke();
                state.onComplete = null;
            };

            return state;
        }


        /// <summary>
        /// Try get the clip of the current state if not null.
        /// </summary>
        /// <param name="_clip"></param>
        /// <returns></returns>
        public bool TryGetCurrentClip(out AnimationClip _clip) {
            if (m_currentState != null && m_currentState.clip != null) {
                _clip = currentState.clip;
                return true;
            }
            _clip = null;
            return false;
        }

        public bool TryGetState(AnimationClip _clip, out SequineState _state) {
            if (m_states.TryGetValue(_clip, out SequineState state)) {
                _state = state;
                return true;
            }
            _state = null;
            return false;
        }

        public void RemoveState(AnimationClip _clip) {
            if (_clip == null) return;
            if (TryGetState(_clip, out var state)) {
                if (m_currentState == state) {
                    m_currentState = null;
                }
                pooledInputPorts.Add(state.inputPort);
                m_exitingStates.Remove(state);
                m_playableGraph.DestroyPlayable(state.clipPlayable);
                m_states.Remove(_clip);
            }
        }

        private void ChangeState(SequineState _state, float _transitionDuration, bool _normalizedTransition) {
            if (m_currentState == _state) return;
            if (_transitionDuration > 0f) {
                //Debug.Log("A " + m_currentState.clip.name + " --> " + _state.clip.name);
                if (_normalizedTransition)
                    m_currentState.fadeOutDurationNormalized = _transitionDuration;
                else
                    m_currentState.fadeOutDuration = _transitionDuration;

                m_currentState.Exit();
                m_exitingStates.Add(m_currentState);
                m_currentState = _state;
                m_currentState.time = 0;

                if (_normalizedTransition)
                    m_currentState.fadeInDurationNormalized = _transitionDuration;
                else
                    m_currentState.fadeInDuration = _transitionDuration;
            }
            else {
                //Debug.Log("B " + m_currentState.clip.name + " --> " + state.clip.name);
                m_currentState.Finish();
                m_currentState = _state;
            }
            m_currentState.Play();
        }

        private void NormalizeWeights() {
            float totalWeights = 0;

            foreach (var state in m_states.Values) {
                totalWeights += state.weight;
            }
            foreach (var state in m_temporaryStates) {
                totalWeights += state.weight;
            }

            foreach (var state in m_states.Values) {
                state.UpdateWeight(state.weight / totalWeights);
            }
            foreach (var state in m_temporaryStates) {
                state.UpdateWeight(state.weight / totalWeights);
            }

        }

        public void Tick(float _deltaTime) {
            //Debug.Log("Tick");

            if (m_currentState != null) {
                m_currentState.Tick(_deltaTime);
            }

            foreach (var exitingState in m_exitingStates.ToArray()) {
                exitingState.Tick(_deltaTime);
            }
            m_exitingStates.RemoveWhere(_state => _state.exited);

            NormalizeWeights();
        }

        public void ExitCurrentState() {
            if (m_currentState != null) m_currentState.Exit();
        }


        public void Stop() {
            foreach (var state in m_exitingStates) {
                state.Finish();
            }
            if (m_currentState != null) m_currentState.Finish();
        }

    }

}