using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Calcatz.CookieCutter {
 
    public class NodeBasedEditor<T> : EditorWindow where T : NodesContainer{

        [SerializeField] private Vector2 lastRealPositionOffset = Vector2.zero;
        [SerializeField] private float lastZoomScale = 1f;

        protected T nodesContainer;

        protected virtual Rect GetContainerArea() {
            return new Rect(0, 0, position.width, position.height);
        }

        protected virtual void OnEnable() {
            EnsureInitNodesContainer();
        }

        protected virtual void OnDisable() {
            if (nodesContainer != null) {
                lastRealPositionOffset = nodesContainer.RealPositionOffset;
                lastZoomScale = nodesContainer.ZoomScale;
            }
        }

        private void CreateGUI() {
            VisualElement root = rootVisualElement;
            EnsureInitNodesContainer();
            nodesContainer.CreateGUI(root, GetContainerArea);
        }

        private void EnsureInitNodesContainer() {
            if (nodesContainer == null) {
                nodesContainer = CreateNodesContainer();
                nodesContainer.RealPositionOffset = lastRealPositionOffset;
                nodesContainer.ZoomScale = lastZoomScale;
                OnNodesContainerCreated();
            }
        }

        protected virtual T CreateNodesContainer() {
            return (T)new NodesContainer();
        }

        protected virtual void OnNodesContainerCreated() {

        }

    }
}
