using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using Object = UnityEngine.Object;

namespace Calcatz.CookieCutter {
    public class UnusedAssetsFinder : EditorWindow {

        private static Object[] GetAssetsAtDirectory<T>(string _directory) {
            string dataPath = Application.dataPath;
            dataPath = dataPath.Substring(0, dataPath.Length - 7); //Remove "Assets" at the end
            List<Object> assets = new List<Object>();
            string[] paths = Directory.GetFiles(dataPath + "/" + _directory, "*", SearchOption.AllDirectories);
            foreach (string path in paths) {
                string assetPath = path.Replace(dataPath + "/", "");
                if (assetPath.Contains(".meta")) {
                    continue;
                }
                Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
                if (asset != null) {
                    assets.Add(asset);
                }
            }
            return assets.ToArray();
        }

        private static bool FindReferencesTo(GameObject[] allObjects, Object to, int assetIndex, int assetCount) {
            List<Shader> shaders = new List<Shader>();
            List<VisualEffectAsset> vfxs = new List<VisualEffectAsset>();

            //var referencedBy = new List<Object>();
            for (int j = 0; j < allObjects.Length; j++) {
                EditorUtility.DisplayProgressBar("Searching Unused Assets", "Asset " + assetIndex + "/" + assetCount + " - Scene Object " + (j+1) + "/" + allObjects.Length, assetIndex / (float)assetCount);
                var go = allObjects[j];

                if (PrefabUtility.GetPrefabInstanceStatus(go) == PrefabInstanceStatus.Connected) {
                    if (PrefabUtility.GetCorrespondingObjectFromSource(go) == to) {
                        //Debug.Log(string.Format("referenced by {0}, {1}", go.name, go.GetType()), go);
                        //referencedBy.Add(go);
                        return true;
                    }
                }

                var components = go.GetComponents<Component>();
                for (int i = 0; i < components.Length; i++) {
                    var c = components[i];
                    if (!c) continue;

                    if (c is Renderer renderer) {
                        foreach(var mat in renderer.sharedMaterials) {
                            if (mat == null) {
                                Debug.Log("Material null in: " + renderer.name, renderer);
                                continue;
                            }
                            if (!shaders.Contains(mat.shader)) {
                                shaders.Add(mat.shader);
                            }
                        }
                    }
                    else if (c is VisualEffect visualEffect) {
                        if (!vfxs.Contains(visualEffect.visualEffectAsset)) {
                            vfxs.Add(visualEffect.visualEffectAsset);
                        }
                    }

                    var so = new SerializedObject(c);
                    var sp = so.GetIterator();

                    while (sp.NextVisible(true)) {
                        if (sp.propertyType == SerializedPropertyType.ObjectReference) {
                            if (sp.objectReferenceValue == to) {
                                //Debug.Log(string.Format("referenced by {0}, {1}", c.name, c.GetType()), c);
                                //referencedBy.Add(c.gameObject);
                                return true;
                            }
                        }
                    }

                }
            }


            List<Object> serializableObjects = new List<Object>();
            serializableObjects.AddRange(shaders);
            serializableObjects.AddRange(vfxs);

            foreach(var shader in serializableObjects) {
                var so = new SerializedObject(shader);
                var sp = so.GetIterator();

                while (sp.NextVisible(true)) {
                    if (sp.propertyType == SerializedPropertyType.ObjectReference) {
                        if (sp.objectReferenceValue == to) {
                            //Debug.Log(string.Format("referenced by {0}, {1}", c.name, c.GetType()), c);
                            //referencedBy.Add(shader);
                            return true;
                        }
                    }
                }
            }

            //if (referencedBy.Any()) {
                //Selection.objects = referencedBy.ToArray();
                //return true;
            //}
            //else {
                //Debug.Log("no references in scene");
                return false;
            //}
        }

        private static string GetSelectedDirectory() {
            var path = "";
            var obj = Selection.activeObject;
            if (obj == null) path = "Assets";
            else path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            if (path.Length > 0) {
                if (Directory.Exists(path)) {
                    //Debug.Log("Folder");
                }
                else {
                    path = path.Remove(path.LastIndexOf('/'));
                }
            }
            else {
                //Debug.Log("Not in assets folder");
            }
            return path;
        }

        #region WINDOW
        public class ObjectItem {
            public UnityEngine.Object obj;
            public string message;
        }

        [MenuItem("Window/Calcatz/Unused Assets Finder")]
        private static void OpenWindow() {
            UnusedAssetsFinder window = GetWindow<UnusedAssetsFinder>();
            window.titleContent.text = "Unused Assets Finder";
            window.Show();
        }

        [SerializeField] private List<Object> unusedAssets = new List<Object>();

        private TextField pathField;
        private ListView unusedAssetList;

        private Button deleteButton;

        private void CreateGUI() {
            VisualElement root = rootVisualElement;

            root.Add(new IMGUIContainer(() => {
                EditorGUILayout.HelpBox("This will only search through the current opened scene.", MessageType.Info);
            }) {
                style = {
                    height = new StyleLength(new Length(40, LengthUnit.Pixel))
                }
            });

            pathField = new TextField("Path") {
                value = GetSelectedDirectory(),
            };
            root.Add(pathField);

            root.Add(new Button(() => {
                pathField.value = GetSelectedDirectory();
            }) {
                text = "Try Locate Selected Folder"
            });

            root.Add(new Button(OnClickSearch) {
                text = "Search"
            });

            deleteButton = new Button(() => {
                if (!EditorUtility.DisplayDialog("Confirm Delete Asset", "Are you sure you want to delete all assets in the list?", "Cancel", "Yes")) {
                    foreach (var asset in unusedAssets) {
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(asset));
                    }
                    unusedAssets.Clear();
                    unusedAssetList.Clear();
                }
            }) {
                text = "Delete Assets In List"
            };
            deleteButton.SetEnabled(unusedAssets.Count > 0);
            root.Add(deleteButton);
            
        }

        private void OnClickSearch() {
            VisualElement root = rootVisualElement;
            string path = pathField.value;
            if (path == "") return;
            EditorUtility.DisplayProgressBar("Searching Unused Assets", "Getting all assets at path...", 0);
            Object[] assets = GetAssetsAtDirectory<Object>(path);

            Undo.RecordObject(this, "find unused assets");

            unusedAssets.Clear();

            EditorUtility.DisplayProgressBar("Searching Unused Assets", "Getting all game objects on current scene...", 0);
            var allObjects = Object.FindObjectsOfType<GameObject>();
            int count = assets.Length;
            int index = 0;

            foreach (var asset in assets) {
                index++;
                EditorUtility.DisplayProgressBar("Searching Unused Assets", "Asset " + index + "/" + count, index / (float)count);
                if (!FindReferencesTo(allObjects, asset, index, count)) {
                    bool subAssetFound = false;
                    if (!(asset is SceneAsset)) {
                        Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(asset));
                        foreach (var subAsset in subAssets) {
                            if (FindReferencesTo(allObjects, subAsset, index, count)) {
                                subAssetFound = true;
                                break;
                            }
                        }
                    }

                    if (!subAssetFound) {
                        unusedAssets.Add(asset);
                    }
                }
            }

            EditorUtility.ClearProgressBar();

            if (unusedAssetList != null) {
                unusedAssetList.parent.Remove(unusedAssetList);
                unusedAssetList.Clear();
            }
            root.Add(CreateUnusedAssetList(unusedAssets));

            deleteButton.SetEnabled(unusedAssets.Count > 0);
        }

        private ListView CreateUnusedAssetList(List<Object> _unusedAssets) {

            Action<VisualElement, int> bindItem = (e, i) => {
                (e.ElementAt(0) as Label).text = _unusedAssets[i].name;
            };

            unusedAssetList = new ListView(_unusedAssets, 16, MakeUnusedAssetItem, bindItem) {
                name = "unused-asset-list",
                style = {
                    //marginTop = new StyleLength(new Length(CommandNode.verticalSpacing, LengthUnit.Pixel)),
                    flexGrow = 1f,
                    flexShrink = 0,
                    flexBasis = 0,
                    //backgroundColor = new StyleColor(NodesContainer.backgroundColor * 0.5f)
                }
            };

            unusedAssetList.selectionType = SelectionType.Single;
            //commandList.onItemChosen += obj => GoToNode((CommandNode)obj);
#if UNITY_2022_2_OR_NEWER
            unusedAssetList.itemsChosen += obj => {

#elif UNITY_2021_2_OR_NEWER
            unusedAssetList.onItemsChosen += obj => {
#else
            unusedAssetList.onItemChosen += obj => {
#endif
                var unusedAsset = (UnityEngine.Object)obj;
                Selection.activeObject = unusedAsset;
                EditorGUIUtility.PingObject(unusedAsset);
            };

            return unusedAssetList;
        }

        private static VisualElement MakeUnusedAssetItem() {
            var box = new VisualElement() {
                style = {
                        flexDirection = FlexDirection.Column,
                        flexGrow = 1f,
                        flexShrink = 0,
                        flexBasis = 0
                    }
            };
            box.Add(new Label() {
                style = {
                        left = new StyleLength(new Length(5, LengthUnit.Pixel))
                    }
            });
            box.Add(new Label() {
                style = {
                        left = new StyleLength(new Length(5, LengthUnit.Pixel))
                    }
            });
            return box;
        }
        #endregion
    }
}
