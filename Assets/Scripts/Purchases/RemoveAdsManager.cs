using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // For TextMeshPro text
using UnityEngine.Purchasing;

public class RemoveAdsManager : MonoBehaviour, IStoreListener
{
    [SerializeField] private GameObject removeAdsButton; // Button to remove ads
    [SerializeField] private TMP_Text removeAdsPriceText; // Display price for "Remove Ads"

    private IStoreController storeController;
    private IExtensionProvider extensionProvider;

    private const string RemoveAdsId = "remove_ads"; // Ensure this ID matches the one in the IAP Catalog

    private void Start()
    {
        InitializePurchasing();

        // Check if ads are already removed
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

        // Add product for removing ads
        builder.AddProduct(RemoveAdsId, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
        // StandardPurchasingModule.Instance().useFakeStoreAlways = true; // For testing purposes
    }

    public void BuyRemoveAds()
    {
        // Check if ads are already removed
        if (Saves.Instance.LoadExtra(RemoveAdsId) == 1)
        {
            removeAdsButton.SetActive(false); // Hide the button if ads are already removed
        }
        else
        {
            // Trigger purchase
            PurchaseRemoveAds();
        }
    }

    private void PurchaseRemoveAds()
    {
        if (storeController != null && storeController.products.WithID(RemoveAdsId) != null)
        {
            Debug.Log("Initiating purchase for Remove Ads.");
            storeController.InitiatePurchase(RemoveAdsId);
        }
        else
        {
            Debug.LogError("Purchase failed: Product or Store Controller is null.");
        }
    }

    // IStoreListener methods
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        extensionProvider = extensions;

        Debug.Log("IAP initialization successful.");

        // Display the price for removing ads
        UpdateRemoveAdsPriceText();
    }

    private void UpdateRemoveAdsPriceText()
    {
        if (Saves.Instance.LoadExtra(RemoveAdsId) == 0) // If ads are not removed
        {
            Product product = storeController.products.WithID(RemoveAdsId);
            if (product != null)
            {
                removeAdsPriceText.text = product.metadata.localizedPriceString; // Display price
                removeAdsPriceText.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("Remove Ads product not found in the store.");
            }
        }
        else
        {
            removeAdsPriceText.gameObject.SetActive(false); // Hide text if already purchased
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"IAP Initialization Failed: {error}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError($"Purchase failed: {product.definition.id}, Reason: {failureReason}");
    }
    
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"IAP Initialization Failed: {error}. Message: {message}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        string purchasedProductId = args.purchasedProduct.definition.id;

        if (purchasedProductId == RemoveAdsId)
        {
            Debug.Log("Remove Ads purchase successful.");
            Saves.Instance.SaveExtra(RemoveAdsId, 1); // Save the "Remove Ads" purchase
            removeAdsButton.SetActive(false); // Hide the button after purchase
        }

        return PurchaseProcessingResult.Complete;
    }
}
