using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Tweens the transform's rotation by interpolating it over time towards a target rotation after a certain duration.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Transform/Tween Rotation", typeof(SequineFlowCommandData))]
    public class TweenRotationCommand : TransformTweenCommand {

        public const int IN_PORT_QUATERNION = 2;
        public const int IN_PORT_EULER = 3;

        public const int OUT_PORT_QUATERNION = 2;

        public bool useQuaternion;
        public Vector3 euler;

        public override void Execute(CommandExecutionFlow _flow) {
            Quaternion val;
            if (useQuaternion) {
                if (inputIds[IN_PORT_QUATERNION].targetId != 0) {
                    val = GetInput<Quaternion>(_flow, IN_PORT_QUATERNION);
                }
                else {
                    Debug.LogError("Quaternion value has not been set.");
                    val = Quaternion.identity;
                }
            }
            else {
                if (inputIds[IN_PORT_EULER].targetId != 0) {
                    val = Quaternion.Euler(GetInput<Vector3>(_flow, IN_PORT_EULER));
                }
                else {
                    val = Quaternion.Euler(euler);
                }
            }

            var transform = GetTransform(_flow);
            Quaternion initialRot;
            if (localSpace) {
                initialRot = transform.localRotation;
            }
            else {
                initialRot = transform.rotation;
            }

            if (localSpace) {
                StartTweening(_flow, _t => {
                    transform.localRotation = Quaternion.Lerp(initialRot, val, _t);
                }, () => {
                    ExecuteOnComplete(_flow);
                });
            }
            else {
                StartTweening(_flow, _t => {
                    transform.rotation = Quaternion.Lerp(initialRot, val, _t);
                }, () => {
                    ExecuteOnComplete(_flow);
                });
            }

            Exit();
        }

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            if (localSpace) {
                return (T)(object)GetTransform(_flow).localRotation;
            }
            else {
                return (T)(object)GetTransform(_flow).rotation;
            }
        }


#if UNITY_EDITOR
        public override void Editor_InitInPoints() {
            base.Editor_InitInPoints();
            if (inputIds.Count <= IN_PORT_QUATERNION) {
                inputIds.Add(new ConnectionTarget());
            }
            if (inputIds.Count <= IN_PORT_EULER) {
                inputIds.Add(new ConnectionTarget());
            }
            CommandGUI.AddPropertyInPoint<Quaternion>();
            CommandGUI.AddPropertyInPoint<Vector3>();
        }
        public override void Editor_InitOutPoints() {
            base.Editor_InitOutPoints();
            if (nextIds.Count <= OUT_PORT_QUATERNION) {
                nextIds.Add(new List<ConnectionTarget>());
            }
            CommandGUI.AddPropertyOutPoint<Quaternion>();
        }
        public override void Editor_OnDrawContents(Vector2 _absPosition) {
            base.Editor_OnDrawContents(_absPosition);
            CommandGUI.DrawToggleField("Use Quaternion", ref useQuaternion);
            bool guiEnabled = GUI.enabled;
            GUI.enabled = guiEnabled && useQuaternion;
            CommandGUI.DrawInPoint(IN_PORT_QUATERNION + 1);
            CommandGUI.DrawLabel("End Rotation (Quaternion)");
            GUI.enabled = guiEnabled && !useQuaternion;
            CommandGUI.DrawInPoint(IN_PORT_EULER + 1);
            if (inputIds[IN_PORT_EULER].targetId != 0) {
                CommandGUI.DrawLabel("End Rotation (Euler)", "Rotation to apply on the transform. (Local rotation if Local Space is checked)");
            }
            else {
                CommandGUI.DrawVector3Field("End Rotation (Euler)", "Rotation to apply on the transform. (Local rotation if Local Space is checked)", ref euler);
            }
            GUI.enabled = guiEnabled;
            CommandGUI.DrawOutPoint(OUT_PORT_ONCOMPLETE);
            CommandGUI.DrawLabel("On Complete", true);
            CommandGUI.DrawOutPoint(OUT_PORT_QUATERNION);
            CommandGUI.DrawLabel("Rotation (Quaternion)", "Rotation applied after this command is executed. (Local rotation if Local Space is checked). Technically the value will be the same with the input field above.", true);
        }
#endif

    }

}