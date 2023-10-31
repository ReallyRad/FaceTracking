using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Plays an action (animation that is listed in a Sequine Animation Data asset) by specifying the Action ID, and apply a simple translation along the animation.
    /// 
    /// This is a simple merge between Play Action Animation command and Tween Position command, where the duration of the tween is automatically set using the animation's duration.
    /// </summary>

    [System.Serializable]
    [RegisterCommand("Animation/Clip Control/Play Action Anim With Translation", typeof(SequineFlowCommandData))]
    public class PlayActionAnimationWithTranslationCommand : SequineActionAnimationCommand {

        public Vector3 translation;
        public EasingFunction.Ease easing = EasingFunction.Ease.Linear;

        public override float nodeWidth => 350f;


        public override void Execute(CommandExecutionFlow _flow) {
            SequinePlayer sequine = GetSequinePlayer(_flow);
            sequine.PlayActionWithTranslation(actionId, translation, GetAnimationConfig(), restart, ()=>ExecuteOnComplete(_flow), easing);
            Exit();
        }


#if UNITY_EDITOR
        public override void Editor_OnDrawTitle(out string _tooltip) {
            base.Editor_OnDrawTitle(out _tooltip);
            _tooltip = @"Plays an action (animation that is listed in a Sequine Animation Data asset) by specifying the Action ID, and apply a simple translation along the animation.

This is a simple merge between Play Action Animation command and Tween Position command, where the duration of the tween is automatically set using the animation's duration.";
        }
        protected override void Editor_OnDrawAnimationContent(Vector2 _absPosition) {
            base.Editor_OnDrawAnimationContent(_absPosition);
            CommandGUI.DrawVector3Field("Translation", ref translation);
            CommandGUI.DrawEnumField("Easing", ref easing);
        }
#endif

    }

}