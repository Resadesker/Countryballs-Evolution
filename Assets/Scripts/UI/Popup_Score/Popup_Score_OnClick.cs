using System;
using TMPro;
using UnityEngine;

public class Popup_Score_OnClick : MonoBehaviour
{
    public static Popup_Score_OnClick Instance { get; private set; }
    [SerializeField] private TMP_Text damagePopup;
    
    void Start()
    {
        Instance = this;
    }

    public void PopUp(long num)
    {
        Create(num);
    }
    
    public void Create(long damageAmount) {
        TMP_Text dp = Instantiate(damagePopup, transform.position, Quaternion.identity);
        dp.transform.parent = gameObject.transform.parent.parent;
        dp.transform.localScale = new Vector3(1, 1, 1);
        
        dp.text = (damageAmount > 0 ? "+" : "-") + Price_Display.Instance.Price_To_Text(Math.Abs(damageAmount));
    }
}
