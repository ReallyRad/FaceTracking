using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Calcatz.CookieCutter;
using System;

namespace Calcatz.Sequine {

    [CommandNodesContainer(typeof(SequineFlowCommandData))]
    public class SequineFlowNodesContainer : CommandNodesContainer {

        public SequineFlowNodesContainer() {
            leftPaneWidth = 300;
            useVariables = true;
            showCreateVariableNodeButton = true;
        }

        protected override void ValidateData() {
            //base.ValidateData();
            if (!commandData.commands.ContainsKey(1)) {
                commandData.commands.Add(1, new SequineFlowCommand() { id = 1, nodePosition = Vector2.zero + Vector2.up * 200 });
            }
            /*if (!commandData.commands.ContainsKey(2)) {
                commandData.commands.Add(2, new SequineFlowCommand() { id = 2, nodePosition = commandData.commands[1].nodePosition - Vector2.up * 200 });
            }*/
        }

        protected override void ValidateAvailableCommands(Type _commandDataType) {
            base.ValidateAvailableCommands(_commandDataType);
            CommandRegistry.RegisterCommand<SequineFlowCommandData>(typeof(SequineFlowCommand), false, "");
        }

        protected override void OnReloadReservedNodes(CommandNode _node) {
            switch (_node.GetCommand().id) {
                case 1:
                    SetNodeStyle(_node, "5");
                    break;
                /*case 2:
                    SetNodeStyle(_node, "2");
                    break;*/
            }
        }

    }
}