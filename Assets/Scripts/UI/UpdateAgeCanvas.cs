using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements; // Import Unity Ads

public class UpdateAgeCanvas : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Text text;
    public static UpdateAgeCanvas Instance { get; private set; }
    private Animator _animator;

    public void Initialize()
    {
        Instance = this;
        _animator = GetComponent<Animator>();
    }

    // Method to check if an interstitial ad should be shown and load it
    public void NewLvl(Sprite img, string name)
    {
        int level = AgeManager.Instance.GetLevel();

        // Check if the level is odd and not 0
        if (level % 2 == 0 && level != 0)
        {
            if (Saves.Instance.LoadExtra("remove_ads") == 0) PanelManager.Instance.ShowInterstitialAd(); // Show the interstitial ad before displaying the new level
        }

        // Proceed with showing the new level details on the canvas
        gameObject.SetActive(true);
        _animator.SetTrigger("Start");
        image.sprite = img;
        text.text = name;
        StartCoroutine(Disappear());
    }

    // Coroutine to hide the canvas after a delay
    private IEnumerator Disappear()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }
}
