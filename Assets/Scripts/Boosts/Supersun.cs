using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Supersun : MonoBehaviour
{
    [SerializeField] private GameObject sun;
    [SerializeField] private Image image;
    private bool activated = false;
    
    void Start()
    {
        StartCoroutine(SelfDestruct());
        StartCoroutine(FadeOutAndDestroy());
    }
    
    public void OnClick()
    {
        if (activated) return;
        activated = true;
        image.sprite = null;
        CurrencyManager.InstanceClicker.AddMoneyAmount(CurrencyManager.InstancePeople.amount);
        GetComponent<Popup_Particles_UI>().OnClick();
        
        StartCoroutine(SpawnSuns());
        
        Color initialColor = image.color;
        image.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0);
    }

    private IEnumerator SpawnSuns()
    {
        for (int i = 0; i < Random.Range(5, 25); i++)
        {
            GameObject sunInstance = Instantiate(sun);
            sunInstance.transform.parent = transform.parent;
            sunInstance.transform.localPosition = new Vector3((float) Random.Range(-2740, 2740) / 10, 715, 0);
            yield return new WaitForSeconds(((float) Random.Range(0, 10)) / 10);
        }
        
        Destroy(gameObject);
    }
    
    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(7);
        Destroy(gameObject);
    }
    
    IEnumerator FadeOutAndDestroy()
    {
        yield return new WaitForSeconds(6f);
        if (activated) yield break;
        float elapsedTime = 0f;
        Color initialColor;
        float fadeDuration = 1;
        initialColor = image.color;
        while (elapsedTime < fadeDuration)
        {
            if (activated) yield break;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            image.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        activated = true;

        yield return new WaitForSeconds(13);
        Destroy(gameObject);
    }
}