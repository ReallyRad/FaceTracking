using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.Serialization;
using Sirenix.OdinInspector;
#else
using Calcatz.OdinSerializer;
#endif

namespace Calcatz.CookieCutter {

    public abstract class CommandNodesWindow : NodeBasedEditor<CommandNodesContainer> {

        protected override CommandNodesContainer CreateNodesContainer() {
            return new CommandNodesContainer();
        }

        public virtual void GoToNode(int _commandId) {
            nodesContainer.GoToNode(_commandId);
        }

        protected virtual void Update() {
            if (EditorApplication.isPlaying && nodesContainer != null && nodesContainer.commandData != null) {
                if (nodesContainer.commandData.needsRuntimeRepaint) {
                    Repaint();
                    nodesContainer.commandData.needsRuntimeRepaint = false;
                }
            }
        }

        protected virtual void OnFocus() {
            if (nodesContainer != null) {
                nodesContainer.RepaintIfDirty();
            }
        }

    }
}
