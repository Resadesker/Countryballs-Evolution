using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AvailablePeopleText : MonoBehaviour
{
    private TMP_Text text;
    public static AvailablePeopleText Instance { get; private set; }

    [SerializeField] private Image redDot; // red dot appears near the person avatar in the panels menu if there are available people
    
    public void Initialize()
    {
        Instance = this;
        text = GetComponent<TMP_Text>();
        UpdateText();
        
        text.text = CurrencyManager.InstancePeople.amount-Saves.Instance.GetTotalWorkingPeople() + " / " + CurrencyManager.InstancePeople.amount;
    }

    // Update is called once per frame
    public void UpdateText()
    {
        long availablePeople = GetTotalAvailablePeople();
        text.text = $"{availablePeople} / {CurrencyManager.InstancePeople.amount}";
        // CheckRedDot(availablePeople);
    }

    public void CheckRedDot(long workingPeople = -1)
    {
        if (workingPeople == -1) workingPeople = Saves.Instance.GetTotalWorkingPeople();
        redDot.enabled = workingPeople < CurrencyManager.InstancePeople.amount && JobsLayoutConstructor.Instance.availableJobs > 0 && workingPeople != 0;
    }

    public long GetTotalAvailablePeople()
    {
        long num = 0;
        for (int i = 0; i < BuildingManager.Instance.homes; i++)
        {
            num += GetAvailablePeopleFromSlot(i);
        }
        return CurrencyManager.InstancePeople.amount-num;
    }
    
    public long GetAvailablePeopleFromSlot(int slot)
    {
        return Saves.Instance.LoadJob(slot);
    }
}
