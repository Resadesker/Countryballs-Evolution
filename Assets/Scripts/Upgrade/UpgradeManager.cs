using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
// using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }
    
    [SerializeField] private TMP_Text priceText;

    private long _price;
    private int _selectedSlotIndex;
    // private BuildingShopSlot _selectedSlotType;
    private Building _selectedBuilding;
    

    // Start is called before the first frame update
    private void Start()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void OnBuildingSelected(int index, Building building)
    {
        OnBuildingDeselected();
        _selectedBuilding = building;
        _selectedSlotIndex = index;
        int level = GetLevel();
        //print("Selected slot: " + _selectedSlotIndex);
        
        if (IsLastLevel(level) || !_selectedBuilding.IsNextLevelAvailable())
        {
            OnBuildingDeselected();
            CurrencyManager.InstanceClicker.OnClick();
            return;
        }
        print(IsLastLevel(level));
        // _selectedSlotType = _buildingShopSlot;
        gameObject.SetActive(true);
        CalculatePrice(level + 1);
    }

    public void OnBuildingDeselected()
    {
        try
        {
            if (_selectedBuilding._buildingShopSlot == null) return;
        }
        catch (NullReferenceException)
        {
            gameObject.SetActive(false);
        }
        
        DeselectBuilding();
    }

    private void DeselectBuilding()
    {
        gameObject.SetActive(false); 
        _selectedSlotIndex = -1;
        
        if (_selectedBuilding == null) return;
        
        try
        {
            _selectedBuilding.DeselectBuilding();
            // _selectedBuilding.selectedImage.SetActive(false);
        }
        catch (NullReferenceException e)
        {
            // throw e;
            // ignore, already deselected
        }
        _selectedBuilding = null;
    }

    public void OnUpgradeButtonClicked()
    {
        if (CurrencyManager.InstanceClicker.amount < _price) return;
        
        CurrencyManager.InstanceClicker.AddMoneyAmount(-_price);
        // Get the selected building index
        
        int level = GetLevel() + 1; // add 1 to the current level (next one)

        print("Building on slot " + _selectedSlotIndex);
        print($"Updating level {level}");
        
        BuildingManager.Instance.Build(_selectedBuilding._buildingShopSlot, _selectedSlotIndex, level, false, GetAnimator(level));
        _selectedBuilding.UpgradeLevel();
        if (IsLastLevel(level) || !_selectedBuilding.IsNextLevelAvailable())
        {
            OnBuildingDeselected();
            return;
        }
        CalculatePrice(level + 1);
    }

    private RuntimeAnimatorController GetAnimator(int level)
    {
        try
        {
            RuntimeAnimatorController animatorController = null;

            if (_selectedBuilding._buildingShopSlot is Home home)
            {
                animatorController = home.levels[level].animator;
            }
            else if (_selectedBuilding._buildingShopSlot is Industry industry)
            {
                animatorController = industry.levels[level].animator;
            }

            return animatorController;
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    private void CalculatePrice(int level)
    {
        _price = _selectedBuilding._buildingShopSlot switch
        {
            Home home => home.levels[level].price * ShopConstructor.Instance.priceMultiplier - home.levels[level - 1].price * ShopConstructor.Instance.priceMultiplier,
            Industry industry => industry.levels[level].price * ShopConstructor.Instance.priceMultiplier - industry.levels[level - 1].price * ShopConstructor.Instance.priceMultiplier,
            _ => 0
        };
        
        priceText.text = _price.ToString();
    }

    // private bool IsLastLevel(int level)
    // {
    //     try
    //     {
    //         if (_selectedBuilding._buildingShopSlot is Home home)
    //         {
    //             HomeLevel homeLevel = home.levels[level + 1];
    //         }
    //         else if (_selectedBuilding._buildingShopSlot is Industry industry)
    //         {
    //             IndustryLevel industryLevel = industry.levels[level + 1];
    //         }
    //         
    //         return false;
    //     }
    //     catch (Exception e)
    //     {
    //         // throw e;
    //         return true;
    //     }
    // }
    
    private bool IsLastLevel(int level)
    {
        try
        {
            if (_selectedBuilding._buildingShopSlot is Home home && level + 1 >= home.levels.Length)
            {
                // Last level for Home
                return true;
            }
            else if (_selectedBuilding._buildingShopSlot is Industry industry && level + 1 >= industry.levels.Length)
            {
                // Last level for Industry
                return true;
            }
        
            return false;
        }
        catch (IndexOutOfRangeException e)
        {
            return true;
        }
    }
    
    private int GetLevel()
    {
        return (int) Saves.Instance.LoadBuildingLevel(_selectedSlotIndex.ToString());
    }
}
