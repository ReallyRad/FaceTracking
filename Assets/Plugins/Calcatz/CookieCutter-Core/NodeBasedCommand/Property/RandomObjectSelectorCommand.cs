using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Gets 1 random object from the specified list of objects.
    /// </summary>
    [System.Serializable]
    public class RandomObjectSelectorCommand : PropertyCommand {

        public List<UnityEngine.Object> objectList = new List<Object>();

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            int randomIndex = Random.Range(0, objectList.Count);
            if (objectList[randomIndex] is T validObject) {
                return validObject;
            }
            else {
                Debug.LogError("Get Random Object: " + objectList[randomIndex] + " is not of type " + typeof(T).Name);
                return default(T);
            }
        }

#if UNITY_EDITOR
        public System.Type typeToSelect = typeof(AnimationClip);
#endif
    }

}