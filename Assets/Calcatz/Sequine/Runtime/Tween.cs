using Calcatz.CookieCutter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Runs and manages a single Tween.
    /// </summary>
    [System.Serializable]
    public class Tween {

        private static readonly Dictionary<object, Action> cancellers = new Dictionary<object, Action>();

        public float duration = 1f;
        public bool timescaleIndependent;
        public EasingFunction.Ease easing = EasingFunction.Ease.EaseInOutQuad;

        public async void RunTween(object _cancelTracker, Action<float> _onUpdate, Action _onComplete, float _overrideDuration = -1) {
            if (_overrideDuration == -1) _overrideDuration = duration;
            bool canceled = false;
#if UNITY_EDITOR
            AsyncUtility.RegisterOnExitPlayMode(() => canceled = true);
#endif
            if (cancellers.ContainsKey(_cancelTracker)) {
                cancellers[_cancelTracker] = () => {
                    canceled = true;
                };
            }
            else {
                cancellers.Add(_cancelTracker, () => {
                    canceled = true;
                });
            }
            var easingFunction = EasingFunction.GetEasingFunction(easing);
            if (timescaleIndependent) {
                for (float t = Time.unscaledDeltaTime; t <= _overrideDuration; t += Time.unscaledDeltaTime) {
#if UNITY_EDITOR
                    if (!Application.isPlaying) {
                        return;
                    }
#endif
                    if (canceled) return;
                    _onUpdate.Invoke(easingFunction.Invoke(0f, 1f, t / _overrideDuration));
                    await Task.Yield();
                }
            }
            else {
                for (float t = Time.deltaTime; t <= _overrideDuration; t += Time.deltaTime) {
#if UNITY_EDITOR
                    if (!Application.isPlaying) {
                        return;
                    }
#endif
                    if (canceled) return;
                    _onUpdate.Invoke(easingFunction.Invoke(0f, 1f, t / _overrideDuration));
                    await Task.Yield();
                }
            }
            if (canceled) return;
            _onUpdate.Invoke(1);
            _onComplete?.Invoke();
            cancellers.Remove(_cancelTracker);
        }

    }

}