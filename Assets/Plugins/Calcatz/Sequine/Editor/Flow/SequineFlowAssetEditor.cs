using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Calcatz.CookieCutter;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Calcatz.Sequine {

    internal class SequineFlowAssetEditor : ScriptableObjectCommandNodesWindow<SequineFlowAsset, SequineFlowAssetEditor> {

        [CustomPreview(typeof(SequineFlowComponent))]
        public class CommandNodesComponentPreview : CommandDataInspectorPreview { }

        [CustomPreview(typeof(SequineFlowAsset))]
        public class CommandNodesAssetPreview : CommandDataInspectorPreview { }

        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line) {
            return HandleOpenAsset<SequineFlowAsset>(instanceID);
        }

        protected override CommandNodesContainer CreateNodesContainer() => new SequineFlowNodesContainer();

        #region DRAW
        protected override void OnCreateBeforeCommandList(VisualElement _leftPane) {
            base.OnCreateBeforeCommandList(_leftPane);
            //SequineFlowAsset asset = (SequineFlowAsset)nodesContainer.commandData.targetObject;
            //Create related GUI if necessary, given the asset
        }
        #endregion
    }

}