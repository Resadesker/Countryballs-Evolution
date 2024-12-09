using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class AgeManager : MonoBehaviour
{
    private Image avatar;

    public int currentAge;
    public int currentEra = 0;

    private string playerPrefsName = "Age";
    private string eraPlayerPrefsName = "Era"; // Era > Age

    public static AgeManager Instance { get; private set; }

    [SerializeField] private int maxHomesInAge;
    
    [SerializeField] private LocalizedString longNotSeenText;
    [SerializeField] private LocalizedString byeText;
    [SerializeField] private GameObject replayButton;
    [SerializeField] private SoundEmitter soundEmitter; // on new lvl sound
    
    private int currentHomesInAgeBuilt;

    private void Awake()
    {
        avatar = GetComponent<Image>();
        Instance = this;
    }
    
    private void Start()
    {
        currentAge = GetAge();
        currentEra = GetEra();
        ChangeAvatar(false);

        // // Select first age as current, if currentAge == 0 (not changed in DB)
        // AgeListConstructor.Instance.ShowAge(currentAge);
    }

    public void SetMaxHomesInAge(int amount)
    {
        maxHomesInAge = amount;
    }

    public void AddBuiltHome()
    {
        currentHomesInAgeBuilt++;
        
        if (currentHomesInAgeBuilt >= maxHomesInAge)
        {
            //print($"{currentHomesInAgeBuilt} >= {maxHomesInAge}");
            currentHomesInAgeBuilt = 0;
            SetNewLevel();
        }
    }

    private void SetNewLevel()
    {
        currentAge = GetAge();
        int currentLevel = GetLevel()+1;
        if (currentLevel >= 46) // cannot go to further level if reached maximum
        {
            // call ending education
            if (Saves.Instance.LoadExtra("gameEnded") == 0)
            {
                EducationPanel.Instance.SetEducation(0, longNotSeenText, 3, byeText);
                replayButton.SetActive(true);
                Saves.Instance.SaveExtra("gameEnded", 1);
            }
            return;
        }
        if (PanelManager.Instance.country.ages[currentEra].eras.Length == currentAge + 1)
        {
            currentAge = 0;
            SetNewEra();
        }
        else
        {
            Saves.Instance.SaveExtra(playerPrefsName, ++currentAge);
        }
        ChangeAvatar();
        ShopConstructor.Instance.UnlockAge(currentLevel, currentAge, currentEra);

        try
        {
            AgeListConstructor.Instance.ShowAge(GetLevel());
            soundEmitter.Play();
        }
        catch (Exception e)
        {
        }
        
        //BuildingManager.Instance.OnNewLevel();//StartCoroutine(WaitAndCheckForBuildingUpdates());
    }

    private void SetNewEra()
    {
        currentEra = GetEra();
        MusicManager.Instance.CheckForMusic(currentEra);
        Saves.Instance.SaveExtra(eraPlayerPrefsName, ++currentEra);
        Saves.Instance.SaveExtra(playerPrefsName, 0);
    }

    private void ChangeAvatar(bool newLevel = true)
    {
        int plus = GetEra() == 2 && GetAge() == 8 ? -1 : 0; // don't use the final age
        currentEra += plus;
        Sprite img = PanelManager.Instance.country.ages[currentEra].eras[currentAge].avatar;
        avatar.sprite = img;
        if (newLevel) UpdateAgeCanvas.Instance.NewLvl(img, 
            LanguageSelector.Instance.GetLocalizedString(
                PanelManager.Instance.country.ages[currentEra].eras[currentAge].title)
            );
    }
    
    public int[] Int_To_Era_And_Age(int num)
    {
        int[] arr = new int[2];
        arr[0] = 0;
        arr[1] = 0;
        while (num > PanelManager.Instance.country.ages[arr[0]].eras.Length)
        {
            num -= PanelManager.Instance.country.ages[arr[0]].eras.Length;
            arr[0]++;
        }
        
        arr[1] = num;

        return arr;
    }
    
    public int GetAge()
    {
        return (int) Saves.Instance.LoadExtra(playerPrefsName);
    }

    public int GetLevel(bool fromStart = false)
    {
        if (fromStart)
        {
            currentEra = GetEra();
            currentAge = GetAge();
        }
        
        int num = 0;
        for (int i = 0; i < currentEra; i++)
        {
            num += PanelManager.Instance.country.ages[i].eras.Length;
        }
        num += currentAge;
        return num;
    }
    
    public int GetEra()
    {
        return (int) Saves.Instance.LoadExtra(eraPlayerPrefsName);
    }
}