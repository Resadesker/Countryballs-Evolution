using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    [SerializeField] public GameObject selectedImage;
    public int index;
    
    // [SerializeField] private Types typesListData;
    public BuildingShopSlot _buildingShopSlot;
    
    [SerializeField] private GameObject canUpgradeButton;
    
    
    private Image _image;
    private Animator _animator;
    
    private int availableLevels, currentLevel;

    // public int type;
    
    // private int level = 0; // TODO: (Script 3) Change it to normal level setting system
    
    // Start is called before the first frame update
    void Awake()
    {
        _image = GetComponent<Image>();
        _animator = GetComponent<Animator>();
        // PlayerPrefs.GetInt(playerPrefsBoolPrefix + index, 0);
    }

    public void Build(BuildingShopSlot _buildingShopSlot, int level = 0, RuntimeAnimatorController  animatorController = null) //Build(int type, AnimatorController animatorController = null)
    {
        // TODO: Instead of Types class use Image from bought item and set it to the image of the building
        // _image.sprite = typesListData.types[type];
        _animator.runtimeAnimatorController = animatorController;
        
        if (_buildingShopSlot is Home home)
        {
            _image.sprite = home.levels[level].avatar;
        }
        else if (_buildingShopSlot is Industry industry)
        {
            _image.sprite = industry.levels[level].avatar;
        }

        //_image.SetNativeSize();
        
        
        
        // this.type = type;
        this._buildingShopSlot = _buildingShopSlot;
        // Saves.Instance.SaveInt(Types.BuildingPlayerPrefsPrefix + index, _buildingShopSlot.index);
        // PlayerPrefs.SetInt(Types.playerPrefsPrefix + index, type);
    }

    public void OnClick()
    {
        if (_buildingShopSlot == null)
        {
            CurrencyManager.InstanceClicker.OnClick();
            UpgradeManager.Instance.OnBuildingDeselected();
            return;
        }
        // Select Building
        selectedImage.SetActive(true);
        
        UpgradeManager.Instance.OnBuildingSelected(index, this);
    }

    public void DeselectBuilding()
    {
        selectedImage.SetActive(false);
    }

    public void UnlockNewLevel()
    {
        availableLevels++;
        canUpgradeButton.SetActive(true);
    }

    public bool IsNextLevelAvailable()
    {
        if (availableLevels > currentLevel)
        {
            return true;
        }

        return false;
    }

    public void UpgradeLevel()
    {
        currentLevel++;
        if (availableLevels <= currentLevel) canUpgradeButton.SetActive(false);
    }
}
