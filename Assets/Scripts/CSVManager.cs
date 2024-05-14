using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ScriptableObjectArchitecture;
using Unity.VisualScripting;
using UnityEngine;

public class CSVManager : MonoBehaviour
{
    [SerializeField] private StringVariable _subjectID;
    [SerializeField] private Experience _selectedExperience;

    private Dictionary<string, string> experimentDataDictionary = new Dictionary<string, string>(); 
    
    private void Start ()
    {
        var fields = typeof(ExperimentData).GetFields();
        
        experimentDataDictionary.Add("PID" , "");

        //add pre and post answer types
        foreach (var answerType in Enum.GetValues(typeof(QuestionnaireAnswerType)))
            foreach (var state in Enum.GetValues(typeof(ExperimentState)))
                experimentDataDictionary.Add(answerType + "_" + state, "");

        experimentDataDictionary.Add("Timestamp" , "");
    }
    
    public void NewDataAvailableForDictionary(ExperimentData experimentData)
    {
        experimentDataDictionary["PID"] = experimentData.subjectID;

        foreach (var answerType in Enum.GetValues(typeof(QuestionnaireAnswerType)))
        {
            if (experimentData.answerType.ToString() == answerType.ToString())
            {
                var state = experimentData.experimentState.ToString();
                experimentDataDictionary[answerType + "_" + state] = experimentData.answerValue;
            }
        }
        
        string path = "./Logs/" + experimentData.subjectID + "_log.csv";
        
        String csv = String.Join(
            Environment.NewLine,
            experimentDataDictionary.Select(d => $"{d.Key};{d.Value};")
        );
        
        File.WriteAllText(path, csv);
    }
    
}
