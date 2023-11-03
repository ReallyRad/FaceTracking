using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
#if TIMELINE_AVAILABLE
using UnityEngine.Timeline;
#endif

namespace Calcatz.CookieCutter {
    /// <summary>
    /// Enables editing command nodes inside inspector through the preview section.
    /// </summary>
    public class CommandDataInspectorPreview : ObjectPreview {

        public override bool HasPreviewGUI() {
            return true;
        }

        private CommandNodesContainer nodesContainer; 

        public override void Initialize(Object[] targets) {
            base.Initialize(targets);
            if (targets.Length != 1) {
                nodesContainer = null;
                return;
            }
            ICommandData commandDataContainer = (ICommandData)targets[0];
            if (commandDataContainer.commandData == null) {
                commandDataContainer.ValidateObject();
            }
            nodesContainer = CommandNodesContainer.CreateFromCommandData(commandDataContainer.commandData);
            nodesContainer.onValidateCommandDataReference = ValidateCommandDataReference;
        }

        public override GUIContent GetPreviewTitle() {
            return new GUIContent("Command Nodes");
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background) {
            if (nodesContainer == null) return;
            base.OnInteractivePreviewGUI(r, background);

            var refreshButtonRect = new Rect(r.x + r.width - 70, r.y + 5, 65, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(refreshButtonRect, "Refresh")) {
                nodesContainer.RepaintIfDirty();
            }

            nodesContainer.containerAreaGetter = () => r;
            nodesContainer.OnDrawNodesArea();
        }

        protected virtual void ValidateCommandDataReference() {
            if (nodesContainer.commandData == null) {
                if (Selection.activeObject != null) {
                    if (Selection.activeObject is GameObject go) {
                        ICommandData commandDataComponent = go.GetComponent<ICommandData>();
                        if (commandDataComponent != null) {
                            nodesContainer.commandData = commandDataComponent.commandData;
                        }
                    }
                    else if (Selection.activeObject is ICommandData commandDataAsset) {
                        nodesContainer.commandData = commandDataAsset.commandData;
                    }
#if TIMELINE_AVAILABLE
                    else {
                        //Use this if some time later Unity changes m_Clip variable name
                        /*FieldInfo[] fields = Selection.activeObject.GetType().GetRuntimeFields().ToArray();
                        foreach (FieldInfo f in fields) {
                            Debug.Log(f.Name);
                        }*/
                        FieldInfo field = Selection.activeObject.GetType().GetField("m_Clip", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                        if (field != null) {
                            TimelineClip clip = field.GetValue(Selection.activeObject) as TimelineClip;
                            if (clip != null) {
                                if (clip.asset is ICommandData commandDataClip) {
                                    nodesContainer.commandData = commandDataClip.commandData;
                                }
                            }
                        }
                    }
#endif
                }
            }

        }

    }
}
