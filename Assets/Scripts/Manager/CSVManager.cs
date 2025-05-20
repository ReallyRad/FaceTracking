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

    public Dictionary<string, string> experimentDataDictionary = new Dictionary<string, string>(); 
    
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
        //initialize dictionary with empty values.
        experimentDataDictionary.Add("PID" , "");
        experimentDataDictionary.Add("Experience" , "");

        string timeStamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        experimentDataDictionary.Add("Timestamp" , timeStamp);

        experimentDataDictionary.Add("exhaleCount", "");
        experimentDataDictionary.Add("exhaleSum", "");
        experimentDataDictionary.Add("exhaleEffective", "");
    }

    public void NewDataAvailableForDictionary() //write data to csv
    {
        experimentDataDictionary["PID"] = _subjectIDVariable.Value;
        experimentDataDictionary["Experience"] = ((Experience) _selectedExperience.Value).ToString();

        string path = Path.Combine(Application.persistentDataPath, _subjectIDVariable.Value + "_log.csv");
    
        String csv = String.Join(
            Environment.NewLine,
            experimentDataDictionary.Select(d => $"{d.Key};{d.Value};")
        );
    
        File.WriteAllText(path, csv);
    }

    private void LogExhaleDuration(float progressInMilliseconds)
    {
        _exhaleDurationsList.Add(progressInMilliseconds);
    }
    
    private void LogExhales() //bypassing experimentData data writing here. the advantage it had was to make sure data is logged as we go, instead of at the end of the scene
    {
        experimentDataDictionary["exhaleCount"] = _exhaleDurationsList.Count.ToString();
        experimentDataDictionary["exhaleSum"] = _exhaleDurationsList.Sum().ToString();
        experimentDataDictionary["exhaleEffective"] = _exhaleDurationsList.Select(value => value - 2000)
            .Where(result => result > 0)
            .Count()
            .ToString();
        NewDataAvailableForDictionary();
    }
    
}
