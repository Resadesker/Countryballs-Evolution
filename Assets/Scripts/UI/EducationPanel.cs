using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public enum ArrowPosition
{
    None,
    Shop,
    Donate,
    Eras
}

public class EducationPanel : MonoBehaviour
{
    public static EducationPanel Instance { get; private set; }
    
    [SerializeField] private Image ballImage;
    [SerializeField] private Sprite[] ballSprite;
    // 0 - speaking
    // 1 - playful
    // 2 - smiling
    // 3 - cute
    // 4 - MONEY!
    [SerializeField] private TMP_Text speechBubbleText;
    
    private int nextEmotionIndex = -1;
    private LocalizedString nextText = null;
    private ArrowPosition nextArrow;

    [SerializeField] private GameObject[] arrows; // 0 = shop; 1 = eras;
    
    private bool _canBeDeactivated = false;

    public void Initialize()
    {
        Instance = this;
    }
    
    public void SetEducation(int emotionIndex, LocalizedString text, int nextEmotion = -1, LocalizedString nextT = null, ArrowPosition arrowPosition1 = ArrowPosition.None, ArrowPosition arrowPosition2 = ArrowPosition.None)
    {
        _canBeDeactivated = false;
        gameObject.SetActive(true);
        ballImage.sprite = ballSprite[emotionIndex];
        speechBubbleText.text = LanguageSelector.Instance.GetLocalizedString(text);

        if (arrowPosition1 != ArrowPosition.None)
        {
            if (arrowPosition1 == ArrowPosition.Shop) arrows[0].SetActive(true);
            else if (arrowPosition1 == ArrowPosition.Eras) arrows[1].SetActive(true);
            else if (arrowPosition1 == ArrowPosition.Donate) arrows[2].SetActive(true);
        }

        nextEmotionIndex = nextEmotion;
        nextText = nextT;
        nextArrow = arrowPosition2;

        StartCoroutine(MakeDeactivatable());
    }

    private IEnumerator MakeDeactivatable()
    {
        yield return new WaitForSeconds(1.5f);
        _canBeDeactivated = true;
    }

    private void Update()
    {
        if (gameObject.activeSelf && _canBeDeactivated)
        {
            if (Input.anyKeyDown)
            {
                Hide();
            }
        }
    }

    private void Hide()
    {
        foreach (GameObject g in arrows)
        {
            g.SetActive(false);
        }
        
        if (nextEmotionIndex != -1)
        {
            SetEducation(nextEmotionIndex, nextText, arrowPosition1 : nextArrow);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
