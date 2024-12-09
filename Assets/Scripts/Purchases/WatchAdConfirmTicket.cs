using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchAdConfirmTicket : MonoBehaviour
{
    [SerializeField] private DailyChest dailyChest;
    [SerializeField] private RewardChestPanel rewardChestPanel;
    
    public void Enable()
    {
        if (Saves.Instance.LoadExtra("remove_ads") == 0) gameObject.SetActive(true);
        else
        {
            dailyChest.DisableChestOnOpen();
            rewardChestPanel.OpenChest(true);
        }
    }
}
