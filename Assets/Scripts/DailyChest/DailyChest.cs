using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyChest : MonoBehaviour
{
    [SerializeField] private bool isRewarded;
    [SerializeField] private Image redDot; // alert = daily chest available
    private Button button;
    
    private void Start()
    {
        button = GetComponent<Button>();
        int currentDate = GetCurrentDate();
        string DBname = GetDBname();
        if (Saves.Instance.LoadExtra(DBname) != currentDate)
        {
            SetInteractable(true);
            
        }
        else
        {
            SetInteractable(false);
        }
    }

    public void DisableChestOnOpen()
    {
        int currentDate = GetCurrentDate();
        string DBname = GetDBname();
        
        Saves.Instance.SaveExtra(DBname, currentDate);

        SetInteractable(false);
    }

    private void SetInteractable(bool interactable)
    {
        if (!isRewarded)
        {
            redDot.enabled = interactable;
        }
        button.interactable = interactable;
    }

    private string GetDBname()
    {
        return isRewarded ? "lastRewardedChestOpen" : "lastChestOpen";
    }

    private int GetCurrentDate()
    {
        DateTime currentDate = DateTime.Now;

        // Combine year, month, and day into an integer (YYYYMMDD) for saving
        int year = currentDate.Year;
        int month = currentDate.Month;
        int day = currentDate.Day;

        int dateAsInt = year * 10000 + month * 100 + day;

        return dateAsInt;
    }
}
