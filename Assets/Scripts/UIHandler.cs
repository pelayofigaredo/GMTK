using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class UIHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] Transform attackButtonsParent;
    TextMeshProUGUI[] attackUIIndicators;


    internal void UpdateTimer(float roundTimer)
    {
       timerText.text = ToMinutesSeconds(roundTimer);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackUIIndicators = attackButtonsParent.GetComponentsInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateAttacks(IAttacker[] attacks)
    {
        for (int i = 0; i < attacks.Length; i++)
        {
            attackUIIndicators[i].transform.parent.gameObject.SetActive(attacks[i].isAlive());
            attackUIIndicators[i].text = attacks[i].GetUses()+"";
        }
    }

    public static string ToMinutesSeconds(float totalSeconds)
    {
        if (totalSeconds < 0f) totalSeconds = 0f;
        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}
