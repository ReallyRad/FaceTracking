using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ScriptableObjectArchitecture;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class CSVManager : MonoBehaviour
{
    [SerializeField] private StringVariable _subjectIDVariable;
    [SerializeField] private IntVariable _selectedExperience;
    [SerializeField] private ExperimentStateSO _experimentStateSO;
    [SerializeField] private ExperimentData _experimentData;

    private Dictionary<string, string> experimentDataDictionary = new Dictionary<string, string>(); 
    
    private void Start ()
    {
        var fields = typeof(ExperimentData).GetFields();
        
        //initialize dictionary witth empty values.
        experimentDataDictionary.Add("PID" , "");
        experimentDataDictionary.Add("Experience" , "");

        //add pre and post answer types
        foreach (var answerType in Enum.GetValues(typeof(QuestionnaireAnswerType)))
            foreach (var state in Enum.GetValues(typeof(ExperimentState)))
                experimentDataDictionary.Add(answerType + "_" + state, "");

        experimentDataDictionary.Add("Timestamp" , "");
    }
    
    public void NewDataAvailableForDictionary()
    {
        experimentDataDictionary["PID"] = _subjectIDVariable.Value;
        experimentDataDictionary["Experience"] = ((Experience) _selectedExperience.Value).ToString();

        foreach (var answerType in Enum.GetValues(typeof(QuestionnaireAnswerType)))
        {
            if (_experimentData.answerType.ToString() == answerType.ToString()) //write the experimentdata value to the correct spot
            {
                var state = _experimentStateSO.experimentState.ToString();
                experimentDataDictionary[answerType + "_" + state] = _experimentData.answerValue;
            }
        }
        string path = Path.Combine(Application.persistentDataPath, _subjectIDVariable.Value + "_log.csv");
    
        String csv = String.Join(
            Environment.NewLine,
            experimentDataDictionary.Select(d => $"{d.Key};{d.Value};")
        );
    
        File.WriteAllText(path, csv);
    }
    
}
