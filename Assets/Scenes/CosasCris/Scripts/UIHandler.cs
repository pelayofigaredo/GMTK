using UnityEngine;
using TMPro;
using System;

public class UIHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] TextMeshProUGUI timerText;


    internal void UpdateTimer(float roundTimer)
    {
       timerText.text = ToMinutesSeconds(roundTimer);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static string ToMinutesSeconds(float totalSeconds)
    {
        if (totalSeconds < 0f) totalSeconds = 0f;
        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}
