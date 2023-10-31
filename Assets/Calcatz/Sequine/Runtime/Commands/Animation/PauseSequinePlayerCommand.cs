using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Pauses the entire animation of the specified Sequine Player.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Animation/Pause Sequine Player", typeof(SequineFlowCommandData))]
    public class PauseSequinePlayerCommand : SequineTargetCommand {

        public AnimationClip clip;

        public override float nodeWidth => 350f;


        public override void Execute(CommandExecutionFlow _flow) {
            SequinePlayer sequine = GetSequinePlayer(_flow);
            sequine.Pause();
            Exit();
        }

#if UNITY_EDITOR
        public override void Editor_OnDrawTitle(out string _tooltip) {
            base.Editor_OnDrawTitle(out _tooltip);
            _tooltip = "Pauses the entire animation of the specified Sequine Player.";
        }
#endif

    }

}