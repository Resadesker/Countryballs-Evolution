using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Localization.PropertyVariants;
using UnityEngine.SceneManagement;

public class LanguageSelector : MonoBehaviour
{
    public TMP_Dropdown languageDropdown;
    public static LanguageSelector Instance { get; private set; }
    public event Action OnLanguageChange;

    private int savedLanguageIndex = -1; // Default to -1, meaning no language has been selected yet
    private List<GameObjectLocalizer> cachedLocalizers = new List<GameObjectLocalizer>();

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        Instance = this;

        // Set the dropdown options
        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(new List<string> { "English", "Deutsch", "Русский", "Español" });

        // Check if a language has been saved, defaulting to -1 if not found
        savedLanguageIndex = (int) Saves.Instance.LoadExtra("SelectedLanguage", -1);
        
        if (savedLanguageIndex == -1)
        {
            // No language saved, use the system language to initialize
            Locale systemLocale = LocalizationSettings.SelectedLocale; // Get system locale
            savedLanguageIndex = GetLanguageIndexFromLocale(systemLocale);
            
            // Save the system language for future use
            Saves.Instance.SaveExtra("SelectedLanguage", savedLanguageIndex);
        }

        // Set the dropdown to match the selected/saved language
        languageDropdown.value = savedLanguageIndex;

        // Set the initial language based on the saved or system locale index
        PanelManager.Instance.StartCoroutine(PanelManager.Instance.WaitForLocalesLoad(savedLanguageIndex));

        // Add listener to handle dropdown value change
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    // Function to refresh all localizers (including inactive GameObjects)
    private void RefreshAllLocalizers()
    {
        cachedLocalizers.Clear(); // Clear the cached list of localizers

        // Loop through all loaded scenes
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.isLoaded)
            {
                // Get root GameObjects in the scene
                GameObject[] rootObjects = scene.GetRootGameObjects();

                // Recursively find and cache all GameObjectLocalizers
                foreach (GameObject rootObject in rootObjects)
                {
                    FindLocalizersRecursively(rootObject);
                }
            }
        }

        // Apply the selected locale to all cached GameObjectLocalizers
        Locale selectedLocale = LocalizationSettings.SelectedLocale;
        foreach (GameObjectLocalizer localizer in cachedLocalizers)
        {
            if (localizer != null)
            {
                localizer.ApplyLocaleVariant(selectedLocale);
            }
        }
    }

    // Recursive function to find GameObjectLocalizers, even in inactive GameObjects
    private void FindLocalizersRecursively(GameObject obj)
    {
        // Check if the object has a GameObjectLocalizer component
        GameObjectLocalizer localizer = obj.GetComponent<GameObjectLocalizer>();
        if (localizer != null)
        {
            cachedLocalizers.Add(localizer);
        }

        // Recursively search child objects
        foreach (Transform child in obj.transform)
        {
            FindLocalizersRecursively(child.gameObject);
        }
    }

    private void OnLanguageChanged(int index)
    {
        SetLanguage(index);
        // Save the selected language index
        Saves.Instance.SaveExtra("SelectedLanguage", index);
    }

    // Notify subscribers of language change
    public void NotifySubscribers()
    {
        OnLanguageChange?.Invoke();
    }

    private void SetLanguage(int index)
    {
        switch (index)
        {
            case 0:
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0]; // English
                break;
            case 1:
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1]; // German
                break;
            case 2:
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[2]; // Russian
                break;
            case 3:
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[3]; // Spanish
                break;
        }

        NotifySubscribers();
        
        RefreshAllLocalizers(); // Refresh all localizers after changing the language

        Debug.Log($"Language changed to index: {index}");
    }

    // Function to match system locale with the corresponding dropdown index
    private int GetLanguageIndexFromLocale(Locale locale)
    {
        if (locale.Identifier.Code == "en")
        {
            return 0; // English
        }
        else if (locale.Identifier.Code == "de")
        {
            return 1; // German
        }
        else if (locale.Identifier.Code == "ru")
        {
            return 2; // Russian
        }
        else if (locale.Identifier.Code == "es")
        {
            return 3; // Spanish
        }

        return 0; // Default to English if no match is found
    }

    public string GetLocalizedString(LocalizedString str)
    {
        // Get the localized string and print it
        var localizedStringHandle = str.GetLocalizedString();
        return localizedStringHandle;
    }
}
