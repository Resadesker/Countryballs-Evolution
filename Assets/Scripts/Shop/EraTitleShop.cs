using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EraTitleShop : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    private EraInList era;

    private void Start()
    {
        LanguageSelector.Instance.OnLanguageChange += UpdateLocalizedTexts;
    }

    private void UpdateLocalizedTexts()
    {
        label.text = LanguageSelector.Instance.GetLocalizedString(era.title);
    }

    public void SetEraTitle(EraInList newEra)
    {
        era = newEra;
        UpdateLocalizedTexts();
    }
}
