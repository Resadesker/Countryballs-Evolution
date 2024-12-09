using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ShopConstructor : MonoBehaviour
{
    [FormerlySerializedAs("_shopSlotConstuctor")] [SerializeField] private ShopSlotConstuctor _industrySlotConstuctor;
    [SerializeField] private ShopSlotConstuctor _buildingSlotConstuctor;
    [SerializeField] private EraTitleShop _eraTitleShop;
    
    private RectTransform _rectTransform;
    
    private List<List<ShopSlotConstuctor>> _shopSlotConstuctors = new List<List<ShopSlotConstuctor>>();
    public static ShopConstructor Instance { get; private set; }

    // TODO: Temportary variable, it should be replaced with a better approach by using UpdateShopSlots() as public later
    public float amountOfCurrentSlots;
    
    private float shopSize = 0;

    private readonly float slotHeight = 279.6965f;
    private readonly float ageTextHeight = 134.2411f;

    private float openAgeOnHeight; // height to open the menu
    private float currentAgeHeightInMenu; // the height of the current age in the menu, so that we skip this part and open the next age when we're done

    public int priceMultiplier = 40;
    
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private SoundEmitter buttonSoundEmitter;
    [SerializeField] private Sprite[] hiddenForegroundSprites; // ranked 0 - 4 as eras
    
    public void Initialize()
    {
        Instance = this;
        _rectTransform = GetComponent<RectTransform>();
        
        UpdateShopSlots();
        // ShowSlot(0);
        int level = AgeManager.Instance.GetLevel();
        int era = AgeManager.Instance.GetEra();
        int age = AgeManager.Instance.GetAge();
        for (int i = 0; i <= level; i++)
        {
            // int[] eraAndAge = AgeManager.Instance.Int_To_Era_And_Age(level);
            
            if (i < level)
            {
                UnlockAge(i, age, era, true);
            }
            else
            {
                AgeManager.Instance.SetMaxHomesInAge(PanelManager.Instance.country.ages[era].eras[age].buildingShopSlots.Length);
                UnlockAge(i, age, era, null);
            }
        }
        
        OpenMenu();
        //Saves.Instance.SaveExtra("Age", 0); // TODO: Remove, Added for TdOT for easy access
    }

    private void UpdateShopSlots()
    {
        int num = 0;
        // - HashSet<uint> alreadyPlaced = new HashSet<uint>();
        int[] array = new int[45];
        for (int u = 0; u < PanelManager.Instance.country.ages.Length; u++)
        {
            for (int i = 0; i < PanelManager.Instance.country.ages[u].eras.Length; i++)
            {
                _shopSlotConstuctors.Add(new List<ShopSlotConstuctor>());
                EraTitleShop eraTitle = Instantiate(_eraTitleShop, transform);
                eraTitle.SetEraTitle(PanelManager.Instance.country.ages[u].eras[i]);
                shopSize += ageTextHeight;
                for (int j = 0; j < PanelManager.Instance.country.ages[u].eras[i].buildingShopSlots.Length; j++)
                {
                    ShopSlotConstuctor shopSlot = Instantiate(PanelManager.Instance.country.ages[u].eras[i].buildingShopSlots[j].building is Industry 
                        ? _industrySlotConstuctor : _buildingSlotConstuctor, transform);

                    shopSlot.indexInShopSlot = _shopSlotConstuctors.Count;
                    _shopSlotConstuctors[num].Add(shopSlot);
                    shopSlot._buildingShopSlot = PanelManager.Instance.country.ages[u].eras[i].buildingShopSlots[j].building;
                    shopSlot.Construct(array[PanelManager.Instance.country.ages[u].eras[i].buildingShopSlots[j].building.index]);
                    shopSlot.buttonSoundEmitter = buttonSoundEmitter;
                    shopSlot.hiddenForegroundImage.sprite = hiddenForegroundSprites[u];
                    amountOfCurrentSlots++;
                    
                    array[PanelManager.Instance.country.ages[u].eras[i].buildingShopSlots[j].building.index]++;
                    
                    shopSize += slotHeight;
                }
                num++;
                amountOfCurrentSlots += 0.5f;
            }
        }
        UpdateShopSize();
    }

    public void OpenMenu()
    {
        // scrollRect.verticalNormalizedPosition = Mathf.Clamp01(openAgeOnHeight);
        float contentHeight = scrollRect.content.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;
        
        // Prevent dividing by zero in case of unexpected setup
        if (contentHeight <= viewportHeight)
        {
            scrollRect.verticalNormalizedPosition = 1.0f;
        }
        else
        {
            // Calculate the target normalized position
            float normalizedPosition = Mathf.Clamp01((contentHeight - openAgeOnHeight) / (contentHeight - viewportHeight));
            scrollRect.verticalNormalizedPosition = normalizedPosition;
        }
    }

    // Stop hiding what is in the slot
    public void ShowSlot(int age, int num)
    {
        try
        {
            // print($"Unlocked age {age} num {num}");
            int i = 0;
            foreach (var slot in _shopSlotConstuctors[age]) {print(slot.name + " " + i); i++;}
            _shopSlotConstuctors[age][num].ShowSlot();
        }
        catch (Exception e)
        {
            throw e;
        }
    }
    
    public void UnlockAge(int level, int age = 0, int era = 0, bool? disable = false)
    {
        currentAgeHeightInMenu = 0;
    
        // Iterate through each slot in the specified age
        foreach (var slot in _shopSlotConstuctors[level])
        {
            // Show the slot in the shop
            slot.ShowSlot();
            currentAgeHeightInMenu += slotHeight;
        
            try
            {
                // If 'disable' is null, check if the building is already built
                if (!disable.HasValue)
                {
                    long slotIndex = Saves.Instance.GetBuildingsOfType(slot._buildingShopSlot.index);
                    if (slotIndex != -1)
                    {
                        // If the slot's building level matches the saved building level, it means the building is built
                        if (slot.level == Saves.Instance.LoadBuildingLevel(slotIndex.ToString(), -1))
                        {
                            AgeManager.Instance.AddBuiltHome();
                            slot.DisableSlot();
                        }
                    }
                }
                // If 'disable' is true, disable the slot
                else if (disable.Value)
                {
                    slot.DisableSlot();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in UnlockAge: {e.Message}");
            }
        }

        // Update the height for opening the age in the menu
        openAgeOnHeight += currentAgeHeightInMenu + ageTextHeight;

        // Set the maximum number of homes in the current age
        if (disable.HasValue) AgeManager.Instance.SetMaxHomesInAge(
            PanelManager.Instance.country.ages[era].eras[age].buildingShopSlots.Length
        );
    }

    private void UpdateShopSize()
    {
        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, shopSize);
    }
}