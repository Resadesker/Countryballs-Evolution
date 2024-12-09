using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Industry")]
public class Industry : BuildingShopSlot
{
    public IndustryLevel[] levels;
}

[Serializable]
public class IndustryLevel : BuildingLevel
{
    // for industry
    [Space]
    public long income; // amount of gold per click
    public long machineBonus;
}