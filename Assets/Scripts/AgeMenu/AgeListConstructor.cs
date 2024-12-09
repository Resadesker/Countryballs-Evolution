using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgeListConstructor : MonoBehaviour
{
    [SerializeField] private AgeSmallAvatar _ageAvatarConstructor; // Small Sprite
    [SerializeField] private AgeAvatarConstructor _eraAvatarConstructor; // Big Sprite
    [SerializeField] private ScrollRect scrollRect; // Scroll rect for scrolling functionality
    [SerializeField] private List<Sprite> eraBackgrounds; // List of background sprites for each era
    [SerializeField] private RectTransform backgroundContainer; // Container for the background images
    [SerializeField] private Image backgroundPrefab; // Prefab for the background images

    private float totalHeight = -50;
    private RectTransform _rectTransform;
    private List<AgeSmallAvatar> _ageAvatars = new List<AgeSmallAvatar>();

    public static AgeListConstructor Instance { get; private set; }

    // Constants for heights
    private const float EraAvatarHeight = 333.4657f; // Height of era avatar
    private const float AgeAvatarHeight = 276.4325f;   // Height of age avatar
    private const float BackgroundHeight = 1080f * 2.5f; // Height of background image (assuming 1080p resolution)
    private const float spacingHeight = -18.11f;

    private float openAgeOnHeight; // Height to open the menu

    // Create all the avatars, launched on startup
    public void Initialize()
    {
        Instance = this;

        // Initialize the background offset so that the first background starts at the top of the scroll content
        float currentBackgroundOffset = -EraAvatarHeight ; // Adjust to start background at the beginning

        for (int u = 0; u < PanelManager.Instance.country.ages.Length; u++)
        {
            // Instantiate and add era avatars
            AgeAvatarConstructor avatarConstructor = Instantiate(_eraAvatarConstructor, transform);
            avatarConstructor.avatar.sprite = PanelManager.Instance.country.ages[u].avatar;
            totalHeight += EraAvatarHeight + spacingHeight; // Era avatar height

            // Instantiate and add background images
            if (u < eraBackgrounds.Count)
            {
                Image background = Instantiate(backgroundPrefab, backgroundContainer);
                background.sprite = eraBackgrounds[u];
                background.rectTransform.sizeDelta = new Vector2(background.rectTransform.sizeDelta.x, BackgroundHeight);
                background.rectTransform.anchoredPosition = new Vector2(0, currentBackgroundOffset);
                background.rectTransform.localScale = new Vector3(1, -1, 1);
                currentBackgroundOffset += BackgroundHeight - EraAvatarHeight + spacingHeight; // Update offset for the next background
            }

            for (int i = 0; i < PanelManager.Instance.country.ages[u].eras.Length; i++)
            {
                // Instantiate and add age avatars
                AgeSmallAvatar avatar = Instantiate(_ageAvatarConstructor, transform);
                avatar.avatar.sprite = PanelManager.Instance.country.ages[u].eras[i].avatar;
                _ageAvatars.Add(avatar);
                totalHeight += AgeAvatarHeight + spacingHeight; // Age height
            }
        }

        _rectTransform = GetComponent<RectTransform>();
        UpdateShopSize();

        ShowAge(AgeManager.Instance.GetLevel()); // Display the current age
        OpenMenu(); // Open menu at the correct height

        scrollRect.onValueChanged.AddListener(OnScrollValueChanged); // Add listener for scroll changes
    }

    private void UpdateShopSize()
    {
        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, totalHeight);
    }

    // Open the menu at the correct position based on the current age
    public void OpenMenu()
    {
        float contentHeight = scrollRect.content.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;

        // Calculate the open height from the bottom to the target age
        float targetHeight = CalculateOpenHeight(AgeManager.Instance.GetLevel());

        // Prevent dividing by zero in case of unexpected setup
        if (contentHeight <= viewportHeight)
        {
            scrollRect.verticalNormalizedPosition = 0.0f; // Position at the top if content fits
        }
        else
        {
            // Calculate the target normalized position
            float normalizedPosition = Mathf.Clamp01(targetHeight / (contentHeight - viewportHeight));
            scrollRect.verticalNormalizedPosition = normalizedPosition; // Invert position for bottom to top
        }

        OnScrollValueChanged(new Vector2(0, targetHeight));
    }

    // Select Age in age menu as "current" (white outline)
    public void ShowAge(int age)
    {
        if (age != 0)
            _ageAvatars[age - 1].outline.SetActive(false);
        _ageAvatars[age].outline.SetActive(true);

        // Calculate open height for the selected age
        CalculateOpenHeight(age);
    }

    // Calculate the open height based on the current age
    private float CalculateOpenHeight(int age)
    {
        openAgeOnHeight = 0;

        // Calculate the total height to the current age from the bottom
        for (int i = 0; i < age; i++)
        {
            openAgeOnHeight += AgeAvatarHeight;// + spacingHeight; // Add height for each age avatar up to the current age
            if (i < PanelManager.Instance.country.ages.Length)
            {
                openAgeOnHeight += EraAvatarHeight; // + spacingHeight; // Add height for each era avatar
            }
        }

        return openAgeOnHeight - AgeAvatarHeight * 5;
    }

    // Update background positions when scrolling
    private void OnScrollValueChanged(Vector2 position)
    {
        float contentHeight = scrollRect.content.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;
        float scrollOffset = scrollRect.verticalNormalizedPosition * (contentHeight - viewportHeight);

        for (int i = 0; i < backgroundContainer.childCount; i++)
        {
            RectTransform background = (RectTransform)backgroundContainer.GetChild(i);
            background.anchoredPosition = new Vector2(0, -(i * BackgroundHeight) + scrollOffset);
        }
    }
}
