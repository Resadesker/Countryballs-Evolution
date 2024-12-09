using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JobsLayoutConstructor : MonoBehaviour
{
    // [SerializeField] private Era era;
    [SerializeField] private JobSlotConstructor _shopSlotConstuctor;
    [SerializeField] private AvailablePeopleText _availablePeopleText;
    
    private RectTransform _rectTransform;
    
    public List<JobSlotConstructor> _shopSlotConstuctors;
    public static JobsLayoutConstructor Instance { get; private set; }

    public long availableJobs;

    [SerializeField] private ScrollRect _scrollRect;
    
    // TODO: Temportary variable, it should be replaced with a better approach by using UpdateShopSlots() as public later
    // public int amountOfCurrentSlots;
    
    public void Initialize()
    {
        Instance = this;
        _rectTransform = GetComponent<RectTransform>();
        _availablePeopleText.Initialize();
        
        // UpdateShopSlots();
        // ShowSlot(0);
    }

    public void UpdateShopSlots()
    {
        // TODO: not delete all slots, add ones that are needed one at a time
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
        
        _shopSlotConstuctors = new List<JobSlotConstructor>();
        availableJobs = 0;
        
        for (int i = 0; i < BuildingManager.Instance.homes; i++)
        {
            if (BuildingManager.Instance.slots[i]._buildingShopSlot is Home) continue;
            
            JobSlotConstructor shopSlot = Instantiate(_shopSlotConstuctor, transform);
            _shopSlotConstuctors.Add(shopSlot);
            shopSlot._buildingShopSlot = BuildingManager.Instance.slots[i]._buildingShopSlot;
            shopSlot.SetIndex(i);
            // shopSlot._buildingShopSlot = era._buildingShopSlots[i];
            shopSlot.Construct();
            availableJobs += shopSlot.freePeopleCount;
            if (shopSlot.freePeopleCount > 0) shopSlot.transform.SetAsFirstSibling();
            // amountOfCurrentSlots++;
        }
        UpdateShopSize();
    }
    
    public void OpenMenu()
    {
        _scrollRect.verticalNormalizedPosition = 1;
    }

    // Stop hiding what is in the slot
    public void ShowSlot(int num)
    {
        try
        {
            // _shopSlotConstuctors[num].ShowSlot();
        }
        catch
        {
            
        }
    }

    private void UpdateShopSize()
    {
        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, (_shopSlotConstuctors.Count + 1) * (279.6965f + 30));
    }
}
