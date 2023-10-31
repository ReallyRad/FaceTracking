
using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Control whether to play, stop, pause, or resume Audio Source.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Audio/Audio Source Control", typeof(SequineFlowCommandData))]
    public class AudioSourceControlCommand : Command {

        public override void Execute(CommandExecutionFlow _flow) {
            var audioSource_value = GetAudioSource(_flow);

            if (audioSource_value != null) {
                switch(control) {
                    case Control.Play:
                        audioSource_value.Play();
                        break;
                    case Control.Stop:
                        audioSource_value.Stop();
                        break;
                    case Control.Pause:
                        audioSource_value.Pause();
                        break;
                    case Control.Resume:
                        audioSource_value.UnPause();
                        break;
                    default:
                        break;
                }
            }

            Exit();
        }

        protected AudioSource GetAudioSource(CommandExecutionFlow _flow) {
            if (inputIds[IN_PORT_TARGET].targetId != 0) {
                var inputObject = GetInput<object>(_flow, IN_PORT_TARGET);
                if (inputObject is AudioSource audioSource) {
                    return audioSource;
                }
                else if (inputObject is UnityEngine.Object) {
                    return CrossSceneBindingUtility.GetObject(target) as AudioSource;
                }
                else {
                    Debug.LogError("Error at command ID " + id + ": The connected input towards the Target is not an Audio Source component.");
                    return null;
                }
            }
            else {
                if (_flow.executor.targetObject is MonoBehaviour) {
                    if (target is AudioSource audioSource) {
                        return audioSource;
                    }
                }
            }
            if (target is UnityEngine.Object) {
                return CrossSceneBindingUtility.GetObject(target) as AudioSource;
            }
            return null;
        }

        #region PROPERTIES
        public override float nodeWidth => 300f;

        private const int IN_PORT_TARGET = 0;

        public UnityEngine.Object target;
        public Control control;

        public enum Control {
            Play, Stop, Pause, Resume
        }
        #endregion PROPERTIES

        #region NODE-DRAWER
#if UNITY_EDITOR
        public bool editor_targetCrossSceneObject;
        public override void Editor_InitInPoints() {
            CommandGUI.AddMainInPoint();
            if (inputIds.Count <= IN_PORT_TARGET) {
                inputIds.Add(new ConnectionTarget());
            }
            CommandGUI.AddPropertyInPoint<UnityEngine.Object>();
        }

        public override void Editor_InitOutPoints() {
            CommandGUI.AddMainOutPoint();
        }

        public override void Editor_OnDrawTitle(out string _tooltip) {
            _tooltip = @"Control whether to play, stop, pause, or resume Audio Source.";
            CommandGUI.DrawInPoint(0);
            CommandGUI.DrawOutPoint(0);
        }
                
        public override void Editor_OnDrawContents(Vector2 _absPosition) {
            if (inputIds[IN_PORT_TARGET].targetId == 0) {
                if (CommandGUI.currentTargetObject is MonoBehaviour) {
                    CommandGUI.DrawToggleLeftField("Target Cross Scene Object", "If the Audio Source is located in another loaded scene, then you need to use Cross Scene Binder component, and bind the Audio Source with a unique asset object.", ref editor_targetCrossSceneObject);
                }
            }
            CommandGUI.DrawInPoint(IN_PORT_TARGET + 1);
            if (inputIds[IN_PORT_TARGET].targetId == 0) {
                if (CommandGUI.currentTargetObject is MonoBehaviour) {
                    if (editor_targetCrossSceneObject) {
                        CommandGUI.DrawBinderField("Target Binder", target, _target => target = _target);
                    }
                    else {
                        CommandGUI.DrawObjectField("Target", target as AudioSource, _target => target = _target, true);
                    }
                }
                else {
                    CommandGUI.DrawBinderField("Target Binder", target, _target => target = _target);
                }
            }
            else {
                CommandGUI.DrawLabel("Target");
            }
            CommandGUI.DrawEnumField("Control", ref control);
        }
                
#endif
        #endregion NODE-DRAWER

    }

}