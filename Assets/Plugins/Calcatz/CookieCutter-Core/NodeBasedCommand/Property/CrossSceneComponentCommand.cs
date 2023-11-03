using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Gets a Cross Scene Component, binded by a Cross Scene Binder.
    /// </summary>
    [System.Serializable]
    public class CrossSceneComponentCommand : PropertyCommand {

#if UNITY_EDITOR
        public bool showTransform;
#endif

        public UnityEngine.Object defaultAsset;

        public CrossSceneComponentCommand() {
            inputIds.Add(new ConnectionTarget());

            nextIds.Add(new List<ConnectionTarget>());  //position
            nextIds.Add(new List<ConnectionTarget>());  //rotation
            nextIds.Add(new List<ConnectionTarget>());  //local scale
        }

        private static readonly Dictionary<int, System.Func<UnityEngine.Object, object>> handlers = new Dictionary<int, System.Func<UnityEngine.Object, object>>() {
            {0, _object => {
                return _object;
            } },

            //Transform

            {1, _object => {
                if (_object is Component comp) {
                    return comp.transform.position;
                }
                else if (_object is GameObject go) {
                    return go.transform.position;
                }
                else {
                    Debug.LogError("Can't get Transform from object: " + _object);
                    return null;
                }
            } },
            {2, _object => {
                if (_object is Component comp) {
                    return comp.transform.rotation.eulerAngles;
                }
                else if (_object is GameObject go) {
                    return go.transform.rotation.eulerAngles;
                }
                else {
                    Debug.LogError("Can't get Transform from object: " + _object);
                    return null;
                }
            } },
            {3, _object => {
                if (_object is Component comp) {
                    return comp.transform.localScale;
                }
                else if (_object is GameObject go) {
                    return go.transform.localScale;
                }
                else {
                    Debug.LogError("Can't get Transform from object: " + _object);
                    return null;
                }
            } }
        };

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            UnityEngine.Object assetInput;
            if (inputIds[0].targetId == 0) {
                assetInput = defaultAsset;
            }
            else {
                assetInput = GetInput<UnityEngine.Object>(_flow, 0);
            }
            return (T)handlers[_pointIndex](CrossSceneBindingUtility.GetObject(assetInput));
        }

    }
}
