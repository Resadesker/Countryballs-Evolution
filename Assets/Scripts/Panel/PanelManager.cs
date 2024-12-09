using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private Panel[] panels;
    [SerializeField] private Image[] buttons; // for UI fade in animations
    
    private int currentPanel; // it is = index of "panels"
    
    public static PanelManager Instance { get; private set; }
    
    // TODO: Move initializing to a GameManager class
    [SerializeField] private JobsLayoutConstructor jobsLayoutConstructor;
    [SerializeField] private AgeListConstructor _ageListConstructor;
    [SerializeField] private UpdateAgeCanvas _updateAgeCanvas;
    [SerializeField] private RewardChestPanel _rewardChestPanel;
    [SerializeField] private ShopConstructor _shopConstructor;
    [SerializeField] private SkinSale _skinSale;

    [SerializeField] private EducationPanel _educationPanel;
    
    public Country country;
    
    [SerializeField] private LocalizedString firstJoinText;
    [SerializeField] private LocalizedString firstJoinText2;
    
    private string interstitialAdPlacementId;
    private const string interstitialAdPlacementIdAndroid = "Interstitial_Android";
    private const string interstitialAdPlacementIdiOS = "Interstitial_iOS";
    
    public string rewardedAdPlacementId;
    private const string rewardedAdPlacementIdAndroid = "Rewarded_Android"; // Replace with your Unity Ads placement ID
    private const string rewardedAdPlacementIdIOS = "Rewarded_iOS"; // Replace with your iOS placement ID

    private void Awake()
    {
        Instance = this;
#if UNITY_IOS || UNITY_EDITOR
        rewardedAdPlacementId = rewardedAdPlacementIdIOS;
#elif UNITY_ANDROID
        rewardedAdPlacementId = rewardedAdPlacementIdAndroid;
#endif
    }

    private void Start()
    {
        for (int i = 1; i < panels.Length; i++)
        {
            SwitchPanel(i);
        }
        SwitchPanel(0);
        
        jobsLayoutConstructor.Initialize();
        _ageListConstructor.Initialize();
        _educationPanel.Initialize();
        _updateAgeCanvas.Initialize();
        // _rewardChestPanel.Initialize();
        _shopConstructor.Initialize();
        _skinSale.Initialize();
        CurrencyManager.InstanceClicker.GetMoneyAmount();
        CurrencyManager.InstancePeople.GetMoneyAmount();
        CurrencyManager.InstanceClicker.UpdateText();
        CurrencyManager.InstancePeople.UpdateText();
        
#if UNITY_IOS || UNITY_EDITOR
        interstitialAdPlacementId = interstitialAdPlacementIdiOS;
        Advertisement.Initialize("5694542", false); // Replace with your Game ID
#elif UNITY_ANDROID
        interstitialAdPlacementId = interstitialAdPlacementIdAndroid;
        Advertisement.Initialize("5694543", false); // Replace with your Game ID
#endif
        LoadInterstitialAd();
        LoadRewardedAd();

        StartCoroutine(WaitForFirstEducation());
    }

    private IEnumerator WaitForFirstEducation()
    {
        yield return new WaitForSeconds(3);
        if (Saves.Instance.LoadExtra("firstJoin") == 0)
        {
            _educationPanel.SetEducation(0, firstJoinText, 1, firstJoinText2);
            Saves.Instance.SaveExtra("firstJoin", 1);
        }
    }

    public int GetCurrentPanel()
    {
        return currentPanel;
    }

    public void SwitchPanel(int index)
    {
        try
        {
            if (index == 1) ShopConstructor.Instance.OpenMenu();
            else if (index == 2) JobsLayoutConstructor.Instance.OpenMenu();
            else if (index == 3) AgeListConstructor.Instance.OpenMenu();
        } catch (Exception) {}

        panels[currentPanel].ChangePanelState(false);
        currentPanel = index;
        panels[index].ChangePanelState(true);
        ColorButton(index);
        try
        {
            UpgradeManager.Instance.OnBuildingDeselected();
        }
        catch (NullReferenceException)
        {
            // ignore if not yet initialized
        }
    }

    private void ColorButton(int number)
    {
        foreach (Image button in buttons)
        {
            if (button != buttons[number]) button.color = Color.white;
        }

        buttons[number].color = new Color32(195, 195, 195, 255);
    }
    
    public IEnumerator WaitForLocalesLoad(int savedLanguageIndex)
    {
        yield return new WaitForSeconds(0.5f);
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[savedLanguageIndex];
        LanguageSelector.Instance.NotifySubscribers();
    }

    // Map is panels[0]
    // The function is normally used when a building is built, so you see the new building
    public void GoToMap()
    {
        SwitchPanel(0);
        // Highlight the building
        ColorButton(0);
    }
    
    
    // ---BELOW---
    // -----------
    // EVERYTHING for Ads
    private void LoadRewardedAd()
    {
        // Load the rewarded ad
        Advertisement.Load(rewardedAdPlacementId, this);
    }
    
    private void LoadInterstitialAd()
    {
        Advertisement.Load(interstitialAdPlacementId, this);
    }

    public void ShowInterstitialAd()
    {
        if (Advertisement.isInitialized)
        {
            Advertisement.Show(interstitialAdPlacementId);
        }
        else
        {
            Debug.LogError( "Interstitial ad is not ready.");
        }
    }

    // IUnityAdsShowListener implementation
    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId == interstitialAdPlacementId && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.Log("Interstitial ad completed.");
        }
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Failed to show interstitial ad for {placementId}: {message}");
    }

    public void OnUnityAdsShowStart(string placementId) { }
    public void OnUnityAdsShowClick(string placementId) { }

    // IUnityAdsLoadListener implementation (optional)
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"Interstitial ad loaded for {placementId}");
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Failed to load interstitial ad for {placementId}: {message}");
    }
}
