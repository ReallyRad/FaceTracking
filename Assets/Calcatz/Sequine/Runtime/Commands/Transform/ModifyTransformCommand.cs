using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Base class for commands that modifies a Transform's value.
    /// </summary>
    [System.Serializable]
    public class ModifyTransformCommand : Command {

        public const int IN_PORT_TARGET = 0;

        public bool localSpace;

        public UnityEngine.Object transformTarget;

        public override float nodeWidth => 350f;

        protected virtual bool globalSpaceAvailable => true;

        protected Transform GetTransform(CommandExecutionFlow _flow) {
            if (inputIds[IN_PORT_TARGET].targetId != 0) {
                if (GetInput<object>(_flow, IN_PORT_TARGET) is Component component) {
                    return component.transform;
                }
                else {
                    var crossSceneTransform = GetCrossSceneTransform();
                    if (crossSceneTransform == null) {
                        Debug.LogError("Error at command ID " + id + ": The connected input towards the Target is neither a Transform, Component, nor a Binder Asset.");
                    }
                    return crossSceneTransform;
                }
            }
            else {
                if (_flow.executor.targetObject is MonoBehaviour) {
                    if (transformTarget is Transform) {
                        return transformTarget as Transform;
                    }
                    else {
                        return GetCrossSceneTransform();
                    }
                }
                else {
                    return GetCrossSceneTransform();
                }
            }
        }

        private Transform GetCrossSceneTransform() {
            if (CrossSceneBindingUtility.TryGetObject(transformTarget, out Component comp)) {
                return comp.transform;
            }
            else {
                var sequine = SequinePlayer.GetInstance(transformTarget as SequineAnimationData);
                if (sequine == null) {
                    Debug.LogError("Error at command ID " + id + ": Can't get any component from the specified transform binder.");
                    return null;
                }
                else {
                    return sequine.transform;
                }
            }
        }

#if UNITY_EDITOR
        public bool editor_targetCrossSceneObject;
        public override void Editor_InitInPoints() {
            base.Editor_InitInPoints();
            if (inputIds.Count <= IN_PORT_TARGET) {
                inputIds.Add(new ConnectionTarget());
            }
            CommandGUI.AddPropertyInPoint<UnityEngine.Object>();
        }

        public override void Editor_OnDrawContents(Vector2 _absPosition) {
            if (CommandGUI.currentTargetObject is MonoBehaviour) {
                if (inputIds[IN_PORT_TARGET].targetId == 0) {
                    UnityEditor.EditorGUI.BeginChangeCheck();
                    CommandGUI.DrawToggleLeftField("Target Cross Scene Object", "If the Transform target is located in another opened scene, then the Cross Scene Binder asset is treated as the bridge/binder to the transform.\nSo make sure to add Cross Scene Binder component to the game object, and then assign the Transform into its \"Component To Bind\" field.", ref editor_targetCrossSceneObject);
                    if (UnityEditor.EditorGUI.EndChangeCheck()) {
                        if (editor_targetCrossSceneObject) {
                            if (transformTarget is Transform) {
                                UnityEditor.Undo.RecordObject(CommandGUI.currentTargetObject, "reset transform target");
                                transformTarget = null;
                                UnityEditor.EditorUtility.SetDirty(CommandGUI.currentTargetObject);
                            }
                        }
                    }
                }
            }

            CommandGUI.DrawInPoint(1);
            if (inputIds[IN_PORT_TARGET].targetId == 0) {
                if (CommandGUI.currentTargetObject is MonoBehaviour) {
                    if (editor_targetCrossSceneObject) {
                        CommandGUI.DrawBinderField<UnityEngine.Object>("Transform Binder", transformTarget, _target => transformTarget = _target);
                    }
                    else {
                        CommandGUI.DrawObjectField("Transform", transformTarget as Transform, _target => transformTarget = _target, true);
                    }
                }
                else {
                    CommandGUI.DrawBinderField<UnityEngine.Object>("Transform Binder", transformTarget, _target => transformTarget = _target);
                }
            }
            else {
                CommandGUI.DrawLabel("Transform");
            }

            if (globalSpaceAvailable) {
                CommandGUI.DrawToggleField("Local Space", ref localSpace);
            }
        }
#endif


    }

}