
using Calcatz.CookieCutter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Executes a Sequine Flow Asset, and continue to next command only when the specified Sequine Flow Asset finished its execution.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Flow/Run Sub-Flow (Asset)", typeof(SequineFlowCommandData))]
    public class RunSubFlowAssetCommand : Command, ISubDataCommand {

        public CommandData subData { 
            get {
                if (executedAsset == null) {
                    if (sequineFlowAsset == null) {
                        return null;
                    }
                    else {
                        return sequineFlowAsset.commandData;
                    }
                }
                return executedAsset.commandData;
            }
        }

        private SequineFlowAsset executedAsset;

        public override void Execute(CommandExecutionFlow _flow) {
            executedAsset = GetInput(_flow, IN_PORT_SEQUINE_FLOW_ASSET, sequineFlowAsset);

            if (executedAsset != null) {
                _flow.RunSubData(id, executedAsset.commandData);
            }
            else {
                Debug.LogError("Sequine Flow Asset is not assigned or Input not connected. Performing next command...");
                Exit();
            }

        }


#region PROPERTIES
        public override float nodeWidth => 300f;

        private const int IN_PORT_SEQUINE_FLOW_ASSET = 0;


        public SequineFlowAsset sequineFlowAsset;
#endregion PROPERTIES

#region NODE-DRAWER
#if UNITY_EDITOR
        public override void Editor_InitInPoints() {
            CommandGUI.AddMainInPoint();
            if (inputIds.Count <= IN_PORT_SEQUINE_FLOW_ASSET) {
                inputIds.Add(new ConnectionTarget());
            }
            CommandGUI.AddPropertyInPoint<UnityEngine.Object>();
        }

        public override void Editor_InitOutPoints() {
            CommandGUI.AddMainOutPoint();
        }

        public override void Editor_OnDrawTitle(out string _tooltip) {
            _tooltip = @"Execute the Sequine Flow Asset as a sub-flow, and proceed to the next command once the sub-flow is complete.";
            CommandGUI.DrawInPoint(0);
            CommandGUI.DrawOutPoint(0);
        }
                
        public override void Editor_OnDrawContents(Vector2 _absPosition) {
            CommandGUI.DrawInPoint(IN_PORT_SEQUINE_FLOW_ASSET + 1);
            if (inputIds[IN_PORT_SEQUINE_FLOW_ASSET].targetId == 0) {    
                CommandGUI.DrawObjectField("Sequine Flow Asset", "", sequineFlowAsset, _sequineFlowAsset => sequineFlowAsset = _sequineFlowAsset, false);
            }
            else {
                CommandGUI.DrawLabel("Sequine Flow Asset");
            }
        }
                
#endif
#endregion NODE-DRAWER

    }

}