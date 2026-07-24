using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class UIHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] UIAttackBarGroup attackBarGroup;


    internal void UpdateTimer(float roundTimer)
    {
       timerText.text = ToMinutesSeconds(roundTimer);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackBarGroup.SetValue(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateAttacks(IAttacker[] attacks)
    {
        for (int i = 0; i < attacks.Length; i++)
        {
            if (!attacks[i].isAlive())
            {
                attackBarGroup.ApplyColor(i, Color.gray3);
            }
            attackBarGroup.SetValue(i,attacks[i].GetUses());
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
