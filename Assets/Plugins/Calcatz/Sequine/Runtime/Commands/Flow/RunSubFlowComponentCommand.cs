
using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Executes a Sequine Flow Component, and continue to next command only when the specified Sequine Flow Component finished its execution.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Flow/Run Sub-Flow (Component)", typeof(SequineFlowCommandData))]
    public class RunSubFlowComponentCommand : Command, ISubDataCommand {

        public CommandData subData { 
            get {
                if (executedComponent == null) {
                    if (target is SequineFlowComponent sequineFlowComponent) {
                        return sequineFlowComponent.commandData;
                    }
                    else {
                        return null;
                    }
                }
                return executedComponent.commandData;
            }
        }

        private SequineFlowComponent executedComponent;

        public override void Execute(CommandExecutionFlow _flow) {
            executedComponent = GetSequineFlowComponent(_flow);

            if (executedComponent != null) {
                _flow.RunSubData(id, executedComponent.commandData);
            }
            else {
                Debug.LogError("Sequine Flow Component is not assigned or Input not connected. Performing next command...");
                Exit();
            }

        }

        protected SequineFlowComponent GetSequineFlowComponent(CommandExecutionFlow _flow) {
            if (inputIds[IN_PORT_TARGET].targetId != 0) {
                var inputObject = GetInput<object>(_flow, IN_PORT_TARGET);
                if (inputObject is SequineFlowComponent sequineFlowComponent) {
                    return sequineFlowComponent;
                }
                else if (inputObject is UnityEngine.Object) {
                    return CrossSceneBindingUtility.GetObject(target) as SequineFlowComponent;
                }
                else {
                    Debug.LogError("Error at command ID " + id + ": The connected input towards the Target is neither a Sequine Flow Component nor Binder Asset.");
                    return null;
                }
            }
            else {
                if (_flow.executor.targetObject is MonoBehaviour) {
                    if (target is SequineFlowComponent sequineFlowComponent) {
                        return sequineFlowComponent;
                    }
                }
            }
            if (target is UnityEngine.Object) {
                return CrossSceneBindingUtility.GetObject(target) as SequineFlowComponent;
            }
            return null;
        }


        #region PROPERTIES
        public override float nodeWidth => 355f;

        private const int IN_PORT_TARGET = 0;

        
        public UnityEngine.Object target;
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
            _tooltip = @"Execute the Sequine Flow Component as a sub-flow, and proceed to the next command once the sub-flow is complete.";
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
            var labelWidth = UnityEditor.EditorGUIUtility.labelWidth;
            UnityEditor.EditorGUIUtility.labelWidth = 170;
            if (editor_targetCrossSceneObject || !(CommandGUI.currentTargetObject is MonoBehaviour)) {
                if (inputIds[IN_PORT_TARGET].targetId == 0) {
                    CommandGUI.DrawBinderField("SFlow Component Binder", "The asset that binds the Sequine Flow Component.", target, _target => target = _target);
                }
                else {
                    CommandGUI.DrawLabel("SFlow Component Binder", "The asset that binds the Sequine Flow Component.");
                }
            }
            else {
                if (inputIds[IN_PORT_TARGET].targetId == 0) {
                    CommandGUI.DrawObjectField<SequineFlowComponent>("Sequine Flow Component", "", target as SequineFlowComponent, _target => target = _target, true);
                }
                else {
                    CommandGUI.DrawLabel("Sequine Flow Component");
                }
            }
            UnityEditor.EditorGUIUtility.labelWidth = labelWidth;
        }
                
#endif
#endregion NODE-DRAWER

    }

}