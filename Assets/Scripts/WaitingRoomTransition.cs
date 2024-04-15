using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Oculus.Interaction;
using VolumetricFogAndMist2;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;
using System.Data;

public class WaitingRoomTransition : MonoBehaviour
{
    public VolumetricFog fog;
    public TextMeshProUGUI timerText;

    private float timeLeft;
    private bool timerRunning = false;
    private float waitingDuration = 10;

    private IDbConnection dbConnection;
    private IDbCommand dbCommand;
    private string sqlQuery;

    void Start()
    {
        string connectionString = "URI=file:" + Application.dataPath + "/database.s3db";
        dbConnection = new SqliteConnection(connectionString);
        dbConnection.Open();
        dbCommand = dbConnection.CreateCommand();
        sqlQuery = "CREATE TABLE IF NOT EXISTS PerformanceData (SubjectId INTEGER, " +
                                                               "PreMood REAL, " +
                                                               "PreAnxiety REAL, " +
                                                               "PreStress REAL, " +
                                                               "PreFirstTimePoint DATETIME, " +
                                                               "PreSecondTimePoint DATETIME, " +
                                                               "PostMood REAL, " +
                                                               "PostAnxiety REAL, " +
                                                               "PostStress REAL, " +
                                                               "PostFirstTimePoint DATETIME, " +
                                                               "PostSecondTimePoint DATETIME)";

        dbCommand.CommandText = sqlQuery;
        dbCommand.ExecuteNonQuery();
        dbConnection.Close();
    }
    public void StartTimer(float duration)
    {
        timeLeft = duration;
        timerRunning = true;
    }

    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.One))
        {
            OnReadyButtonClicked();
        }

        if (timerRunning)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                timeLeft = 0;
                timerRunning = false;
                SceneManager.LoadScene("SceneSnow");
            }
            timerText.text = Mathf.RoundToInt(timeLeft).ToString();
            fog.settings.density = (waitingDuration - timeLeft) / waitingDuration;
        }
    }

    public void OnReadyButtonClicked()
    {
        StartTimer(waitingDuration);
    }
}
