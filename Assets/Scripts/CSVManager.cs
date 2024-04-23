using System.Collections.Generic;
using UnityEngine;

public class CSVManager : MonoBehaviour
{
    //[SerializeField] private ExperimentData _experimentData;

    [SerializeField] private List<string> varNames = new List<string>();
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
    }
    
    private void WriteToFile(List<string> stringList, ExperimentData experimentData)
    {
        string stringLine = string.Join(",", stringList.ToArray());
        string path = "./Logs/" + experimentData.subjectID + "_log.csv";
        System.IO.StreamWriter file = new System.IO.StreamWriter(path, true);
        file.WriteLine(stringLine);
        file.Close();	
    }

    public void NewDataAvailable(ExperimentData experimentData)
    {
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
}
