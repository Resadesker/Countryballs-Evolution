using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster_Afterlife : MonoBehaviour
{
    public static Booster_Afterlife Instance { get; private set; }
    private long currentSpeedUp = 0;
    private int speedUpInstances = 0;

    private void Start()
    {
        Instance = this;
    }
    
    public IEnumerator OnSpeedUp(GameObject g, long speed, int multiplier)
    {
        speedUpInstances++;

        if (speedUpInstances > 1)
        {
            CurrencyManager.InstanceClicker.TotalMoneyPerClick /= currentSpeedUp;
        }
        
        currentSpeedUp = multiplier;
        CurrencyManager.InstanceClicker.TotalMoneyPerClick *= multiplier;
        yield return new WaitForSeconds(speed);
        speedUpInstances--;
        CurrencyManager.InstanceClicker.TotalMoneyPerClick /= multiplier;
        if (g != null) Destroy(g);
    }
}
