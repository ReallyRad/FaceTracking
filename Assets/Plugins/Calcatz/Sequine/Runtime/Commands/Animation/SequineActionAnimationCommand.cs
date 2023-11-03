using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Base class for commands that targets a Sequine Player component that plays an action animation.
    /// </summary>
    [System.Serializable]
    public class SequineActionAnimationCommand : SequineAnimationCommand {

        public int actionId;

        protected override float GetTotalAnimationDuration(CommandExecutionFlow _flow) {
            var sequinePlayer = GetSequinePlayer(_flow);
            if (sequinePlayer != null) {
                if (sequinePlayer.TryGetActionClip(actionId, out var clip)) {
                    return clip.length * lengthToPlay;
                }
            }
            return 0f;
        }

#if UNITY_EDITOR
        protected override void Editor_OnDrawAnimationContent(Vector2 _absPosition) {
            DrawActionsField(actionId, OnActionIDChanged);
        }

        private void OnActionIDChanged(int _actionId) {
            actionId = _actionId;
        }
#endif

    }

}