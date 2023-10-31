using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TIMELINE_AVAILABLE
using UnityEngine.Playables;
using UnityEngine.Timeline;
#endif
#if ODIN_INSPECTOR
using Sirenix.Serialization;
#else
using Calcatz.OdinSerializer;
#endif
#if MEC_AVAILABLE
using MEC;
#endif

[assembly: BindTypeNameToType("WorldTimelinePlayerCommand", typeof(Calcatz.CookieCutter.TimelinePlayerCommand))]

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Play, Resume, Stop, or Pause a PlayableDirector.
    /// </summary>
    public class TimelinePlayerCommand : Command {
#if TIMELINE_AVAILABLE
        public Control control;
        public TimelineAsset asset;
        public bool asynchronous;
        public bool deactivateGameObject = true;

#if MEC_AVAILABLE
        private CoroutineHandle waitCoroutine;
#else
        private Coroutine waitCoroutine;
#endif

        public enum Control {
            Play,
            Resume,
            Stop,
            Pause
        }

        public override void Execute(CommandExecutionFlow _flow) {
            if (asset != null) {
                var playableDirector = CrossSceneBindingUtility.GetObject<PlayableDirector>(asset);
                if (playableDirector == null) {
                    Debug.LogError("Playable Director for " + asset.name + "not found: " + _flow.currentCommandData.targetObject.name + ", Command ID: " + id);
                    Exit();
                    return;
                }
                switch (control) {
                    case Control.Play:
                        PlayTimeline(_flow, playableDirector);
                        break;
                    case Control.Stop:
                        StopTimeline(_flow, playableDirector);
                        break;
                    case Control.Resume:
                        ResumeTimeline(_flow, playableDirector);
                        break;
                    case Control.Pause:
                        PauseTimeline(_flow, playableDirector);
                        break;
                }
                if (asynchronous) {
                    OnFinishedExecution();
                }
            }
            else {
                Debug.LogError("Timeline asset is null: " + _flow.currentCommandData.targetObject.name + ", Command ID: " + id);
                Exit();
            }
        }

        protected virtual void OnFinishedExecution() {
            Exit();
        }

        protected virtual void StopTimeline(CommandExecutionFlow _flow, PlayableDirector playableDirector) {
            playableDirector.Stop();
            if (deactivateGameObject) playableDirector.gameObject.SetActive(false);
            if (!asynchronous) {
#if MEC_AVAILABLE
                if (waitCoroutine != null) {
                    Timing.KillCoroutines(waitCoroutine);
                }
#else
                if (waitCoroutine != null) {
                    MonoBehaviour obj;
                    if (_flow.executor.targetObject is MonoBehaviour mb){
                        obj = mb;
                    }
                    else {
                        obj = playableDirector.GetComponent<MonoBehaviour>();
                    }
                    if (obj != null) {
                        obj.StopCoroutine(waitCoroutine);
                    }
                }
#endif
                OnFinishedExecution();
            }
        }

        protected virtual void PlayTimeline(CommandExecutionFlow _flow, PlayableDirector playableDirector) {
            playableDirector.gameObject.SetActive(true);
            playableDirector.Play();
            if (!asynchronous) {
#if MEC_AVAILABLE
                if (waitCoroutine != null) {
                    
                    Timing.KillCoroutines(waitCoroutine);
                }
                waitCoroutine = Timing.RunCoroutine(WaitForTimeline(_flow, playableDirector));
#else
                MonoBehaviour obj;
                if (_flow.executor.targetObject is MonoBehaviour mb){
                    obj = mb;
                }
                else {
                    obj = playableDirector.GetComponent<MonoBehaviour>();
                }
                if (obj != null) {
                    if (waitCoroutine != null) {
                        obj.StopCoroutine(waitCoroutine);
                    }
                    waitCoroutine = obj.StartCoroutine(WaitForTimeline(_flow, playableDirector));
                }
                else {
                    WaitForTimelineAsync(_flow, playableDirector);
                }
#endif
            }
        }

        protected virtual void ResumeTimeline(CommandExecutionFlow _flow, PlayableDirector playableDirector)
        {
            playableDirector.gameObject.SetActive(true);
            playableDirector.Resume();
            if (!asynchronous) {
#if MEC_AVAILABLE
                if (waitCoroutine != null) {

                    Timing.KillCoroutines(waitCoroutine);
                }
                waitCoroutine = Timing.RunCoroutine(WaitForTimeline(_flow, playableDirector));
#else
                MonoBehaviour obj;
                if (_flow.executor.targetObject is MonoBehaviour mb){
                    obj = mb;
                }
                else {
                    obj = playableDirector.GetComponent<MonoBehaviour>();
                }
                if (obj != null) {
                    if (waitCoroutine != null) {
                        obj.StopCoroutine(waitCoroutine);
                    }
                    waitCoroutine = obj.StartCoroutine(WaitForTimeline(_flow, playableDirector));
                }
                else {
                    WaitForTimelineAsync(_flow, playableDirector);
                }
#endif
            }

        }

        protected virtual void PauseTimeline(CommandExecutionFlow _flow, PlayableDirector playableDirector) {
            playableDirector.Pause();
            if (!asynchronous) {
#if MEC_AVAILABLE
                if (waitCoroutine != null) {
                    Timing.KillCoroutines(waitCoroutine);
                }
#else
                if (waitCoroutine != null) {
                    MonoBehaviour obj;
                    if (_flow.executor.targetObject is MonoBehaviour mb){
                        obj = mb;
                    }
                    else {
                        obj = playableDirector.GetComponent<MonoBehaviour>();
                    }
                    if (obj != null) {
                        obj.StopCoroutine(waitCoroutine);
                    }
                }
#endif
                OnFinishedExecution();
            }

        }

#if MEC_AVAILABLE
        private IEnumerator<float> WaitForTimeline(CommandExecutionFlow _flow, PlayableDirector _playableDirector) {
#else
        private IEnumerator WaitForTimeline(CommandExecutionFlow _flow, PlayableDirector _playableDirector) {
#endif
            while (true) {
                OnWaitForTimeline(_flow, _playableDirector);
                if (_playableDirector.time >= _playableDirector.duration - /*0.05f*/ (2*Time.deltaTime)) {
                    break;
                }
#if MEC_AVAILABLE
                yield return 0;
#else
                yield return null;
#endif
            }
            OnWaitForTimelineDone(_flow);
            OnFinishedExecution();
        }

        private async void WaitForTimelineAsync(CommandExecutionFlow _flow, PlayableDirector _playableDirector) {
            bool canceled = false;
#if UNITY_EDITOR
            AsyncUtility.RegisterOnExitPlayMode(() => canceled = true);
#endif
            System.Action<PlayableDirector> onStopped = null;
            onStopped = _pb => {
                canceled = true;
                _playableDirector.stopped -= onStopped;
            };
            _playableDirector.stopped += onStopped;
            while (true) {
#if UNITY_EDITOR
                if (!Application.isPlaying) {
                    return;
                }
#endif
                if (canceled) return;
                OnWaitForTimeline(_flow, _playableDirector);
                if (_playableDirector.time >= _playableDirector.duration - /*0.05f*/ (2 * Time.deltaTime)) {
                    break;
                }
                await System.Threading.Tasks.Task.Yield();
            }
            OnWaitForTimelineDone(_flow);
            OnFinishedExecution();
        }

        /// <summary>
        /// Callback for synchronous timeline.
        /// </summary>
        /// <param name="_playableDirector"></param>
        protected virtual void OnWaitForTimeline(CommandExecutionFlow _flow, PlayableDirector _playableDirector) {

        }

        /// <summary>
        /// Callback for synchronous timeline.
        /// </summary>
        /// <param name="_flow"></param>
        protected virtual void OnWaitForTimelineDone(CommandExecutionFlow _flow) {

        }
#endif
    }
}
