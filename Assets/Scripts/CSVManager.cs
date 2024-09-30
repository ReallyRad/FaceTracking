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
    [SerializeField] private ExperimentStateSO _experimentStateSO; //stores pre/post state accross scenes
    [SerializeField] private ExperimentData _experimentData; //contains one VAS answers
    [SerializeField] private ExperimentDataStorage _experimentDataStorage; //storage of the entire data to dictionary format to be written to csv
    
    private void Start ()
    {
        var fields = typeof(ExperimentData).GetFields();
        
        //initialize dictionary with empty values.
        _experimentDataStorage.experimentDataDictionary.Add("PID" , "");
        _experimentDataStorage.experimentDataDictionary.Add("Experience" , "");

        //add pre and post answer types
        foreach (var answerType in Enum.GetValues(typeof(QuestionnaireAnswerType)))
            foreach (var state in Enum.GetValues(typeof(ExperimentState)))
                _experimentDataStorage.experimentDataDictionary.Add(answerType + "_" + state, "");


        string timeStamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        _experimentDataStorage.experimentDataDictionary.Add("Timestamp" , timeStamp);
    }
    
    public void NewDataAvailableForDictionary()
    {
        _experimentDataStorage.experimentDataDictionary["PID"] = _subjectIDVariable.Value;
        _experimentDataStorage.experimentDataDictionary["Experience"] = ((Experience) _selectedExperience.Value).ToString();

        foreach (var answerType in Enum.GetValues(typeof(QuestionnaireAnswerType)))
        {
            if (_experimentData.answerType.ToString() == answerType.ToString()) //write the experimentdata value to the correct spot
            {
                var state = _experimentStateSO.experimentState.ToString();
                _experimentDataStorage.experimentDataDictionary[answerType + "_" + state] = _experimentData.answerValue;
            }
        }
        string path = Path.Combine(Application.persistentDataPath, _subjectIDVariable.Value + "_log.csv");
    
        String csv = String.Join(
            Environment.NewLine,
            _experimentDataStorage.experimentDataDictionary.Select(d => $"{d.Key};{d.Value};")
        );
    
        File.WriteAllText(path, csv);
    }
    
}
