using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Include TextMeshPro for text fields
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class SkinSale : MonoBehaviour, IDetailedStoreListener
{
    [SerializeField] private BallAnimation ballAnimation;
    [SerializeField] private TMP_Text cyborgSkinCostText; // Cost text for Cyborg Skin
    [SerializeField] private TMP_Text philosopherSkinCostText; // Cost text for Philosopher Skin
    [SerializeField] private TMP_Text removeAdsPriceText; // Price text for removing ads
    [SerializeField] private GameObject removeAdsButton; // Button for removing ads

    private IStoreController storeController;
    private IExtensionProvider extensionProvider;

    // IDs for skins and remove ads
    private const string CyborgSkinId = "cyborg_skin";
    private const string PhilosopherSkinId = "philosoph_skin";
    private const string RemoveAdsId = "remove_ads";
    
    private int initializationAttempts = 0;
    private const int maxInitializationAttempts = 5;
    private const float retryDelay = 2.0f;

    public void Initialize()
    {
        InitializePurchasing();

        // Check if ads are already removed and hide the button if necessary
        if (Saves.Instance.LoadExtra(RemoveAdsId) == 1)
        {
            removeAdsButton.SetActive(false); // Hide the button if ads are removed
        }
    }

    private void InitializePurchasing()
    {
        if (storeController != null) return; // IAP is already initialized

        // Builder for IAP initialization
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add skin products and remove ads
        builder.AddProduct(CyborgSkinId, ProductType.NonConsumable);
        builder.AddProduct(PhilosopherSkinId, ProductType.NonConsumable);
        builder.AddProduct(RemoveAdsId, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }

    public void BuySkin(int skin)
    {
        // Check if the skin is already bought
        if (skin == 0)
        {
            EquipSkin(skin);
        }
        else if (skin == 1 && Saves.Instance.LoadExtra(CyborgSkinId) == 1)
        {
            EquipSkin(skin);
            cyborgSkinCostText.gameObject.SetActive(false); // Hide cost text if bought
        }
        else if (skin == 2 && Saves.Instance.LoadExtra(PhilosopherSkinId) == 1)
        {
            EquipSkin(skin);
            philosopherSkinCostText.gameObject.SetActive(false); // Hide cost text if bought
        }
        else
        {
            // Trigger purchase if skin is not bought
            PurchaseSkin(skin);
        }
    }

    private void PurchaseSkin(int skin)
    {
        string productId = skin == 1 ? CyborgSkinId : PhilosopherSkinId;

        if (storeController != null && storeController.products.WithID(productId) != null)
        {
            storeController.InitiatePurchase(productId);
        }
        else
        {
            Debug.LogError("Purchase failed: product or store controller is null.");
        }
    }

    private void EquipSkin(int skin)
    {
        CurrencyManager.InstancePeople.DisableX4People();
        CurrencyManager.InstanceClicker.DisableX2Coins();

        switch (skin)
        {
            case 0: // Default skin, no purchase required
                break;
            case 1: // Cyborg skin
                CurrencyManager.InstancePeople.StartX4People();
                break;
            case 2: // Philosopher skin
                CurrencyManager.InstanceClicker.StartX2Coins();
                break;
        }

        ballAnimation.SaveSkinAnimator(skin);
    }

    // Remove Ads Purchase Integration
    public void BuyRemoveAds()
    {
        if (Saves.Instance.LoadExtra(RemoveAdsId) == 1)
        {
            removeAdsButton.SetActive(false); // Hide the button if ads are already removed
        }
        else
        {
            // Trigger purchase
            if (storeController != null && storeController.products.WithID(RemoveAdsId) != null)
            {
                storeController.InitiatePurchase(RemoveAdsId);
            }
            else
            {
                Debug.LogError("Purchase failed: product or store controller is null.");
            }
        }
    }

    private void UpdateSkinCostText(string skinId, TMP_Text skinCostText)
    {
        if (Saves.Instance.LoadExtra(skinId) == 0) // If skin is not bought
        {
            Product product = storeController.products.WithID(skinId);
            if (product != null)
            {
                skinCostText.text = product.metadata.localizedPriceString; // Display price with currency
                skinCostText.gameObject.SetActive(true);
            }
        }
        else
        {
            skinCostText.gameObject.SetActive(false); // Hide text if already bought
        }
    }

    private void UpdateRemoveAdsCostText()
    {
        if (Saves.Instance.LoadExtra(RemoveAdsId) == 0) // If ads are not removed
        {
            Product product = storeController.products.WithID(RemoveAdsId);
            if (product != null)
            {
                removeAdsPriceText.text = product.metadata.localizedPriceString; // Display price with currency
                removeAdsPriceText.gameObject.SetActive(true);
            }
        }
        else
        {
            removeAdsPriceText.gameObject.SetActive(false); // Hide text if already purchased
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError($"Purchase failed: {product.definition.id}, Reason: {failureReason}");
    }
    
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        extensionProvider = extensions;

        UpdateSkinCostText(CyborgSkinId, cyborgSkinCostText);
        UpdateSkinCostText(PhilosopherSkinId, philosopherSkinCostText);
        UpdateRemoveAdsCostText();

        Debug.Log("IAP initialization successful");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogError($"IAP Purchase Failed: {failureDescription}");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"IAP Initialization Failed: {error}. Message: {message}");

        // Recursive re-initialization attempt
        if (initializationAttempts < maxInitializationAttempts)
        {
            initializationAttempts++;
            Debug.LogWarning($"Retrying IAP initialization in {retryDelay} seconds (Attempt {initializationAttempts}/{maxInitializationAttempts})...");
            StartCoroutine(RetryInitializePurchasing());
        }
        else
        {
            Debug.LogError("Maximum IAP initialization attempts reached. Initialization failed.");
        }
    }
    
    private IEnumerator RetryInitializePurchasing()
    {
        yield return new WaitForSeconds(retryDelay);
        InitializePurchasing();
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"IAP Initialization Failed: {error}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        string purchasedProductId = args.purchasedProduct.definition.id;

        // Handle skin purchase
        if (purchasedProductId == CyborgSkinId)
        {
            Saves.Instance.SaveExtra(CyborgSkinId, 1);
            EquipSkin(1); // Equip the cyborg skin
            cyborgSkinCostText.gameObject.SetActive(false); // Hide the price after purchase
        }
        else if (purchasedProductId == PhilosopherSkinId)
        {
            Saves.Instance.SaveExtra(PhilosopherSkinId, 1);
            EquipSkin(2); // Equip the philosopher skin
            philosopherSkinCostText.gameObject.SetActive(false); // Hide the price after purchase
        }
        // Handle "Remove Ads" purchase
        else if (purchasedProductId == RemoveAdsId)
        {
            Saves.Instance.SaveExtra(RemoveAdsId, 1); // Save the "Remove Ads" purchase
            removeAdsButton.SetActive(false); // Hide the button after purchase
            removeAdsPriceText.gameObject.SetActive(false); // Hide the price after purchase
        }

        return PurchaseProcessingResult.Complete;
    }
}
