using System;
// using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

[CreateAssetMenu(menuName = "Building Shop Slot")]
public class BuildingShopSlot : ScriptableObject
{
    public uint index = 0;
    public string label;
    public LocalizedString title;
    public Countries country;
    
    private static uint previousIndex = 0;

    private void OnEnable()
    {
        // При создании нового экземпляра увеличиваем индекс на 1 относительно предыдущего
        if (index == 0)
            index = ++previousIndex;
    }
}
public class BuildingLevel
{
    public Sprite avatar;
    public Sprite UI_avatar; // avatar for the shop, in full resolution and quality
    public long price;
    
    // universal, but optional
    [Space]
    public RuntimeAnimatorController animator;
}