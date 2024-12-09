using System;
using System.Collections;
using System.Collections.Generic;
// using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    // [SerializeField] private Types typesListData;
    public int homes = 0;
    
    private int workplaces = 0;
    // Text shows how many of each type exist:
    [SerializeField] private Text homesText;
    [SerializeField] private Text workplacesText;
    // max for each type:
    [SerializeField] private int maxHomes = 4;
    
    [SerializeField] private Image happinessIcon;
    [SerializeField] private Sprite[] happinessIcons;
    
    public List<Building> slots = new List<Building>();

    [SerializeField] private BuildingShopSlot[] types;
    
    [SerializeField] private LocalizedString machinesUnlockedText;
    
    public static BuildingManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private int CheckExistance(int i)
    {
        long type = Saves.Instance.LoadBuilding(i.ToString(), -1);
        if (type == -1) return -1;  // Stop loading if there are no more buildings

        int level = (int)Saves.Instance.LoadBuildingLevel(i.ToString());
        
        if (types[type - 1] is Home home) 
            Build(home, i, level, true, home.levels[level].animator, true);
        else if (types[type - 1] is Industry industry) 
            Build(industry, i, level, true, industry.levels[level].animator, true);

        return 0;
    }

    private void Start()
    {
        // homes = (Saves.Instance.GetBuildingsCount());
        
        // int[] types = new int[slots.Count];
        int i = 0;
        while (true)  // Continue looping until explicitly broken
        {
            if (CheckExistance(i) == -1) break;
        
            i++;
        }

        CheckExistance(39); // port
        JobsLayoutConstructor.Instance.UpdateShopSlots();
    }

    public void Build(BuildingShopSlot _buildingShopSlot, int index = -1, int level = 0, bool newHome = true, RuntimeAnimatorController  animatorController = null, bool onStart = false)//(int type, AnimatorController animatorController = null)
    {
        // Check if the building is the special one with index 12 -> Port
        if (_buildingShopSlot.index == 12)
        {
            // Always place it in the special slot
            index = slots.Count-1;
            print("The port has level " + level);
            Booster_Spawner.Instance.EnableShips(level);
            
            if (Saves.Instance.LoadExtra("firstPort") == 0)
            {
                EducationPanel.Instance.SetEducation(0, machinesUnlockedText);
                Saves.Instance.SaveExtra("firstPort", 1);
            }
        }
        else
        {
            // If the index is not provided, assign it as usual
            if (index == -1)
                index = homes + workplaces;
            
            // newHome false = slot is being upgraded
            // newHome true = home on slot is being built
            if (newHome) homes++;
        }
        
        if (_buildingShopSlot.index == 26)
        {
            // Call EnableRoad with the building level
            print("The road has level " + level);
            Booster_Spawner.Instance.EnableRoad(level);
        }
        else if (_buildingShopSlot.index == 28)
        {
            print("Building plane track with level " + level);
            Booster_Spawner.Instance.EnablePlane(level);
        }
        
        slots[index].Build(_buildingShopSlot, level, animatorController);
        
        Saves.Instance.SaveBuilding(index.ToString(), (int) _buildingShopSlot.index, level);
        
        if (!onStart) AgeManager.Instance.AddBuiltHome();
        
        JobsLayoutConstructor.Instance.UpdateShopSlots();
        // AvailablePeopleText.Instance.CheckRedDot();
        // uncomment for first ever demo scene to work
        // CountHomesAndWork();
    }

    // public void UpgradeSlot(int id, BuildingShopSlot _buildingShopSlot)
    // {
    //     int nextLevel = 1; // TODO: Change to next level number
    //     
    //     if (_buildingShopSlot is Home home)
    //     {
    //         slots[id].Build(_buildingShopSlot, home.levels[nextLevel].animator);
    //     }
    //     else if (_buildingShopSlot is Industry industry)
    //     {
    //         slots[id].Build(_buildingShopSlot, industry.levels[nextLevel].animator);
    //     }
    // }

    private void CountHomesAndWork()
    {
        homes = 0;
        workplaces = 0;
        string types = "";
        
        for (int i = 0; i < slots.Count; i++)
        {
            long type = Saves.Instance.LoadBuilding(i.ToString());
            // int type = PlayerPrefs.GetInt(Types.playerPrefsPrefix + i.ToString(), 0);
            
            if (type == 1) homes++;
            else if (type == 2) workplaces++;
            
            types += " " + type;
            // print(i);
            // print(type);
            // homes += type == 1 ? 1 : 0;
            // workplaces += type == 2 ? 1 : 0;
        }
        
        print(types);

        homesText.text = homes.ToString();
        workplacesText.text = workplaces.ToString();
        
        DefineHappiness();
    }

    private void DefineHappiness()
    {
        if (homes == workplaces) // (homes == maxHomes && workplaces == maxHomes)
        {
            happinessIcon.sprite = happinessIcons[0];
            return;
        }
        
        happinessIcon.sprite = happinessIcons[1];
    }

    // Check for new building upgrades
    public void OnNewLevel()
    {
        int nextLevel = AgeManager.Instance.GetAge();
        int num = 0;
        
        for (int i = 0; i < PanelManager.Instance.country.ages.Length; i++)
        {
            for (int j = 0; j < PanelManager.Instance.country.ages[i].eras.Length; j++)
            {
                //print("Checking Age " + PanelManager.Instance.country.ages[i].eras[j].name);
                if (num == nextLevel)
                {
                    for (int k = 0; k < PanelManager.Instance.country.ages[i].eras[j].buildingShopSlots.Length; k++)
                    {
                        int buildings = (int) Saves.Instance.GetBuildingsOfType(PanelManager.Instance.country.ages[i].eras[j].buildingShopSlots[k].building.index);
                        if (buildings != -1)//(PanelManager.Instance.country.ages[i].eras[j].buildingShopSlots[k].IsNextLevel)
                        {
                            slots[buildings].UnlockNewLevel();
                            // foreach (int building in buildings)
                            // {
                            //     slots[building].UnlockNewLevel();
                            // }
                        }
                    }
                }

                num++;
            }
        }
    }
}
