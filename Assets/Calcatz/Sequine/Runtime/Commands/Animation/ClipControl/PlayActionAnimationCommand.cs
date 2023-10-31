using Calcatz.CookieCutter;
using Calcatz.Sequine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Plays an action (animation that is listed in a Sequine Animation Data asset) by specifying the Action ID.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Animation/Clip Control/Play Action Animation", typeof(SequineFlowCommandData))]
    public class PlayActionAnimationCommand : SequineActionAnimationCommand {

        public override void Execute(CommandExecutionFlow _flow) {
            SequinePlayer sequine = GetSequinePlayer(_flow);
            sequine.PlayAction(layer, actionId, GetAnimationConfig(), restart, () => ExecuteOnComplete(_flow));
            Exit();
        }

#if UNITY_EDITOR
        public override void Editor_OnDrawTitle(out string _tooltip) {
            base.Editor_OnDrawTitle(out _tooltip);
            _tooltip = "Plays an action (animation that is listed in a Sequine Animation Data asset) by specifying the Action ID.";
        }
#endif

    }

}