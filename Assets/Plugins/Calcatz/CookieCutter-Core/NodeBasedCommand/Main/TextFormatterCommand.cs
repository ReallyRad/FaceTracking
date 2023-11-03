using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Base class for commands that include string formatting using arguments.
    /// </summary>
    [System.Serializable]
    public class TextFormatterCommand : Command, ITextFormatterCommand {

        public string templateText = "";

        public string TemplateText { get => templateText; set => templateText = value; }

        public void AddArgument() {
            inputIds.Add(new ConnectionTarget());
        }

        public void RemoveArgument(int _index) {
            if (_index < inputIds.Count) {
                inputIds.RemoveAt(_index);
            }
        }

        public override void Execute(CommandExecutionFlow _flow) {
            HandleFormattedText(_flow, GetFormattedText(_flow));
            base.Execute(_flow);
        }

        protected string GetFormattedText(CommandExecutionFlow _flow) {
            object[] arguments = new object[inputIds.Count];
            for (int i = 0; i < arguments.Length; i++) {
                arguments[i] = GetInput<object>(_flow, i);
            }
            //Debug.Log(templateText + " | " + arguments.Length);
            string formattedText = string.Format(templateText, arguments);
            return formattedText;
        }

        protected virtual void HandleFormattedText(CommandExecutionFlow _flow, string _formattedText) {
            
        }


    }


    public interface ITextFormatterCommand {

        string TemplateText { get; set; }
        void AddArgument();
        void RemoveArgument(int _index);

    }
}
