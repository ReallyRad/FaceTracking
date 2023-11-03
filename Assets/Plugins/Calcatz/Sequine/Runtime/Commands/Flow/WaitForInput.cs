
using Calcatz.CookieCutter;
using System.Threading.Tasks;
using UnityEngine;

namespace Calcatz.Sequine {

    /// <summary>
    /// Waits for an input before proceeding to the next command.
    /// </summary>
    [System.Serializable]
    [RegisterCommand("Flow/Wait For Input", typeof(SequineFlowCommandData))]
    public class WaitForInput : Command {

        public override void Execute(CommandExecutionFlow _flow) {
            Wait();
        }

        private async void Wait() {
            while (true) {
#if UNITY_EDITOR
                if (!Application.isPlaying) {
                    return;
                }
#endif
                if (inputType == InputType.KeyDown) {
                    if (Input.GetKeyDown(keyCode)) {
                        break;
                    }
                }
                else if (inputType == InputType.KeyUp) {
                    if (Input.GetKeyUp(keyCode)) {
                        break;
                    }
                }
                else if (inputType == InputType.MouseButtonDown) {
                    if (Input.GetMouseButtonDown((int)mouseButton)) {
                        break;
                    }
                }
                else {
                    if (Input.GetMouseButtonUp((int)mouseButton)) {
                        break;
                    }
                }
                await Task.Yield();
            }
            Exit();
        }

        #region PROPERTIES
        public override float nodeWidth => 250f;

        public InputType inputType = InputType.KeyDown;
        public KeyCode keyCode = KeyCode.Return;
        public MouseButton mouseButton = MouseButton.LeftClick;

        public enum InputType {
            KeyDown,
            KeyUp,
            MouseButtonDown,
            MouseButtonUp
        }

        public enum MouseButton {
            LeftClick = 0,
            RightClick = 1,
            MiddleClick = 2
        }
        #endregion PROPERTIES

        #region NODE-DRAWER
#if UNITY_EDITOR
        public override void Editor_InitInPoints() {
            CommandGUI.AddMainInPoint();
        }

        public override void Editor_InitOutPoints() {
            CommandGUI.AddMainOutPoint();
        }

        public override void Editor_OnDrawTitle(out string _tooltip) {
            _tooltip = @"Wait for an input before proceeding to the next command.";
            CommandGUI.DrawInPoint(0);
            CommandGUI.DrawOutPoint(0);
        }
                
        public override void Editor_OnDrawContents(Vector2 _absPosition) {            var labelWidth = UnityEditor.EditorGUIUtility.labelWidth;            UnityEditor.EditorGUIUtility.labelWidth = 85;            CommandGUI.DrawEnumField("Input Type", "", ref inputType);            if (inputType == InputType.KeyUp || inputType == InputType.KeyDown) {
                CommandGUI.DrawEnumField("Key Code", "", ref keyCode);
            }
            else {
                CommandGUI.DrawEnumField("Mouse Button", "", ref mouseButton);
            }
            UnityEditor.EditorGUIUtility.labelWidth = labelWidth;
        }
                
#endif
        #endregion NODE-DRAWER

    }

}