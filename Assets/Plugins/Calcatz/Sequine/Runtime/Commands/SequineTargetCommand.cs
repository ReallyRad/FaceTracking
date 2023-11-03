using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Base class for commands that targets a Sequine Player component.
    /// </summary>
    [System.Serializable]
    public class SequineTargetCommand : Command {

        public const int IN_PORT_TARGET = 0;

        public UnityEngine.Object target;

        public override float nodeWidth => 350f;

        /// <summary>
        /// Get the suitable Sequine Player component.
        /// </summary>
        /// <param name="_flow">The flow context</param>
        /// <returns>Sequine Player target</returns>
        protected SequinePlayer GetSequinePlayer(CommandExecutionFlow _flow) {
            if (inputIds[IN_PORT_TARGET].targetId != 0) {
                var inputObject = GetInput<object>(_flow, IN_PORT_TARGET);
                if (inputObject is SequinePlayer sequinePlayer) {
                    return sequinePlayer;
                }
                else if (inputObject is SequineAnimationData animData) {
                    return SequinePlayer.GetInstance(animData);
                }
                else if (inputObject is UnityEngine.Object) {
                    if (CrossSceneBindingUtility.TryGetObject<SequinePlayer>(target, out var seqPlayer)) {
                        return seqPlayer;
                    }
                    else {
                        Debug.LogError(GetType().Name + " - Error at command ID " + id + ": The connected input towards the Target is neither a Sequine Player component nor Binder Asset.");
                        return null;
                    }
                }
                else {
                    Debug.LogError(GetType().Name + " - Error at command ID " + id + ": The connected input towards the Target is neither a Sequine Player component nor Binder Asset.");
                    return null;
                }
            }
            else {
                if (_flow.executor.targetObject is MonoBehaviour) {
                    if (target is SequinePlayer) {
                        return target as SequinePlayer;
                    }
                    else if (target is SequineAnimationData animData) {
                        return SequinePlayer.GetInstance(animData);
                    }
                }
                else {
                    return SequinePlayer.GetInstance(target as SequineAnimationData);
                }
            }
            if (target is UnityEngine.Object) {
                return CrossSceneBindingUtility.GetObject(target) as SequinePlayer;
            }
            Debug.LogError(GetType().Name + ": Can't get the Sequine Player, make sure it is assigned.");
            return null;
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
            var labelWidth = UnityEditor.EditorGUIUtility.labelWidth;
            UnityEditor.EditorGUIUtility.labelWidth = 100;

            if (inputIds[IN_PORT_TARGET].targetId == 0) {
                if (CommandGUI.currentTargetObject is MonoBehaviour) {
                    CommandGUI.DrawToggleLeftField("Target Cross Scene Object", "If the Sequine Player is located in another loaded scene, then the Animation Data asset is treated as the bridge/binder to the component.\nSo make sure the Animation Data is not set to null in the Sequine Player component.", ref editor_targetCrossSceneObject);
                }
            }
            CommandGUI.DrawInPoint(IN_PORT_TARGET+1);
            if (inputIds[IN_PORT_TARGET].targetId == 0) {
                if (CommandGUI.currentTargetObject is MonoBehaviour) {
                    if (editor_targetCrossSceneObject) {
                        DrawTargetBinderField();
                    }
                    else {
                        CommandGUI.DrawObjectField("Target", target as SequinePlayer, _target => target = _target, true);
                    }
                }
                else {
                    DrawTargetBinderField();
                }
            }
            else {
                CommandGUI.DrawLabel("Target");
            }

            UnityEditor.EditorGUIUtility.labelWidth = labelWidth;
        }

        private void DrawTargetBinderField() {
            CommandGUI.DrawBinderField("Target Binder", target as SequineAnimationData, _target => target = _target, _draggedObject => {
                if (_draggedObject is SequinePlayer sp) {
                    return sp.animationData;
                }
                else if (_draggedObject is GameObject go) {
                    if (go.TryGetComponent<SequinePlayer>(out var sPlayer)) {
                        return sPlayer.animationData;
                    }
                }
                else if (_draggedObject is Component comp) {
                    if (comp.TryGetComponent<SequinePlayer>(out var sPlayer)) {
                        return sPlayer.animationData;
                    }
                }
                return null;
            });
        }

        private static readonly string[] emptyArray = new string[] { "None" };

        protected void DrawActionsField(int _actionId, System.Action<int> _onActionIdChanged) {
            if (target != null) {
                SequineAnimationData animationData;
                if (CommandGUI.currentTargetObject is MonoBehaviour && !editor_targetCrossSceneObject) {
                    SequinePlayer sequinePlayer = target as SequinePlayer;
                    if (sequinePlayer != null) {
                        animationData = sequinePlayer.animationData;
                    }
                    else {
                        animationData = null;
                    }
                }
                else {
                    animationData = target as SequineAnimationData;
                }
                if (animationData != null) {
                    if (animationData.actionClips == null)
                        animationData.actionClips = new SequineAnimationData.ActionClip[0];

                    List<string> actions = new List<string>();
                    int index = -1;
                    for (int i = 0; i < animationData.actionClips.Length; i++) {
                        var actionClip = animationData.actionClips[i];
                        actions.Add(actionClip.id + ": " + actionClip.clip.name);
                        if (actionClip.id == _actionId) index = i;
                    }
                    var guiColor = GUI.color;
                    string label = "Action";
                    if (index < 0 && _actionId != 0) {
                        label += " (Missing Id: " + _actionId + ")";
                        GUI.color = new Color(0.8f, 0.2f, 0.2f);
                    }
                    CommandGUI.DrawPopupField(label, index, actions.ToArray(), _index => {
                        _onActionIdChanged(animationData.actionClips[_index].id);
                    });
                    GUI.color = guiColor;
                }
            }
            else {
                bool guiEnabled = GUI.enabled;
                GUI.enabled = false;
                CommandGUI.DrawPopupField("Action", 0, emptyArray, null);
                GUI.enabled = guiEnabled;
            }
        }
#endif

    }

}