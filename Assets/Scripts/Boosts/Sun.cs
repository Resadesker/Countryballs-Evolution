using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class Sun : MonoBehaviour
{
    [SerializeField] private LocalizedString firstSunText;
    public SoundEmitter soundEmitter;
    
    void Start()
    {
        if (Saves.Instance.LoadExtra("firstSun") == 0 && PanelManager.Instance.GetCurrentPanel() == 0)
        {
            EducationPanel.Instance.SetEducation(0, firstSunText);
            Saves.Instance.SaveExtra("firstSun", 1);
        }

        StartCoroutine(SelfDestruct(7));
        StartCoroutine(FadeOutAndDestroy());
    }
    
    public void OnClick()
    {
        CurrencyManager.InstanceClicker.AddMoneyAmount(CurrencyManager.InstancePeople.amount * (CurrencyManager.InstancePeople.isDoubledPeople ? 2 : 1) * (CurrencyManager.InstancePeople.isX4People ? 4 : 1));
        GetComponent<Popup_Particles_UI>().OnClick();
        soundEmitter.Play();
        StartCoroutine(SelfDestruct(0f));
    }
    
    private IEnumerator SelfDestruct(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
    
    IEnumerator FadeOutAndDestroy()
    {
        yield return new WaitForSeconds(6f);
        float elapsedTime = 0f;
        Color initialColor;
        float fadeDuration = 1;
        Image image = GetComponent<Image>();
        initialColor = image.color;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            image.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
