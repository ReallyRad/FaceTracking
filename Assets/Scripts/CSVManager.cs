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
    [SerializeField] private List<string> varNames = new List<string>();

    private Dictionary<string, string> experimentDataDictionary = new Dictionary<string, string>(); 
    
    private List<string> varValues = new List<string>();

    private bool _newFile;
    
    private void Start ()
    {
        var fields = typeof(ExperimentData).GetFields();
        
        foreach (var field in fields) {
            varValues.Add(null); //initialize varNames array
            varNames.Add(field.Name);
        }

        _newFile = true;
        
        experimentDataDictionary.Add("PID" , "");

        //add pre and post answer types
        foreach (var answerType in Enum.GetValues(typeof(QuestionnaireAnswerType)))
            foreach (var state in Enum.GetValues(typeof(ExperimentState)))
                experimentDataDictionary.Add(answerType + "_" + state, "");

        experimentDataDictionary.Add("Timestamp" , "");
        
        
    }
    
    public void NewDataAvailable(ExperimentData experimentData)
    {
        experimentData.subjectID = _subjectID.Value;
        if (_newFile) //if this is the first time writing to this CSV, start with column names
        {
            WriteToFile(varNames, experimentData);
            _newFile = false;
        }
        var fields = typeof(ExperimentData).GetFields();
        for (int i=0; i<fields.Length; i++) //write values from ExperimentData scriptable object to the csv file
        {
            var result = fields[i].GetValue(experimentData);
            if (result == null) varValues[i] = "";
            else varValues[i] = result.ToString();
        }
        WriteToFile(varValues, experimentData);
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
    
    private void WriteToFile(List<string> stringList, ExperimentData experimentData)
    {
        string stringLine = string.Join(",", stringList.ToArray());
        string path = "./Logs/" + experimentData.subjectID + "_log.csv";
        System.IO.StreamWriter file = new System.IO.StreamWriter(path, true);
        file.WriteLine(stringLine);
        file.Close();	
    }

}
