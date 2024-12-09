using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    private Slider _progressSlider;
    public static ProgressBar Instance { get; private set; }

    [SerializeField] private Image ageAvatar;
    
    // Start is called before the first frame update
    private void Start()
    {
        _progressSlider = GetComponent<Slider>();
        Instance = this;
    }

    // Update is called once per frame
    public void UpdateBar()
    {
        _progressSlider.value = ((float) BuildingManager.Instance.homes) / (PanelManager.Instance.country.ages[AgeManager.Instance.currentEra].eras[AgeManager.Instance.currentAge].buildingShopSlots.Length) * 100;
        //
        // if (_progressSlider.value == 100) // full
        // {
        //     ageAvatar.sprite = PanelManager.Instance.country.ages[0].eras[1].avatar; // TODO: Change avatar
        // }
    }
}
