using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Types List")]
public class Types : ScriptableObject
{
    public List<Sprite> types;
    
    public static readonly string BuildingPlayerPrefsPrefix = "PlaceForBuilding_";
    
    public static readonly string JobPlayerPrefsPrefix = "JobOnPlaceForBuilding_";
    
    public static readonly string moneyPlayerPrefs = "money";
    
    public static readonly string savesPrefix = "Save_";
}
