using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// A simple asset which the sole purpose is to act as a binder asset.
    /// </summary>
#if SEQUINE
    [CreateAssetMenu(fileName = "Binder_New", menuName = "Sequine/Cross Scene Binder Asset")]
#else
    [CreateAssetMenu(fileName = "Binder_New", menuName = "CookieCutter/Cross Scene Binder Asset")]
#endif
    public class CrossSceneBinderAsset : ScriptableObject {
        


    }
}
