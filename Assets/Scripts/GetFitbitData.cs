using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using TMPro;
using Oculus.Interaction;


public class GetFitbitData : MonoBehaviour
{
    private string accessToken;
    public TextMeshProUGUI heartRateTMP;

    private void Start()
    {
        accessToken = "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiIyMzk4SzciLCJzdWIiOiJCQ1JUREIiLCJpc3MiOiJGaXRiaXQiLCJ0eXAiOiJhY2Nlc3NfdG9rZW4iLCJzY29wZXMiOiJyaHIiLCJleHAiOjE3MDk2NjY0NjEsImlhdCI6MTcwOTYzNzY2MX0.OmYcLBHOoklsJUeP3CtSs33PbPIApTB-elr8HoH-Eso";
        //GetHeartRateData();
        //InvokeRepeating("GetHeartRateData", 0f, 5f);
        StartCoroutine(RepeatCoroutine());
    }
    private void Update()
    {
        //string date = DateTime.Now.ToString("yyyy-MM-dd");
        //string time = DateTime.Now.AddSeconds(-15).ToString("HH:mm:ss");
        //heartRateTMP.text = string.Format("Date is: {0}, Time is {1}", date, time);

        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.Active))
        {
            heartRateTMP.gameObject.SetActive(!heartRateTMP.gameObject.activeSelf);
        }
    }
    private void GetData()
    {
        string date = DateTime.Now.ToString("yyyy-MM-dd");
        string startTime = DateTime.Now.AddSeconds(-5).ToString("HH:mm:ss");
        string endTime = DateTime.Now.ToString("HH:mm:ss");
        //string date = "2023-01-23";
        //string startTime = "10:54";
        //string endTime = "11:05";
        //string endpoint = string.Format("https://api.fitbit.com/1/user/-/activities/heart/date/{0}/1d/1sec/time/{1}/{2}.json", date, startTime, endTime);
        //string endpoint = "https://api.fitbit.com/1/user/-/activities/heart/date/2023-01-23/1d/1sec/time/10:54/11:05.json";
        string endpoint = "https://api.fitbit.com/1/user/-/activities/heart/date/2023-01-17/1d/1min/time/09:00/11:10.json";

        UnityWebRequest request = UnityWebRequest.Get(endpoint);
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);

        request.SendWebRequest();
        string result = request.downloadHandler.text;

        result = result.Replace("activities-heart", "activities_heart");
        result = result.Replace("heart-intraday", "heart_intraday");
        Debug.Log(result);

        HeartRateDataStructure jsonResult = JsonUtility.FromJson<HeartRateDataStructure>(result);

        Debug.Log(jsonResult);
        Debug.Log(jsonResult.activities_heart_intraday.dataset[4].value);

        int number = jsonResult.activities_heart_intraday.dataset.Length;
        double m = 0;
        for (int i = 0; i <= number; i++)
        {
            m = m + jsonResult.activities_heart_intraday.dataset[i].value;
        }
        //double heartRate = m / number;
        //heartRateTMP.text = string.Format("Your Current Heart Rate is: {0}", heartRate);

        int count = jsonResult.activities_heart_intraday.dataset.Length;
        int heartRate = jsonResult.activities_heart_intraday.dataset[count - 1].value;
        heartRateTMP.text = string.Format("Your Current Heart Rate is: {0}", heartRate);
    }

    IEnumerator RepeatCoroutine()
    {
        while (true)
        {
            // Start your coroutine here
            StartCoroutine(GetHeartRateData());

            // Wait for 5 seconds before repeating
            yield return new WaitForSeconds(5f);
        }
    }
    private IEnumerator GetHeartRateData()
    {
        string date = DateTime.Now.ToString("yyyy-MM-dd");
        string startTime = DateTime.Now.AddSeconds(-150).ToString("HH:mm");
        string endTime = DateTime.Now.ToString("HH:mm");
        string endpoint = string.Format("https://api.fitbit.com/1/user/-/activities/heart/date/{0}/1d/1sec/time/{1}/{2}.json", date, startTime, endTime);
        //string endpoint = "https://api.fitbit.com/1/user/-/activities/heart/date/2023-01-17/1d/1min/time/09:01/11:11.json";
        UnityWebRequest request = UnityWebRequest.Get(endpoint);
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string result = request.downloadHandler.text;
            Debug.Log("received json: " + result);
            result = result.Replace("activities-heart", "activities_heart");
            result = result.Replace("heart-intraday", "heart_intraday");
            HeartRateDataStructure jsonResult = JsonUtility.FromJson<HeartRateDataStructure>(result);
            Debug.Log("constructed json: " + result);
            int count = jsonResult.activities_heart_intraday.dataset.Length;
            if (count > 0)
            {
                int heartRate = jsonResult.activities_heart_intraday.dataset[count - 1].value;
                heartRateTMP.text = string.Format("Your Current Heart Rate is: {0}", heartRate);
            }
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }


    }
}

[Serializable]
public class HeartRateDataStructure
{
    public string activities_heart;
    public Activities_Heart_Intraday activities_heart_intraday;
}

[Serializable]
public class Activities_Heart_Intraday
{
    public Dataset[] dataset;
    public int datasetInterval;
    public string datasetType;
}

[Serializable]
public class Dataset
{
    public string time;
    public int value;
}


