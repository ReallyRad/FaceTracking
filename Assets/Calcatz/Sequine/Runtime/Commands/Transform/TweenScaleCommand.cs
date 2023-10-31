using Calcatz.CookieCutter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Tweens the transform's local scale by interpolating it over time towards a target local scale after a certain duration.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Transform/Tween Scale", typeof(SequineFlowCommandData))]
    public class TweenScaleCommand : TransformTweenCommand {

        public const int IN_PORT_SCALE = 2;

        public const int OUT_PORT_SCALE = 2;

        public Vector3 endLocalScale;

        protected override bool globalSpaceAvailable => false;

        public override void Execute(CommandExecutionFlow _flow) {
            Vector3 val;
            if (inputIds[IN_PORT_SCALE].targetId != 0) {
                val = GetInput<Vector3>(_flow, IN_PORT_SCALE);
            }
            else {
                val = endLocalScale;
            }

            var transform = GetTransform(_flow);
            Vector3 initialScale;
            initialScale = transform.localScale;

            StartTweening(_flow, _t => {
                transform.localScale = Vector3.Lerp(initialScale, val, _t);
            }, () => {
                ExecuteOnComplete(_flow);
            });

            Exit();
        }

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            return (T)(object)GetTransform(_flow).localScale;
        }


#if UNITY_EDITOR
        public override void Editor_InitInPoints() {
            base.Editor_InitInPoints();
            if (inputIds.Count <= IN_PORT_SCALE) {
                inputIds.Add(new ConnectionTarget());
            }
            CommandGUI.AddPropertyInPoint<Vector3>();
        }
        public override void Editor_InitOutPoints() {
            base.Editor_InitOutPoints();
            if (nextIds.Count <= OUT_PORT_SCALE) {
                nextIds.Add(new List<ConnectionTarget>());
            }
            CommandGUI.AddPropertyOutPoint<Vector3>();
        }
        public override void Editor_OnDrawContents(Vector2 _absPosition) {
            base.Editor_OnDrawContents(_absPosition);
            CommandGUI.DrawInPoint(IN_PORT_SCALE + 1);
            if (inputIds[IN_PORT_SCALE].targetId != 0) {
                CommandGUI.DrawLabel("End Local Scale", "Local scale to apply on the transform.");
            }
            else {
                CommandGUI.DrawVector3Field("End Local Scale", "Local scale to apply on the transform.", ref endLocalScale);
            }
            CommandGUI.DrawOutPoint(OUT_PORT_ONCOMPLETE);
            CommandGUI.DrawLabel("On Complete", true);
            CommandGUI.DrawOutPoint(OUT_PORT_SCALE);
            CommandGUI.DrawLabel("End Local Scale", "Local scale applied after this command is executed. Technically the value will be the same with the input field above.", true);
        }
#endif

    }

}