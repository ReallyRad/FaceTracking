using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Plays an animation by specifying the animation clip asset.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Animation/Clip Control/Play Animation Clip", typeof(SequineFlowCommandData))]
    public class PlayAnimationClipCommand : SequineAnimationCommand {

        public const int IN_PORT_CLIP = 1;

        public AnimationClip clip;

        protected float calculatedDuration;

        public override float nodeWidth => 350f;
        protected override float GetTotalAnimationDuration(CommandExecutionFlow _flow) {
            return calculatedDuration;
        }


        public override void Execute(CommandExecutionFlow _flow) {
            SequinePlayer sequine = GetSequinePlayer(_flow);
            var clipValue = GetInput<AnimationClip>(_flow, IN_PORT_CLIP, clip);
            OnPlayClip(_flow, sequine, clipValue);
            Exit();
        }

        protected virtual void OnPlayClip(CommandExecutionFlow _flow, SequinePlayer _sequine, AnimationClip _clip) {
            calculatedDuration = _clip.length * lengthToPlay;
            _sequine.PlayAnimationClip(layer, _clip, GetAnimationConfig(), restart, () => ExecuteOnComplete(_flow));
        }


#if UNITY_EDITOR
        public override void Editor_OnDrawTitle(out string _tooltip) {
            base.Editor_OnDrawTitle(out _tooltip);
            _tooltip = "Plays an animation by specifying the animation clip asset.";
        }

        public override void Editor_InitInPoints() {
            base.Editor_InitInPoints();
            if (inputIds.Count <= IN_PORT_CLIP) {
                inputIds.Add(new ConnectionTarget());
            }
            CommandGUI.AddPropertyInPoint<UnityEngine.Object>();
        }

        protected override void Editor_OnDrawAnimationContent(Vector2 _absPosition) {
            base.Editor_OnDrawAnimationContent(_absPosition);
            CommandGUI.DrawInPoint(IN_PORT_CLIP+1);
            if (inputIds[IN_PORT_CLIP].targetId == 0) {
                CommandGUI.DrawObjectField("Clip", clip, _clip => clip = _clip);
            }
            else {
                CommandGUI.DrawLabel("Clip");
            }
        }
#endif

    }

}