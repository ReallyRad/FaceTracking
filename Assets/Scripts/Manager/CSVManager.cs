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
    [SerializeField] private ExperimentStateSO _experimentStateSO; //stores pre/post state across scenes
    [SerializeField] private ExperimentData _experimentData; //contains one VAS answers
    [SerializeField] private ExperimentDataStorage _experimentDataStorage; //storage of the entire data to dictionary format to be written to csv

    private List<float> _exhaleDurationsList = new List<float>();

    private void OnEnable()
    {
        ProgressManager.ExhaleEnded += LogExhaleDuration;
        SceneTimer.SceneFinished += LogExhales;
    }

    private void OnDisable()
    {
        ProgressManager.ExhaleEnded -= LogExhaleDuration;
        SceneTimer.SceneFinished -= LogExhales;
    }

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

        _experimentDataStorage.experimentDataDictionary.Add("exhaleCount", "");
        _experimentDataStorage.experimentDataDictionary.Add("exhaleSum", "");
        _experimentDataStorage.experimentDataDictionary.Add("exhaleEffective", "");
    }

    public void NewDataAvailableForDictionary() //write data to csv
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

    private void LogExhaleDuration(float progressInMilliseconds)
    {
        _exhaleDurationsList.Add(progressInMilliseconds);
    }
    
    private void LogExhales() //bypassing experimentData data writing here. the advantage it had was to make sure data is logged as we go, instead of at the end of the scene
    {
        _experimentDataStorage.experimentDataDictionary["exhaleCount"] = _exhaleDurationsList.Count.ToString();
        _experimentDataStorage.experimentDataDictionary["exhaleSum"] = _exhaleDurationsList.Sum().ToString();
        _experimentDataStorage.experimentDataDictionary["exhaleEffective"] = _exhaleDurationsList.Select(value => value - 2000)
            .Where(result => result > 0)
            .Count()
            .ToString();
        NewDataAvailableForDictionary();
    }
    
}
