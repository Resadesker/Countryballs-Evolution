using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Price_Display : MonoBehaviour
{
    public static Price_Display Instance { get; private set; }
    
    private void Awake()
    {
        Instance = this;
    }

    public string Price_To_Text(long price)
    {
        string priceText;
        
        if (price >= 1000000000000000)
        {
            double value = price / 1000000000000000.0;
            priceText = value >= 100 ? $"{(int)value}Q" : $"{Math.Floor(value * 100) / 100:0.##}Q";
        }
        else if (price >= 1000000000000)
        {
            double value = price / 1000000000000.0;
            priceText = value >= 100 ? $"{(int)value}T" : $"{Math.Floor(value * 100) / 100:0.##}T";
        }
        else if (price >= 1000000000)
        {
            double value = price / 1000000000.0;
            priceText = value >= 100 ? $"{(int)value}B" : $"{Math.Floor(value * 100) / 100:0.##}B";
        }
        else if (price >= 1000000)
        {
            double value = price / 1000000.0;
            priceText = value >= 100 ? $"{(int)value}M" : $"{Math.Floor(value * 100) / 100:0.##}M";
        }
        else if (price >= 1000)
        {
            double value = price / 1000.0;
            priceText = value >= 100 ? $"{(int)value}K" : $"{Math.Floor(value * 100) / 100:0.##}K";
        }
        else
        {
            priceText = price.ToString();
        }

        return priceText;
    }


}
