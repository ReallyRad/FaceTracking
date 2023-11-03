using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Resumes the animation of the specified Sequine Player and continue its state.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Animation/Resume Sequine Player", typeof(SequineFlowCommandData))]
    public class ResumeSequinePlayerCommand : SequineTargetCommand {

        public AnimationClip clip;

        public override float nodeWidth => 350f;


        public override void Execute(CommandExecutionFlow _flow) {
            SequinePlayer sequine = GetSequinePlayer(_flow);
            sequine.Resume();
            Exit();
        }

#if UNITY_EDITOR
        public override void Editor_OnDrawTitle(out string _tooltip) {
            base.Editor_OnDrawTitle(out _tooltip);
            _tooltip = "Resumes the animation of the specified Sequine Player and continue its state.";
        }
#endif

    }

}