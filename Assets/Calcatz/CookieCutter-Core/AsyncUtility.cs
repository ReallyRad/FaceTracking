using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {

    public static class AsyncUtility {

#if UNITY_EDITOR
        /// <summary>
        /// Only call this using #if UNITY_EDITOR.
        /// The registered action will be invoked once, and removed immediately.
        /// </summary>
        /// <param name="_action"></param>
        public static void RegisterOnExitPlayMode(System.Action _action) {
            System.Action<UnityEditor.PlayModeStateChange> onPlayModeStateChanged = null;
            onPlayModeStateChanged = _stateChange => {
                if (_stateChange == UnityEditor.PlayModeStateChange.ExitingPlayMode) {
                    _action?.Invoke();
                    UnityEditor.EditorApplication.playModeStateChanged -= onPlayModeStateChanged;
                }
            };
            UnityEditor.EditorApplication.playModeStateChanged += onPlayModeStateChanged;
        }
#endif

    }

}
