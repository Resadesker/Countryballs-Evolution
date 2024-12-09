using UnityEngine;
using UnityEngine.UI;

public class DynamicVerticalLayout : MonoBehaviour
{
    public VerticalLayoutGroup verticalLayoutGroup;
    public ScrollRect scrollRect;

    void Start()
    {
        // Call this whenever you add/remove elements to update the layout height
        UpdateVerticalLayoutHeight();
    }

    // Update the Vertical Layout Group height based on the content's preferred height
    void UpdateVerticalLayoutHeight()
    {
        float preferredHeight = CalculatePreferredHeight();
        SetVerticalLayoutHeight(preferredHeight);
    }

    // Calculate the preferred height of the content
    float CalculatePreferredHeight()
    {
        float totalHeight = 0f;
        int childCount = verticalLayoutGroup.transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            RectTransform child = verticalLayoutGroup.transform.GetChild(i) as RectTransform;
            totalHeight += LayoutUtility.GetPreferredHeight(child);
        }

        return totalHeight;
    }

    // Set the Vertical Layout Group's height
    void SetVerticalLayoutHeight(float height)
    {
        RectTransform rectTransform = verticalLayoutGroup.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);

        // Ensure the ScrollRect updates its position correctly
        scrollRect.verticalNormalizedPosition = 1.0f;
    }
}