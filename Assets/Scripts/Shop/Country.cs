using System;
using UnityEngine;
using UnityEngine.Localization;

[Serializable]
public class EraInList
{
    public string name;
    public LocalizedString title;
    public Sprite avatar;
    public BuildingShopSlotInEra[] buildingShopSlots;
}

[Serializable]
public class Age
{
    public string name;
    public Sprite avatar;
    public EraInList[] eras;
}


[Serializable]
public class BuildingShopSlotInEra
{
    public BuildingShopSlot building;
    public int amount = 1;
    
    // level 0 is default, first level EVER
    public bool IsNextLevel;
}

[CreateAssetMenu(menuName = "Country")]
public class Country : ScriptableObject
{
    public string name;
    
    public Age[] ages;
    
    // public EraInList[] eras;
}
