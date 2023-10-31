
using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Changes/modifies the text of a TextMeshPro Text component, and apply animation to it.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Text/Text Animation", typeof(SequineFlowCommandData))]
    public class TextAnimationCommand : Command {

        private int textLength;

        public override void Execute(CommandExecutionFlow _flow) {
            var target_value = GetSequineText(_flow);
            var text_value = GetInput<System.String>(_flow, IN_PORT_TEXT, text);

            if (target_value != null) {
                if (!appendWithPreviousText) {
                    target_value.ResetText();
                    if (string.IsNullOrEmpty(text_value)) {
                        Exit();
                        return;
                    }
                }
                if (behaviourProfile == null) {
                    target_value.text += text_value;
                }
                else {
                    target_value.AppendText(text_value, behaviourProfile, !appendWithPreviousText, () => RunSubFlow(_flow, OUT_PORT_ONCOMPLETE));
                }
                textLength = text_value.Length;
            }

            Exit();
        }

        protected SequineText GetSequineText(CommandExecutionFlow _flow) {
            if (inputIds[IN_PORT_TARGET].targetId != 0) {
                var inputObject = GetInput<object>(_flow, IN_PORT_TARGET);
                if (inputObject is SequineText sequineText) {
                    return sequineText;
                }
                else if (inputObject is UnityEngine.Object) {
                    return CrossSceneBindingUtility.GetObject(target) as SequineText;
                }
                else {
                    Debug.LogError("Error at command ID " + id + ": The connected input towards the Target is neither a Sequine Text component or Binder Asset.");
                    return null;
                }
            }
            else {
                if (_flow.executor.targetObject is MonoBehaviour) {
                    if (target is SequineText sequineText) {
                        return sequineText;
                    }
                }
            }
            if (target is UnityEngine.Object) {
                return CrossSceneBindingUtility.GetObject(target) as SequineText;
            }
            return null;
        }

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            return (T)(object)(behaviourProfile.delayPerCharacter * (float)textLength + behaviourProfile.durationPerCharacter);
        }

        #region PROPERTIES
        public override float nodeWidth => 300f;

        private const int IN_PORT_TARGET = 0;
        private const int IN_PORT_TEXT = 1;

        private const int OUT_PORT_ONCOMPLETE = 1;
        private const int OUT_PORT_TOTALDURATION = 2;

        public UnityEngine.Object target;
        public System.String text;
        public System.Boolean appendWithPreviousText;
        public TextBehaviourProfile behaviourProfile;
        #endregion PROPERTIES

        #region NODE-DRAWER
#if UNITY_EDITOR
        public bool editor_targetCrossSceneObject;
        public override void Editor_InitInPoints() {
            CommandGUI.AddMainInPoint();
            if (inputIds.Count <= IN_PORT_TARGET) {
                inputIds.Add(new ConnectionTarget());
            }
            if (inputIds.Count <= IN_PORT_TEXT) {
                inputIds.Add(new ConnectionTarget());
            }
            CommandGUI.AddPropertyInPoint<UnityEngine.Object>();
            CommandGUI.AddPropertyInPoint<System.String>();
        }

        public override void Editor_InitOutPoints() {
            CommandGUI.AddMainOutPoint();
            if (nextIds.Count <= OUT_PORT_ONCOMPLETE) {
                nextIds.Add(new List<ConnectionTarget>());
            }
            if (nextIds.Count <= OUT_PORT_TOTALDURATION) {
                nextIds.Add(new List<ConnectionTarget>());
            }
            CommandGUI.AddMainOutPoint();
            CommandGUI.AddPropertyOutPoint<float>();
        }

        public override void Editor_OnDrawTitle(out string _tooltip) {
            _tooltip = @"Set text to a sequine text, and play the animation.";
            CommandGUI.DrawInPoint(0);
            CommandGUI.DrawOutPoint(0);
        }
                
        public override void Editor_OnDrawContents(Vector2 _absPosition) {
            if (inputIds[IN_PORT_TARGET].targetId == 0) {
                if (CommandGUI.currentTargetObject is MonoBehaviour) {
                    CommandGUI.DrawToggleLeftField("Target Cross Scene Object", "If the Sequine Text is located in another loaded scene, then you need to use Cross Scene Binder component, and bind the Sequine Text with a unique asset object.", ref editor_targetCrossSceneObject);
                }
            }
            CommandGUI.DrawInPoint(IN_PORT_TARGET + 1);
            if (inputIds[IN_PORT_TARGET].targetId == 0) {
                if (CommandGUI.currentTargetObject is MonoBehaviour) {
                    if (editor_targetCrossSceneObject) {
                        CommandGUI.DrawBinderField("Target Binder", target, _target => target = _target);
                    }
                    else {
                        CommandGUI.DrawObjectField("Target", target as SequineText, _target => target = _target, true);
                    }
                }
                else {
                    CommandGUI.DrawBinderField("Target Binder", target, _target => target = _target);
                }
            }
            else {
                CommandGUI.DrawLabel("Target");
            }
            CommandGUI.DrawInPoint(IN_PORT_TEXT + 1);
            if (inputIds[IN_PORT_TEXT].targetId == 0) {    
                CommandGUI.DrawTextAreaField("Text", "", ref text, 3);
            }
            else {
                CommandGUI.DrawLabel("Text");
            }
            CommandGUI.DrawObjectField("Behaviour Profile", "", behaviourProfile, _behaviourProfile => behaviourProfile = _behaviourProfile, false);
            CommandGUI.DrawToggleLeftField("Append with Previous Text", "If not, then the text will be cleared first.", ref appendWithPreviousText);
            CommandGUI.DrawOutPoint(OUT_PORT_ONCOMPLETE);
            CommandGUI.DrawLabel("On Complete", true);
            CommandGUI.DrawOutPoint(OUT_PORT_TOTALDURATION);
            CommandGUI.DrawLabel("Total Duration", true);
        }
                
#endif
        #endregion NODE-DRAWER

    }

}