using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotConstuctor : MonoBehaviour
{
    public BuildingShopSlot _buildingShopSlot;
    
    // currently only configured for "Industry" buildings

    [SerializeField] private Image avatar;
    [SerializeField] private TMP_Text price;
    [SerializeField] private TMP_Text label;

    public int indexInShopSlot;
    
    // for disabling the button
    [SerializeField] private GameObject disabledBackground;
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject hiddenForeground;
    public Image hiddenForegroundImage;
    [Space]
    // [SerializeField] private TMP_Text vault;
    [SerializeField] private TMP_Text income;
    [SerializeField] private TMP_Text peopleVault;
    [Space]
    [SerializeField] private TMP_Text lvlText;
    [SerializeField] private Image upgradeIcon;
    [Space]
    [SerializeField] private Image machineBonus;
    [SerializeField] private TMP_Text machineBonusText;
    [SerializeField] private Sprite[] machines; // 0 = ship; 1 = car; 2 = plane
    public SoundEmitter buttonSoundEmitter;

    private int age = 0; // At which age the building is unlocked
    
    public int level; // TODO: Change it to normal level setting system
    
    private bool isDisabled = false;

    public void Construct(int level = 0)
    {
        this.level = level;
        LanguageSelector.Instance.OnLanguageChange += UpdateLocalizedTexts;
        
        if (_buildingShopSlot is Industry industry)
        {
            avatar.sprite = industry.levels[level].UI_avatar;
        
            price.text = Price_Display.Instance.Price_To_Text(industry.levels[level].price * ShopConstructor.Instance.priceMultiplier);
            label.text = LanguageSelector.Instance.GetLocalizedString(industry.title);
            if (industry.index == 12 || industry.index == 26 || industry.index == 28)
            {
                machineBonus.gameObject.SetActive(true);
                machineBonus.sprite =
                    (industry.index == 12 ? machines[0] : (industry.index == 26 ? machines[1] : machines[2]));
                string machineText = level == 0 ? Price_Display.Instance.Price_To_Text(industry.levels[level].machineBonus) : 
                    Price_Display.Instance.Price_To_Text(industry.levels[level-1].machineBonus) + " -> " + 
                    Price_Display.Instance.Price_To_Text(industry.levels[level].machineBonus);
                machineBonusText.text = machineText;
            }
            string chel = "";
            string incomeText = level == 0 ? Price_Display.Instance.Price_To_Text(industry.levels[level].income) + chel : 
                Price_Display.Instance.Price_To_Text(industry.levels[level-1].income) + " -> " + 
                Price_Display.Instance.Price_To_Text(industry.levels[level].income) + chel;
            income.text = incomeText;
        }
        else if (_buildingShopSlot is Home home)
        {
            avatar.sprite = home.levels[level].UI_avatar;

            string newPeopleText = level == 0
                ? Price_Display.Instance.Price_To_Text(home.levels[level].newPeople)
                : Price_Display.Instance.Price_To_Text(home.levels[level - 1].newPeople) + " -> " +
                  Price_Display.Instance.Price_To_Text(home.levels[level].newPeople);
            string incomeText = level == 0 ? Price_Display.Instance.Price_To_Text(home.levels[level].income) : 
                Price_Display.Instance.Price_To_Text(home.levels[level-1].income) + " -> " + 
                Price_Display.Instance.Price_To_Text(home.levels[level].income);
            price.text = Price_Display.Instance.Price_To_Text(home.levels[level].price * ShopConstructor.Instance.priceMultiplier);
            label.text = LanguageSelector.Instance.GetLocalizedString(home.title);

            income.text = incomeText;
            peopleVault.text = newPeopleText;
        }
        
        lvlText.text = (level + 1) + " lvl";
        if (level != 0) upgradeIcon.gameObject.SetActive(true);
        //avatar.SetNativeSize();
    }
    
    private void UpdateLocalizedTexts()
    {
        if (_buildingShopSlot is Industry industry)
        {
            label.text = LanguageSelector.Instance.GetLocalizedString(industry.title);
        }
        else if (_buildingShopSlot is Home home)
        {
            label.text = LanguageSelector.Instance.GetLocalizedString(home.title);
        }
    }

    public void DisableSlot()
    {
        disabledBackground.SetActive(true);
        Destroy(button);
        isDisabled = true;
    }
    
    private void HideSlot()
    {
        hiddenForeground.SetActive(true);
    }
    
    public void ShowSlot()
    {
        // if (!isDisabled) Construct();
        hiddenForeground.SetActive(false);
    }

    public void OnBuyButtonClicked()
    {
        buttonSoundEmitter.Play();
        if (_buildingShopSlot is Home home)
        {
            if (CurrencyManager.InstanceClicker.amount < home.levels[level].price * ShopConstructor.Instance.priceMultiplier) return;
            // specific
            long plusPeople;
            // 30 newPeople but previous level had 20 newPeople = +10 NEW people
            if (level == 0) plusPeople = home.levels[level].newPeople;
            else plusPeople = home.levels[level].newPeople - home.levels[level - 1].newPeople;
            
            CurrencyManager.InstancePeople.AddPeople(plusPeople);
            if (level == 0) CurrencyManager.InstanceClicker.AddMoneyPerClick(home.levels[level].income);
            else CurrencyManager.InstanceClicker.AddMoneyPerClick(home.levels[level].income - home.levels[level - 1].income);
            AvailablePeopleText.Instance.UpdateText();
            BuyBuilding(home);
        }
        else if (_buildingShopSlot is Industry industry)
        {
            if (CurrencyManager.InstanceClicker.amount < industry.levels[level].price * ShopConstructor.Instance.priceMultiplier) return;
            BuyBuilding(industry);
        }
    }
    
    private int GetLevel()
    {
        long arr = Saves.Instance.GetBuildingsOfType(_buildingShopSlot.index);
        print($"Buildings of type {_buildingShopSlot.label}: " + arr);
        if (arr == -1) print("Level of the building: " + Saves.Instance.LoadBuildingLevel(arr.ToString()));
        return (arr == -1 ? 0 : (int) Saves.Instance.LoadBuildingLevel(arr.ToString())+1);
    }

    private void BuyBuilding(BuildingShopSlot _buildingShopSlot)
    {
        // universal
        RuntimeAnimatorController animatorController = null;
        long builtOnSlot = 0;
        if (level != 0) builtOnSlot = Saves.Instance.GetBuildingsOfType(_buildingShopSlot.index);
        if (_buildingShopSlot is Home home)
        {
            if (CurrencyManager.InstanceClicker.amount < home.levels[level].price * ShopConstructor.Instance.priceMultiplier) return;
            CurrencyManager.InstanceClicker.AddMoneyAmount(-home.levels[level].price * ShopConstructor.Instance.priceMultiplier);
            animatorController = home.levels[level].animator;
            
        }
        else if (_buildingShopSlot is Industry industry)
        {
            if (CurrencyManager.InstanceClicker.amount < industry.levels[level].price * ShopConstructor.Instance.priceMultiplier) return;
            CurrencyManager.InstanceClicker.AddMoneyAmount(-industry.levels[level].price * ShopConstructor.Instance.priceMultiplier);
            if (level > 0) CurrencyManager.InstanceClicker.AddMoneyPerClick(industry.levels[level].income - industry.levels[level - 1].income);
            else CurrencyManager.InstanceClicker.AddMoneyPerClick(industry.levels[level].income);
            animatorController = industry.levels[level].animator;
        }
        
        if (level == 0) BuildingManager.Instance.Build(_buildingShopSlot, animatorController: animatorController, level: level);
        else
        {
            BuildingManager.Instance.Build(_buildingShopSlot, (int) builtOnSlot, level, false, animatorController);
        }
        
        PanelManager.Instance.GoToMap();
        CurrencyManager.InstanceClicker.UpdateText();
        ProgressBar.Instance.UpdateBar();
        DisableSlot();
    }
}
