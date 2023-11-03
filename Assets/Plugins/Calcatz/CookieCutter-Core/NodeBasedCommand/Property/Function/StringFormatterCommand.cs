using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.CookieCutter {

    /// <summary>
    /// Creates a string value, which is formattable using format items.
    /// </summary>
    public class StringFormatterCommand : PropertyCommand, ITextFormatterCommand {

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

        public override T GetOutput<T>(CommandExecutionFlow _flow, int _pointIndex) {
            return (T)(object)GetFormattedText(_flow);
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

    }
}