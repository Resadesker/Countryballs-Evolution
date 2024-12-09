using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JobSlotConstructor : MonoBehaviour
{
    public BuildingShopSlot _buildingShopSlot;
    
    // currently only configured for "Industry" buildings
    
    [SerializeField] private Image avatar;
    [SerializeField] private TMP_Text label;
    [Space]
    [SerializeField] private TMP_Text income;
    [SerializeField] private TMP_Text workingPeople; // = peopleVault
    
    [SerializeField] private GameObject buyButton;

    public long freePeopleCount;
    private int level = 0; // TODO: (Script 2) Change it to normal level setting system

    private int slotIndex;
    // private void Start()
    // {
    //     Construct();
    // }

    public void Construct()
    {
        level = (int) Saves.Instance.LoadBuildingLevel(slotIndex.ToString()); //AgeManager.Instance.GetLevel(); print($"It's level {level} of {_buildingShopSlot.label} in age {AgeManager.Instance.currentAge}");
        if (_buildingShopSlot is Home home)
        {
            avatar.sprite = home.levels[level].UI_avatar;
        }
        else if (_buildingShopSlot is Industry industry)
        {
            // if (industry.levels[level].peopleVault == 0)
            // {
            //     buyButton.SetActive(false);
            // }

            avatar.sprite = industry.levels[level].UI_avatar;
            
            // specific for industry
            income.text = industry.levels[level].income.ToString(); //+ (industry.levels[level].peopleVault == 0 ? "" : " / чел");
            
            int workingPeopleCount = GetWorkingPeople();
            
            // workingPeople.text = $"{workingPeopleCount} / {industry.levels[level].peopleVault}";
            // freePeopleCount = industry.levels[level].peopleVault - workingPeopleCount;
            
            // if (workingPeopleCount >= industry.levels[level].peopleVault) buyButton.SetActive(false);
            //
            // if (industry.levels[level].peopleVault <= GetWorkingPeople()) Destroy(buyButton);
        }
        
        //avatar.SetNativeSize();
        
        // label.text = _buildingShopSlot.label;
    }

    public void SetIndex(int index)
    {
        slotIndex = index;
    }

    private int GetWorkingPeople()
    {
        return (int) AvailablePeopleText.Instance.GetAvailablePeopleFromSlot(slotIndex);
    }

    private void AddPeopleToSlot(int num)
    {
        Saves.Instance.SaveJob(slotIndex, GetWorkingPeople() + num);
        JobsLayoutConstructor.Instance.availableJobs -= num;
    }

    public void OnAddJobButtonClicked()
    {
        int peoplePrice = 1; // TODO: add more than 1 people to slot
        
        if (AvailablePeopleText.Instance.GetTotalAvailablePeople() < peoplePrice) return;
        if (_buildingShopSlot is Industry industry)
        {
            // if (industry.levels[level].peopleVault < GetWorkingPeople()) return;
            CurrencyManager.InstanceClicker.AddMoneyPerClick(industry.levels[level].income);

            // if (GetWorkingPeople() >= industry.levels[level].peopleVault)
            // {
            //     buyButton.SetActive(false);
            // }
        }

        AddPeopleToSlot(peoplePrice); 
        //AvailablePeopleText.Instance.CheckRedDot();
        // BuildingManager.Instance.Build(1);
        AvailablePeopleText.Instance.UpdateText();
        Construct();
        // PanelManager.Instance.GoToMap();

    }
}
