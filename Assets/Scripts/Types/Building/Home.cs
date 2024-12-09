using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Home")]
public class Home : BuildingShopSlot
{
    public HomeLevel[] levels;
}

[Serializable]
public class HomeLevel : BuildingLevel
{
    // for Homes
    [Space]
    public long newPeople;

    public long income;
}