using TMPro;
using UnityEngine;

public class Popup_Particles_UI : MonoBehaviour
{
    [SerializeField] private GameObject popup;
    [SerializeField] private int amount = 20;
    
    public void OnClick()
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject dp = Instantiate(popup, transform.position, Quaternion.identity);
            dp.transform.parent = (amount == 20) ? gameObject.transform.parent.parent : gameObject.transform.parent;
        }
        // dp.transform.localScale = new Vector3(1, 1, 1);
    }
}
