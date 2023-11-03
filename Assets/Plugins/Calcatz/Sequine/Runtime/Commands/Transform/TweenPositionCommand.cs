using Calcatz.CookieCutter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Tweens the transform's position by interpolating it over time towards a target position after a certain duration.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Transform/Tween Position", typeof(SequineFlowCommandData))]
    public class TweenPositionCommand : TransformTweenCommand {

        public const int IN_PORT_POSITION = 2;

        public const int OUT_PORT_POSITION = 2;

        public Vector3 endPosition;

        public override void Execute(CommandExecutionFlow _flow) {
            Vector3 val;
            if (inputIds[IN_PORT_POSITION].targetId != 0) {
                val = GetInput<Vector3>(_flow, IN_PORT_POSITION);
            }
            else {
                val = endPosition;
            }

            var transform = GetTransform(_flow);
            Vector3 initialPos;
            if (localSpace) {
                initialPos = transform.localPosition;
            }
            else {
                initialPos = transform.position;
            }

            if (localSpace) {
                StartTweening(_flow, _t => {
                    transform.localPosition = Vector3.Lerp(initialPos, val, _t);
                }, () => {
                    ExecuteOnComplete(_flow);
                });
            }
            else {
                StartTweening(_flow, _t => {
                    transform.position = Vector3.Lerp(initialPos, val, _t);
                }, () => {
                    ExecuteOnComplete(_flow);
                });
            }

            Exit();
        }

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            if (localSpace) {
                return (T)(object)GetTransform(_flow).localPosition;
            }
            else {
                return (T)(object)GetTransform(_flow).position;
            }
        }


#if UNITY_EDITOR
        public override void Editor_InitInPoints() {
            base.Editor_InitInPoints();
            if (inputIds.Count <= IN_PORT_POSITION) {
                inputIds.Add(new ConnectionTarget());
            }
            CommandGUI.AddPropertyInPoint<Vector3>();
        }
        public override void Editor_InitOutPoints() {
            base.Editor_InitOutPoints();
            if (nextIds.Count <= OUT_PORT_POSITION) {
                nextIds.Add(new List<ConnectionTarget>());
            }
            CommandGUI.AddPropertyOutPoint<Vector3>();
        }
        public override void Editor_OnDrawContents(Vector2 _absPosition) {
            base.Editor_OnDrawContents(_absPosition);
            CommandGUI.DrawInPoint(IN_PORT_POSITION + 1);
            if (inputIds[IN_PORT_POSITION].targetId != 0) {
                CommandGUI.DrawLabel("End Position", "Position to apply on the transform. (Local position if Local Space is checked)");
            }
            else {
                CommandGUI.DrawVector3Field("End Position", "Position to apply on the transform. (Local position if Local Space is checked)", ref endPosition);
            }
            CommandGUI.DrawOutPoint(OUT_PORT_ONCOMPLETE);
            CommandGUI.DrawLabel("On Complete", true);
            CommandGUI.DrawOutPoint(OUT_PORT_POSITION);
            CommandGUI.DrawLabel("Position", "Position applied after this command is executed. (Local position if Local Space is checked). Technically the value will be the same with the input field above.", true);
        }
#endif

    }

}