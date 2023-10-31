using Calcatz.CookieCutter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Sets transform's local scale.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Transform/Set Scale", typeof(SequineFlowCommandData))]
    public class SetScaleCommand : ModifyTransformCommand {

        public const int IN_PORT_SCALE = 1;

        public const int OUT_PORT_SCALE = 1;

        public Vector3 localScale;

        protected override bool globalSpaceAvailable => false;

        public override void Execute(CommandExecutionFlow _flow) {
            Vector3 val;
            if (inputIds[IN_PORT_SCALE].targetId != 0) {
                val = GetInput<Vector3>(_flow, IN_PORT_SCALE);
            }
            else {
                val = localScale;
            }

            GetTransform(_flow).localScale = val;
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
                CommandGUI.DrawLabel("Local Scale", "Local scale to apply on the transform.");
            }
            else {
                CommandGUI.DrawVector3Field("Local Scale", "Local scale to apply on the transform.", ref localScale);
            }
            CommandGUI.DrawOutPoint(OUT_PORT_SCALE);
            CommandGUI.DrawLabel("Local Scale", "Local scale applied after this command is executed. Technically the value will be the same with the input field above.", true);
        }
#endif

    }

}