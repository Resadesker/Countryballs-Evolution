using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.UI;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] private CurrencyTypes currencyType;
    [SerializeField] private BallAnimation ballAnimation; // for coins only
    private string currencyPlayerPrefsName;
    public long amount = 0;
    private TMP_Text text;

    public long TotalMoneyPerClick = 1;
    private string totalMoneyPerClickPlayerPrefsName = "TotalMoneyPerClick";

    [SerializeField] private LocalizedString firstShop;
    [SerializeField] private LocalizedString agesArrow;
    
    [SerializeField] private LocalizedString donateArrow;
    [SerializeField] private LocalizedString goodLuck;

    [SerializeField] private SoundEmitter clickSoundEmiter;
    
    public static CurrencyManager InstanceClicker { get; private set; }
    public static CurrencyManager InstancePeople { get; private set; }

    public bool isDoubledPeople = false;
    
    // for in-app purchased skins
    public bool isX4People = false;
    public bool isX2Coins = false;

    // Start is called before the first frame update
    private void Awake()
    {
        if (currencyType == CurrencyTypes.Money)
        {
            currencyPlayerPrefsName = "Money";
            InstanceClicker = this;
            StartCoroutine(PassiveIncome());
        }
        else
        {
            currencyPlayerPrefsName = "People";
            InstancePeople = this;
        }
        
        text = GetComponent<TMP_Text>();
        
        GetMoneyPerClick();
        
        UpdateText();
    }

    private IEnumerator PassiveIncome()
    {
        yield return new WaitForSeconds(3);
        OnClick();
        StartCoroutine(PassiveIncome());
    }

    public void UpdateText()
    {
        GetMoneyAmount();
        try
        {
            text.text = Price_Display.Instance.Price_To_Text(amount * (isDoubledPeople ? 2 : 1) * (CurrencyManager.InstancePeople.isX4People ? 4 : 1));
            if (currencyType == CurrencyTypes.Money)
            {
                if (Saves.Instance.LoadExtra("shopsTutorial") == 0 && amount >= 40)
                {
                    EducationPanel.Instance.SetEducation(4, firstShop, 2, agesArrow, ArrowPosition.Shop,
                        ArrowPosition.Eras);
                    Saves.Instance.SaveExtra("shopsTutorial", 1);
                }
                else if (Saves.Instance.LoadExtra("donateTutorial") == 0 && amount >= 1000)
                {
                    EducationPanel.Instance.SetEducation(0, donateArrow, 3, goodLuck, ArrowPosition.Donate);
                    Saves.Instance.SaveExtra("donateTutorial", 1);
                }
            }
        }
        catch (NullReferenceException e) // Price_to_text is not initialized yet
        {
            text.text = "0";
        }
    }

    public void OnClick()
    {
        clickSoundEmiter.Play();
        AddMoneyAmount(TotalMoneyPerClick * (isX2Coins ? 2 : 1) );
    }
    
    public void AddMoneyAmount(long num)
    {
        Saves.Instance.SaveExtra(currencyPlayerPrefsName, amount + num);
        ballAnimation.PlayAnimation();
        Popup_Score_OnClick.Instance.PopUp(num);
        amount += num;
        UpdateText();
    }

    public IEnumerator x2People()
    {
        isDoubledPeople = true;
        UpdateText();
        yield return new WaitForSeconds(60 * 7);
        isDoubledPeople = false;
        UpdateText();
    }
    
    public void StartX4People()
    {
        isX4People = true;
        UpdateText();
    }
    
    public void DisableX4People()
    {
        isX4People = false;
        UpdateText();
    }
    
    public void StartX2Coins()
    {
        isX2Coins = true;
    }
    
    public void DisableX2Coins()
    {
        isX2Coins = false;
    }

    public void GetMoneyAmount()
    {
        amount = Saves.Instance.LoadExtra(currencyPlayerPrefsName);
    }

    private void GetMoneyPerClick()
    {
        TotalMoneyPerClick = Saves.Instance.LoadExtra(totalMoneyPerClickPlayerPrefsName, 1);
    }

    public void AddMoneyPerClick(long num)
    {
        TotalMoneyPerClick += num;
        Saves.Instance.SaveExtra(totalMoneyPerClickPlayerPrefsName, TotalMoneyPerClick);
    }

    public void AddPeople(long num)
    {
        amount += num;
        Saves.Instance.SaveExtra(currencyPlayerPrefsName, amount);
        UpdateText();
    }
}
