using Calcatz.CookieCutter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Sets transform's position.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Transform/Set Position", typeof(SequineFlowCommandData))]
    public class SetPositionCommand : ModifyTransformCommand {

        public const int IN_PORT_POSITION = 1;

        public const int OUT_PORT_POSITION = 1;

        public Vector3 position;

        public override void Execute(CommandExecutionFlow _flow) {
            Vector3 val;
            if (inputIds[IN_PORT_POSITION].targetId != 0) {
                val = GetInput<Vector3>(_flow, IN_PORT_POSITION);
            }
            else {
                val = position;
            }
            if (localSpace) {
                GetTransform(_flow).localPosition = val;
            }
            else {
                GetTransform(_flow).position = val;
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
                CommandGUI.DrawLabel("Position", "Position to apply on the transform. (Local position if Local Space is checked)");
            }
            else {
                CommandGUI.DrawVector3Field("Position", "Position to apply on the transform. (Local position if Local Space is checked)", ref position);
            }
            CommandGUI.DrawOutPoint(OUT_PORT_POSITION);
            CommandGUI.DrawLabel("Position", "Position applied after this command is executed. (Local position if Local Space is checked). Technically the value will be the same with the input field above.", true);
        }
#endif

    }

}