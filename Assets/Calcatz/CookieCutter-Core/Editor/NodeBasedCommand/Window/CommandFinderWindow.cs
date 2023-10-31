using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using UnityEditor.UIElements;
#if TIMELINE_AVAILABLE
using UnityEngine.Timeline;
using UnityEngine.Playables;
#endif
using System.Reflection;
using System.Linq;
#if ODIN_INSPECTOR
using Sirenix.Serialization;
using Sirenix.OdinInspector;
#else
using Calcatz.OdinSerializer;
#endif

namespace Calcatz.CookieCutter {

    public class FoundCommandItem {
        public CommandData commandData;
        public int commandId;
        /// <summary>
        /// Optional
        /// </summary>
        public object[] arguments;

        public FoundCommandItem() { }
        public FoundCommandItem(CommandData _commandData, int _commandId) {
            commandData = _commandData;
            commandId = _commandId;
        }

        /// <summary>
        /// Get the first argument available.
        /// </summary>
        public object argument {
            get {
                if (arguments != null && arguments.Length > 0) return arguments[0];
                return null;
            }
        }
    }

#if TIMELINE_AVAILABLE
    public class TimelineFoundCommandItem : FoundCommandItem {
        public TimelineAsset timeline;
        //commandDataObject is Clip
    }
#endif

    public class CommandDataListToSearch {
        public class CommandDataArgumentPair {
            public CommandData commandData;
            public object[] arguments;
        }
        private List<CommandDataArgumentPair> m_commandDataList = new List<CommandDataArgumentPair>();
        public List<CommandDataArgumentPair> GetList() { return m_commandDataList; }
        public void AddCommandData(CommandData _commandData, params object[] _arguments) {
            if (_commandData == null) return;
            m_commandDataList.Add(new CommandDataArgumentPair() {
                commandData = _commandData,
                arguments = _arguments
            });
        }
    }

    public class CommandFinderWindow : EditorWindow {

        [MenuItem("Window/Calcatz/CookieCutter/Command Finder")]
        private static CommandFinderWindow OpenWindow() {
            var window = GetWindow<CommandFinderWindow>();
            window.titleContent.text = "Command Finder Window";
            window.Show();
            return window;
        }

        [SerializeField] private MonoScript commandType;
        [SerializeField] private UnityEngine.Object objectReference;

        private ObjectField commandTypeField;
        private ObjectField objectReferenceField;
        private ListView foundList;

        public static void FindAssetsByCommandType(MonoScript _commandScript) {
            var window = OpenWindow();
            window.commandTypeField.value = _commandScript;
            window.commandType = _commandScript;
            if (window.commandType != null) {
                window.OnClickFindInAssetsByCommandType();
            }
        }

#region GUI
        private void CreateGUI() {
            TypeCache.TypeCollection customCommandFinders = TypeCache.GetTypesDerivedFrom<CustomCommandFinder>();

            VisualElement root = rootVisualElement;

            var foldoutByCommandType = new Foldout() {
                text = "Find By Command Type"
            };
            root.Add(foldoutByCommandType);
            {
                CreateGUIFindByCommandType(customCommandFinders, foldoutByCommandType);

            }

            var foldoutByObjectReference = new Foldout() {
                text = "Find By Object Reference"
            };
            root.Add(foldoutByObjectReference);
            {
                CreateGUIFindByObjectReference(customCommandFinders, foldoutByObjectReference);
            }

            var emptySpace = new VisualElement() {
                style = {
                    flexDirection = FlexDirection.Column,
                    height = new StyleLength(new Length(10, LengthUnit.Pixel)),
                    borderBottomColor = new Color(0.5f, 0.5f, 0.5f, 0.8f),
                    borderBottomWidth = new StyleFloat(1)
                }
            };
            root.Add(emptySpace);
        }

        private void CreateGUIFindByCommandType(TypeCache.TypeCollection customCommandFinders, Foldout foldoutByCommandType) {
            List<Button> customButtons = new List<Button>();
            foreach (var customFinderType in customCommandFinders) {
                var tempInstance = Activator.CreateInstance(customFinderType) as CustomCommandFinder;
                string findButtonText;
                CommandDataListToSearch commandDataListToSearch;
                Action<FoundCommandItem> onClickFoundItem;
                tempInstance.FindInSpecifiedCommandDataList(out findButtonText, out commandDataListToSearch, out onClickFoundItem);
                Button customFindButton = new Button(() => {
                    OnClickFindCustomByCommandType(commandDataListToSearch, onClickFoundItem);
                }) {
                    text = findButtonText
                };
                if (commandType == null) customFindButton.SetEnabled(false);
                customButtons.Add(customFindButton);
            }

            Button findInAssetsButton = new Button(OnClickFindInAssetsByCommandType) {
                text = "Find in Assets"
            };
            if (commandType == null) findInAssetsButton.SetEnabled(false);

            Button findInSceneButton = new Button(OnClickFindInSceneByCommandType) {
                text = "Find in Opened Scenes"
            };
            if (commandType == null) findInSceneButton.SetEnabled(false);

            commandTypeField = new ObjectField("Command Type") {
                objectType = typeof(MonoScript),
                value = commandType
            };
            commandTypeField.RegisterValueChangedCallback(_e => {
                Undo.RecordObject(this, "change command type");
                commandType = _e.newValue as MonoScript;
                findInAssetsButton.SetEnabled(commandType != null);
                findInSceneButton.SetEnabled(commandType != null);
                foreach (var button in customButtons) {
                    button.SetEnabled(commandType != null);
                }
            });
            foldoutByCommandType.Add(commandTypeField);
            foldoutByCommandType.Add(findInAssetsButton);
            foldoutByCommandType.Add(findInSceneButton);
            foreach (var button in customButtons) {
                foldoutByCommandType.Add(button);
            }
        }

        private void CreateGUIFindByObjectReference(TypeCache.TypeCollection customCommandFinders, Foldout foldoutByObjectReference) {
            List<Button> customButtons = new List<Button>();
            foreach (var customFinderType in customCommandFinders) {
                var tempInstance = Activator.CreateInstance(customFinderType) as CustomCommandFinder;
                string findButtonText;
                CommandDataListToSearch commandDataListToSearch;
                Action<FoundCommandItem> onClickFoundItem;
                tempInstance.FindInSpecifiedCommandDataList(out findButtonText, out commandDataListToSearch, out onClickFoundItem);
                Button customFindButton = new Button(() => {
                    OnClickFindCustomByObjectReference(commandDataListToSearch, onClickFoundItem);
                }) {
                    text = findButtonText
                };
                if (ReferenceEquals(objectReference, null)) customFindButton.SetEnabled(false);
                customButtons.Add(customFindButton);
            }

            Button findInAssetsButton = new Button(OnClickFindInAssetsByObjectReference) {
                text = "Find in Assets"
            };
            if (ReferenceEquals(objectReference, null)) findInAssetsButton.SetEnabled(false);

            Button findInSceneButton = new Button(OnClickFindInSceneByObjectReference) {
                text = "Find in Opened Scenes"
            };
            if (ReferenceEquals(objectReference, null)) findInSceneButton.SetEnabled(false);

            objectReferenceField = new ObjectField("Object") {
                objectType = typeof(UnityEngine.Object),
                value = objectReference
            };
            objectReferenceField.RegisterValueChangedCallback(_e => {
                Undo.RecordObject(this, "change object reference");
                objectReference = _e.newValue;
                findInAssetsButton.SetEnabled(objectReference != null);
                findInSceneButton.SetEnabled(objectReference != null);
                foreach(var button in customButtons) {
                    button.SetEnabled(objectReference != null);
                }
            });
            foldoutByObjectReference.Add(objectReferenceField);
            foldoutByObjectReference.Add(findInAssetsButton);
            foldoutByObjectReference.Add(findInSceneButton);
            foreach (var button in customButtons) {
                foldoutByObjectReference.Add(button);
            }
        }
#endregion

#region CALLBACKS
        private void OnClickFindInAssetsByCommandType() {
            VisualElement root = rootVisualElement;
            var commandDataAssets = CCAssetUtility.FindAssetsByType<ScriptableObjectCommandData>().ToArray();
#if TIMELINE_AVAILABLE
            var timelineAssets = CCAssetUtility.FindAssetsByType<TimelineAsset>().ToArray();
#endif
            List<FoundCommandItem> foundCommands = new List<FoundCommandItem>();

            int progress = 1;
#if TIMELINE_AVAILABLE
            int count = commandDataAssets.Length + timelineAssets.Length;
#else
            int count = commandDataAssets.Length;
#endif
            foreach (var asset in commandDataAssets) {
                EditorUtility.DisplayProgressBar("Searching Commands in Assets", asset.name + " (" + progress + "/" + count + ")", (float)progress / (float)count);
                try {
                    if (asset.commandData == null) continue;
                    asset.commandData.TraverseCommands(_command => {
                        if (_command.GetType() == commandType.GetClass()) {
                            foundCommands.Add(new FoundCommandItem() {
                                commandData = asset.commandData,
                                commandId = _command.id
                            });
                        }
                    });
                }
                catch {
                    Debug.LogError("Error on processing asset: " + asset.name, asset);
                    continue;
                }
                progress++;
            }

#if TIMELINE_AVAILABLE
            foreach (var timeline in timelineAssets) {
                EditorUtility.DisplayProgressBar("Searching Commands in Assets", timeline.name + " (" + progress + "/" + count + ")", (float)progress / (float)count);
                try {
                    foreach(TrackAsset track in timeline.GetOutputTracks()) {

                        if (track is TimelineCommandTrack commandTrack) {
                            foreach(TimelineClip clip in commandTrack.GetClips()) {

                                if (clip.asset is TimelineCommandDataClip commandDataClip) {
                                    commandDataClip.commandData.TraverseCommands(_command => {
                                        if (_command.GetType() == commandType.GetClass()) {
                                            foundCommands.Add(new TimelineFoundCommandItem() {
                                                timeline = timeline,
                                                commandData = commandDataClip.commandData,
                                                commandId = _command.id
                                            });
                                        }
                                    });
                                }
                            }
                        }
                    }
                }
                catch {
                    Debug.LogError("Error on processing timeline: " + timeline.name, timeline);
                }
                progress++;
            }
#endif
            EditorUtility.ClearProgressBar();

            if (foundList != null) {
                foundList.parent.Remove(foundList);
                foundList.Clear();
            }

            root.Add(CreateResultList(foundCommands, OnAssetItemChoosen));
        }

        private void OnClickFindInSceneByCommandType() {
            VisualElement root = rootVisualElement;
#if UNITY_2020_OR_NEWER
            var commandDataComponents = FindObjectsOfType<MonoBehaviourCommandData>(true);
#else
            var commandDataComponents = FindObjectsOfType<MonoBehaviourCommandData>();
#endif
            List<FoundCommandItem> foundCommands = new List<FoundCommandItem>();

            int progress = 1;
            foreach (var component in commandDataComponents) {
                EditorUtility.DisplayProgressBar("Searching Commands in Scene", component.name + " (" + progress + "/" + commandDataComponents.Length + ")", (float)progress / (float)commandDataComponents.Length);
                try {
                    if (component.commandData == null) continue;
                    component.commandData.TraverseCommands(_command => {
                        if (_command.GetType() == commandType.GetClass()) {
                            foundCommands.Add(new FoundCommandItem() {
                                commandData = component.commandData,
                                commandId = _command.id
                            });
                        }
                    });
                }
                catch {
                    Debug.LogError("Error on processing component: " + component.name, component);
                    continue;
                }
                progress++;
            }
            EditorUtility.ClearProgressBar();

            if (foundList != null) {
                foundList.parent.Remove(foundList);
                foundList.Clear();
            }

            root.Add(CreateResultList(foundCommands, OnUnityObjectItemChoosen));
        }

        private void OnClickFindCustomByCommandType(CommandDataListToSearch _commandDataList, Action<FoundCommandItem> _onClickResultItem) {
            VisualElement root = rootVisualElement;
            List<FoundCommandItem> foundCommands = new List<FoundCommandItem>();

            int progress = 1;
            int count = _commandDataList.GetList().Count;
            foreach (var commandDataPair in _commandDataList.GetList()) {
                EditorUtility.DisplayProgressBar("Searching Commands", commandDataPair.commandData.targetObject.name + " (" + progress + "/" + count + ")", (float)progress / (float)count);
                try {
                    if (commandDataPair.commandData == null || ReferenceEquals(commandDataPair.commandData.targetObject, null)) continue;
                    commandDataPair.commandData.TraverseCommands(_command => {
                        if (_command.GetType() == commandType.GetClass()) {
                            foundCommands.Add(new FoundCommandItem() {
                                commandData = commandDataPair.commandData,
                                arguments = commandDataPair.arguments,
                                commandId = _command.id
                            });
                        }
                    });
                }
                catch {
                    Debug.LogError("Error on processing object: " + commandDataPair.commandData.targetObject.name, commandDataPair.commandData.targetObject);
                    continue;
                }
                progress++;
            }
            EditorUtility.ClearProgressBar();

            if (foundList != null) {
                foundList.parent.Remove(foundList);
                foundList.Clear();
            }

            root.Add(CreateResultList(foundCommands, _onClickResultItem != null ? _onClickResultItem : OnUnityObjectItemChoosen));
        }

        private void OnClickFindInAssetsByObjectReference() {
            VisualElement root = rootVisualElement;
            var commandDataAssets = CCAssetUtility.FindAssetsByType<ScriptableObjectCommandData>().ToArray();
#if TIMELINE_AVAILABLE
            var timelineAssets = CCAssetUtility.FindAssetsByType<TimelineAsset>().ToArray();
#endif
            List<FoundCommandItem> foundCommands = new List<FoundCommandItem>();

            int progress = 1;
#if TIMELINE_AVAILABLE
            int count = commandDataAssets.Length + timelineAssets.Length;
#else
            int count = commandDataAssets.Length;
#endif
            foreach (var asset in commandDataAssets) {
                EditorUtility.DisplayProgressBar("Searching Commands In Assets", asset.name + " (" + progress + "/" + count + ")", (float)progress / (float)count);
                try {
                    if (asset.commandData == null) continue;
                    FindObjectInCommandData(asset.commandData, _commandId => {
                        foundCommands.Add(new FoundCommandItem() {
                            commandData = asset.commandData,
                            commandId = _commandId
                        });
                    });
                }
                catch {
                    Debug.LogError("Error on processing asset: " + asset.name, asset);
                    continue;
                }
                progress++;
            }

#if TIMELINE_AVAILABLE
            foreach (var timeline in timelineAssets) {
                EditorUtility.DisplayProgressBar("Searching Commands In Assets", timeline.name + " (" + progress + "/" + count + ")", (float)progress / (float)count);
                try {
                    foreach (TrackAsset track in timeline.GetOutputTracks()) {

                        if (track is TimelineCommandTrack commandTrack) {
                            foreach (TimelineClip clip in commandTrack.GetClips()) {

                                if (clip.asset is TimelineCommandDataClip commandDataClip) {

                                    FindObjectInCommandData(commandDataClip.commandData, _commandId => {
                                        foundCommands.Add(new TimelineFoundCommandItem() {
                                            timeline = timeline,
                                            commandData = commandDataClip.commandData,
                                            commandId = _commandId
                                        });
                                    });

                                }
                            }
                        }
                    }
                }
                catch {
                    Debug.LogError("Error on processing timeline: " + timeline.name, timeline);
                }
                progress++;
            }
#endif
            EditorUtility.ClearProgressBar();

            if (foundList != null) {
                foundList.parent.Remove(foundList);
                foundList.Clear();
            }

            root.Add(CreateResultList(foundCommands, OnAssetItemChoosen));
        }
        
        private void OnClickFindInSceneByObjectReference() {
            VisualElement root = rootVisualElement;
#if UNITY_2020_OR_NEWER
            var commandDataComponents = FindObjectsOfType<MonoBehaviourCommandData>(true);
#else
            var commandDataComponents = FindObjectsOfType<MonoBehaviourCommandData>();
#endif
            List<FoundCommandItem> foundCommands = new List<FoundCommandItem>();

            int progress = 1;
            foreach (var component in commandDataComponents) {
                EditorUtility.DisplayProgressBar("Searching Commands In Scene", component.name + " (" + progress + "/" + commandDataComponents.Length + ")", (float)progress / (float)commandDataComponents.Length);
                try {
                    if (component.commandData == null) continue;
                    FindObjectInCommandData(component.commandData, _commandId => {
                        foundCommands.Add(new FoundCommandItem() {
                            commandData = component.commandData,
                            commandId = _commandId
                        });
                    });
                }
                catch {
                    Debug.LogError("Error on processing component: " + component.name, component);
                    continue;
                }
                progress++;
            }
            EditorUtility.ClearProgressBar();

            if (foundList != null) {
                foundList.parent.Remove(foundList);
                foundList.Clear();
            }

            root.Add(CreateResultList(foundCommands, OnUnityObjectItemChoosen));
        }

        private void OnClickFindCustomByObjectReference(CommandDataListToSearch _commandDataList, Action<FoundCommandItem> _onClickResultItem) {
            VisualElement root = rootVisualElement;
            List<FoundCommandItem> foundCommands = new List<FoundCommandItem>();

            int progress = 1;
            int count = _commandDataList.GetList().Count;
            foreach (var commandDataPair in _commandDataList.GetList()) {
                EditorUtility.DisplayProgressBar("Searching Commands", commandDataPair.commandData.targetObject.name + " (" + progress + "/" + count + ")", (float)progress / (float)count);
                try {
                    if (commandDataPair.commandData == null || ReferenceEquals(commandDataPair.commandData.targetObject, null)) continue;
                    FindObjectInCommandData(commandDataPair.commandData, _commandId => {
                        foundCommands.Add(new FoundCommandItem() {
                            commandData = commandDataPair.commandData,
                            arguments = commandDataPair.arguments,
                            commandId = _commandId
                        });
                    });
                }
                catch {
                    Debug.LogError("Error on processing object: " + commandDataPair.commandData.targetObject.name, commandDataPair.commandData.targetObject);
                    continue;
                }
                progress++;
            }
            EditorUtility.ClearProgressBar();

            if (foundList != null) {
                foundList.parent.Remove(foundList);
                foundList.Clear();
            }

            root.Add(CreateResultList(foundCommands, _onClickResultItem != null? _onClickResultItem : OnUnityObjectItemChoosen));
        }

        private void FindObjectInCommandData(CommandData _commandData, Action<int> _onCommandFound) {
            _commandData.TraverseCommands(_command => {
                if (_command is BuildableCommand buildableCommand) {
                    foreach (var inputValue in buildableCommand.io.inputValues) {
                        if (inputValue is UnityEngine.Object inputUnityValue) {
                            if (inputUnityValue == objectReference) {
                                _onCommandFound.Invoke(_command.id);
                            }
                        }
                    }
                }
                else if (_command is BuildablePropertyCommand buildablePropertyCommand) {
                    foreach (var inputValue in buildablePropertyCommand.io.inputValues) {
                        if (inputValue is UnityEngine.Object inputUnityValue) {
                            if (inputUnityValue == objectReference) {
                                _onCommandFound.Invoke(_command.id);
                            }
                        }
                    }
                }
                else {
                    FieldInfo[] fields = GetFieldInfosIncludingBaseClasses(_command.GetType(), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    foreach (var field in fields) {
                        if (!field.FieldType.IsValueType) {
                            object fieldObj = field.GetValue(_command);
                            if (fieldObj is UnityEngine.Object fieldUnityObj) {
                                if (fieldUnityObj.Equals(objectReference)) {
                                    _onCommandFound.Invoke(_command.id);
                                }
                            }
                        }
                    }
                }
            });
        }
#endregion

#region RESULT-HANDLER
        private ListView CreateResultList(List<FoundCommandItem> _assetValidations, Action<FoundCommandItem> _onItemChoosen) {

            Action<VisualElement, int> bindItem = (e, i) => {
                (e.ElementAt(0) as ObjectField).value = _assetValidations[i].commandData.targetObject is UnityEngine.Object? _assetValidations[i].commandData.targetObject : null;
                (e.ElementAt(0) as ObjectField).SetEnabled(false);
                (e.ElementAt(1) as Label).text = "Command ID: " + _assetValidations[i].commandId;

#if TIMELINE_AVAILABLE
                if (_assetValidations[i] is TimelineFoundCommandItem timelineValidation) {
                    (e.ElementAt(0) as ObjectField).style.flexGrow = 0.5f;
                    (e.ElementAt(0) as ObjectField).style.flexShrink = 0.5f;
                    e.Insert(0, new ObjectField() {
                        value = timelineValidation.timeline,
                        style = {
                            marginTop = new StyleLength(new Length(2, LengthUnit.Pixel)),
                            flexGrow = 0.5f,
                            flexShrink = 0.5f
                        }
                    });
                    //e.ElementAt(0).SetEnabled(false);
                }
#endif
            };

            foundList = new ListView(_assetValidations, 25, MakeResultItem, bindItem) {
                name = "found-list",
                style = {
                    //marginTop = new StyleLength(new Length(CommandNode.verticalSpacing, LengthUnit.Pixel)),
                    flexGrow = 1f,
                    flexShrink = 0,
                    flexBasis = 0,
                    //backgroundColor = new StyleColor(NodesContainer.backgroundColor * 0.5f)
                }
            };

            foundList.selectionType = SelectionType.Single;
            //commandList.onItemChosen += obj => GoToNode((CommandNode)obj);
#if UNITY_2022_2_OR_NEWER
            foundList.itemsChosen += _obj => {
                foreach (var elm in _obj) {
                    _onItemChoosen(elm as FoundCommandItem);
                }
            };
#elif UNITY_2020_1_OR_NEWER
            foundList.onItemsChosen += _obj => {
                foreach(var elm in _obj) {
                    _onItemChoosen(elm as FoundCommandItem);
                }
            };
#else
            foundList.onItemChosen += _obj => {
                _onItemChoosen(_obj as FoundCommandItem);
            };
#endif

            return foundList;
        }

        private static VisualElement MakeResultItem() {
            var box = new VisualElement() {
                style = {
                    flexDirection = FlexDirection.Row,
                    flexGrow = 1f,
                    flexShrink = 0,
                    flexBasis = 0,
                    left = new StyleLength(new Length(5, LengthUnit.Pixel)),
                    right = new StyleLength(new Length(5, LengthUnit.Pixel)),
                    borderBottomColor = new Color(0.5f, 0.5f, 0.5f, 0.8f),
                    borderBottomWidth = new StyleFloat(1)
                }
            };
            box.Add(new ObjectField() {
                style = {
                    marginTop = new StyleLength(new Length(2, LengthUnit.Pixel)),
                    flexGrow = 1f
                }
            });
            box.Add(new Label() {
                style = {
                    //flexGrow = 0.5f,
                    width = new StyleLength(new Length(110, LengthUnit.Pixel)),
                    marginTop = new StyleLength(new Length(4, LengthUnit.Pixel))
                }
            });
            return box;
        }

        private static FieldInfo[] GetFieldInfosIncludingBaseClasses(Type type, BindingFlags bindingFlags) {
            FieldInfo[] fieldInfos = type.GetFields(bindingFlags);

            // If this class doesn't have a base, don't waste any time
            if (type.BaseType == typeof(object)) {
                return fieldInfos;
            }
            else {   // Otherwise, collect all types up to the furthest base class
                var currentType = type;
                var fieldComparer = new FieldInfoComparer();
                var fieldInfoList = new HashSet<FieldInfo>(fieldInfos, fieldComparer);
                while (currentType != typeof(object)) {
                    fieldInfos = currentType.GetFields(bindingFlags);
                    fieldInfoList.UnionWith(fieldInfos);
                    currentType = currentType.BaseType;
                }
                return fieldInfoList.ToArray();
            }
        }

        private class FieldInfoComparer : IEqualityComparer<FieldInfo> {
            public bool Equals(FieldInfo x, FieldInfo y) {
                return x.DeclaringType == y.DeclaringType && x.Name == y.Name;
            }

            public int GetHashCode(FieldInfo obj) {
                return obj.Name.GetHashCode() ^ obj.DeclaringType.GetHashCode();
            }
        }


        private void OnAssetItemChoosen(FoundCommandItem _foundCommandItem) {
#if TIMELINE_AVAILABLE
            if (_foundCommandItem is TimelineFoundCommandItem timelineResultItem) {
                Selection.activeObject = timelineResultItem.timeline;
                Selection.activeObject = timelineResultItem.commandData.targetObject as PlayableAsset;
                var nodesWindow = GetWindow<CommandInspectorWindow>();
                CommandInspectorWindow.OpenWindow();
                nodesWindow.GoToNode(timelineResultItem.commandId);
#else
            if (false) {
#endif
            }
            else {
                AssetDatabase.OpenAsset((_foundCommandItem.commandData.targetObject as UnityEngine.Object).GetInstanceID());
                if (EditorWindow.focusedWindow is CommandNodesWindow nodesWindow) {
                    nodesWindow.GoToNode(_foundCommandItem.commandId);
                }
            }
        }

        private void OnUnityObjectItemChoosen(FoundCommandItem _foundCommandItem) {
            Selection.activeObject = _foundCommandItem.commandData.targetObject;
            var nodesWindow = GetWindow<CommandInspectorWindow>();
            CommandInspectorWindow.OpenWindow();
            nodesWindow.GoToNode(_foundCommandItem.commandId);
        }
#endregion
    
    }

}