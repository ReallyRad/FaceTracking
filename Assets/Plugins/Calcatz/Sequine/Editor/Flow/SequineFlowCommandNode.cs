using UnityEngine;
using Calcatz.CookieCutter;

namespace Calcatz.Sequine {

    [CustomCommandNodeDrawer(typeof(SequineFlowCommand))]
    public class SequineFlowCommandNode : BlankCommandNode {

        private static readonly string[] reservedNodeNames = new string[] { "None", "Start" /*, "Alternate Start"*/ };
        public override string[] ReservedNodeNames => reservedNodeNames;

        public SequineFlowCommandNode(CommandData _commandData, Command _command, Vector2 position, Config config)
            : base(_commandData, _command, position, config) {
        }

    }
}