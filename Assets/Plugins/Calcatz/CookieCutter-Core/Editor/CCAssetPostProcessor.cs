using UnityEngine;
using UnityEditor;
using Calcatz.CookieCutter;
using System.Collections.Generic;
using System.Diagnostics;

namespace Calcatz.CookieCutter {
    class CCAssetPostProcessor : AssetPostprocessor {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {

            List<Object> assets = null;

            foreach (string str in importedAssets) {
                ScriptableObjectWithGUID reimportedAsset = AssetDatabase.LoadAssetAtPath<ScriptableObjectWithGUID>(str);
                if (reimportedAsset != null) {
                    bool dirty = false;
                    if (assets == null) {
                        assets = CCAssetUtility.FindAssetsByType(reimportedAsset.GetType());
                    }

                    foreach (var assetObject in assets) {
                        if (assetObject == reimportedAsset) continue;
                        var asset = (ScriptableObjectWithGUID)assetObject;
                        //Ensure unique GUID
                        if (asset.guid == reimportedAsset.guid) {
                            reimportedAsset.guid = System.Guid.NewGuid().ToString();
                            EditorUtility.SetDirty(reimportedAsset);
                            dirty = true;
                        }
                    }
                    reimportedAsset.ValidateObject();
                    if (dirty) {
                        AssetDatabase.SaveAssets();
                    }
                }

            }

            /*foreach (string str in deletedAssets) {
                Debug.Log("Deleted Asset: " + str);
            }

            for (int i = 0; i < movedAssets.Length; i++) {
                Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
            }*/
        }
    }
}