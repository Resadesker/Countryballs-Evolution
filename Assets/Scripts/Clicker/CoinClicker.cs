
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CoinClicker : MonoBehaviour, IPointerClickHandler
{
    private Canvas targetCanvas;

    private void Start()
    {
        targetCanvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (targetCanvas != null)
        {
            // Check if the clicked position is within the target canvas
            RectTransform rectTransform = targetCanvas.GetComponent<RectTransform>();
            Vector2 localPoint;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
            {
                // Check if the click is within the canvas boundaries
                if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, eventData.position, eventData.pressEventCamera))
                {
                    CurrencyManager.InstanceClicker.OnClick();
                    
                    UpgradeManager.Instance.OnBuildingDeselected();
                    // Handle your click logic for the desired canvas here
                }
            }
        }
    }
}