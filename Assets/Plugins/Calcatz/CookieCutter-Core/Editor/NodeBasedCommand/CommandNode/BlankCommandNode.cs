using Calcatz.CookieCutter;
using UnityEngine;

namespace Calcatz.CookieCutter {

    [CustomCommandNodeDrawer(typeof(Command))]
    public class BlankCommandNode : CommandNode {

        public BlankCommandNode(CommandData _commandData, Command _command, Vector2 position, Config config)
            : base(_commandData, _command, position, 150, 50, config) {

        }

    }
}
