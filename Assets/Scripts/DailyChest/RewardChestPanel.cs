using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class RewardChestPanel : MonoBehaviour, IUnityAdsShowListener, IUnityAdsLoadListener
{
    [SerializeField] private Image chest;
    [SerializeField] private Sprite[] chestSprites; // 0 = normal closed; 1 = rewarded ad closed; 2 = normal open; 3 = rewarded open
    [SerializeField] private Animator chestAnimation;
    [SerializeField] private SoundEmitter soundEmitter; // chest open sound
    [SerializeField] private SoundEmitter failedSoundEmitter; // failed to load ad sound
    [Space]
    [SerializeField] private Animator bonusAnimator;
    [SerializeField] private Image bonus;
    [SerializeField] private Sprite[] bonusSprites; // 0 = coin; 1 = clock;
    [Space] 
    [SerializeField] private TMP_Text text;
    [SerializeField] private TMP_Text x2text;
    [SerializeField] private DailyChest dailyChest;

    private int currentChestIndex;
    private int currentReward; // index of bonusSprites
    
    private bool _canBeDeactivated = false;

    public void OpenChest(bool isRewardedChest)
    {
        if (isRewardedChest)
        {
            if (Saves.Instance.LoadExtra("remove_ads") == 0) ShowRewardedAd(); // Only show the rewarded ad, do not open the chest until ad is watched
            else
            {
                currentChestIndex = 1; // Set to the rewarded chest sprite
                ActivateChestPanel();
            }
        }
        else
        {
            // Directly show the chest if it's not a rewarded ad chest
            currentChestIndex = 0; // Normal chest
            ActivateChestPanel();
        }
    }

    private void ShowRewardedAd()
    {
        if (Advertisement.isInitialized)
        {
            Advertisement.Show(PanelManager.Instance.rewardedAdPlacementId, this); // Show the rewarded ad and wait for completion
        }
        else
        {
            failedSoundEmitter.Play();
            Debug.LogWarning("Rewarded ad is not ready...");
        }
    }

    // Only activate and show the chest panel after the ad has been successfully watched
    private void ActivateChestPanel()
    {
        _canBeDeactivated = false;
        gameObject.SetActive(true);
        bonusAnimator.SetTrigger("Start");
        chest.sprite = chestSprites[currentChestIndex]; // Update to the correct chest sprite
        chestAnimation.SetTrigger("Zoom");
        x2text.enabled = false;
        StartCoroutine(ChestOpenAnimation());
    }

    private IEnumerator ChestOpenAnimation()
    {
        soundEmitter.Play();
        yield return new WaitForSeconds(1);
        chest.sprite = chestSprites[currentChestIndex + 2]; // Open chest sprite (2 for normal, 3 for rewarded)
        bonusAnimator.SetTrigger("Open");
        bonusAnimator.ResetTrigger("Start");
        GenerateReward();
        
        yield return new WaitForSeconds(1);
        _canBeDeactivated = true;
    }

    private void GenerateReward()
    {
        do
        {
            currentReward = Random.Range(0, 3);
        } while (CurrencyManager.InstancePeople.isDoubledPeople && currentReward == 1);

        bonus.sprite = bonusSprites[currentReward];
        switch (currentReward)
        {
            case 0:
                long moneyReward = CurrencyManager.InstanceClicker.TotalMoneyPerClick * 250 * Random.Range(5, 11);
                text.text = Price_Display.Instance.Price_To_Text(moneyReward);
                CurrencyManager.InstanceClicker.AddMoneyAmount(moneyReward);
                break;
            case 1:
                text.text = "7min";
                x2text.enabled = true;
                x2text.text = "x2";
                CurrencyManager.InstancePeople.StartCoroutine(CurrencyManager.InstancePeople.x2People());
                break;
            case 2:
                text.text = "60s";
                x2text.enabled = true;
                x2text.text = "x2";
                break;
        }
    }

    private void Update()
    {
        if (gameObject.activeSelf && _canBeDeactivated)
        {
            if (Input.anyKeyDown)
            {
                gameObject.SetActive(false);

                if (currentReward == 2)
                {
                    Clock_UI.Instance.Launch(true);
                    Booster_Afterlife.Instance.StartCoroutine(Booster_Afterlife.Instance.OnSpeedUp(null, 60, 2));
                }
            }
        }
    }

    // IUnityAdsShowListener implementation
    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId == PanelManager.Instance.rewardedAdPlacementId && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            dailyChest.DisableChestOnOpen();
            Debug.Log("Ad successfully watched.");
            currentChestIndex = 1; // Set to the rewarded chest sprite
            ActivateChestPanel(); // Only activate the chest panel if ad is successfully watched
        }
        else
        {
            Debug.Log("Ad skipped or failed.");
            // Optionally handle cases where ad wasn't fully watched
        }
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Failed to show ad for {placementId}: {message}");
    }

    public void OnUnityAdsShowStart(string placementId) { }
    public void OnUnityAdsShowClick(string placementId) { }

    // IUnityAdsLoadListener implementation (optional)
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"Ad loaded for {placementId}");
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Failed to load ad for {placementId}: {message}");
    }
}
