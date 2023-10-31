using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Calcatz.CookieCutter {
    public class CCAssetUtility {

        public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++) {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null) {
                    assets.Add(asset);
                }
            }
            return assets;
        }

        public static List<UnityEngine.Object> FindAssetsByType(System.Type _type) {
            List<UnityEngine.Object> assets = new List<UnityEngine.Object>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", _type));
            for (int i = 0; i < guids.Length; i++) {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                if (asset != null) {
                    assets.Add(asset);
                }
            }
            return assets;
        }

        public static List<UnityEngine.Object> FindAssetsDerivedFrom<T>() {
            List<Object> objects = new List<Object>();
            var types = TypeCache.GetTypesDerivedFrom<T>();
            foreach (var type in types) {
                List<Object> objects2 = FindAssetsByType(type);
                foreach(var obj in objects2) {
                    if (!objects.Contains(obj)) {
                        objects.Add(obj);
                    }
                }
            }
            return objects;
        }

        public static MonoScript[] GetScriptAssetsOfType<T>() {
            MonoScript[] scripts = (MonoScript[])Resources.FindObjectsOfTypeAll(typeof(MonoScript));

            List<MonoScript> result = new List<MonoScript>();

            foreach (MonoScript m in scripts) {
                var classType = m.GetClass();
                if (classType != null && classType != typeof(Shader) && classType.IsSubclassOf(typeof(T))) {
                    result.Add(m);
                }
            }
            return result.ToArray();
        }


        public static T CreateAssetAtSceneFolder<T>(Scene _scene, string _folderSuffix, string _fileName) where T : ScriptableObject {
            string path;

            if (string.IsNullOrEmpty(_scene.path)) {
                path = "Assets/";
            }
            else {
                var scenePath = System.IO.Path.GetDirectoryName(_scene.path);
                var extPath = _scene.name + "_" + _folderSuffix;
                var bindersPath = scenePath + "/" + extPath;

                if (!AssetDatabase.IsValidFolder(bindersPath))
                    AssetDatabase.CreateFolder(scenePath, extPath);

                path = bindersPath + "/";
            }

            path += _fileName + ".asset";
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return asset;
        }

    }
}
