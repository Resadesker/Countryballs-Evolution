using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyCounter : MonoBehaviour
{
    private Text moneyText;
    
    void Start()
    {
        moneyText = GetComponent<Text>();
    }

    // This SHOULDN'T be in update, but it is for testing purposes. FIX IT!
    void Update()
    {
        moneyText.text = Saves.Instance.LoadExtra(Types.moneyPlayerPrefs).ToString(); 
        //PlayerPrefs.GetInt(Types.moneyPlayerPrefs).ToString();
    }

    public void OnClickerClick()
    {
        Saves.Instance.SaveExtra(Types.moneyPlayerPrefs, Saves.Instance.LoadExtra(Types.moneyPlayerPrefs) + 1);
        // PlayerPrefs.SetInt(Types.moneyPlayerPrefs, PlayerPrefs.GetInt(Types.moneyPlayerPrefs) + 1);
    }
}
