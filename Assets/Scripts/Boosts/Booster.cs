using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public enum BoosterType
{
    Chest,
    Clock,
    Car,
    Ship,
    Plane
}

public class Booster : MonoBehaviour
{
    [SerializeField] private Popup_Particles_UI _popupOnClick;
    [SerializeField] private BoosterType _boosterType;
    
    [Space] // for car and ship only
    [SerializeField] private Sprite[] _levels;
    public int level;
    private Image _image;
    
    [Space] // for clock only
    [SerializeField] private GameObject text5X;
    [SerializeField] private Industry[] machines; // 0 = ship; 1 = car; 2 = plane
    public SoundEmitter clockTickSoundEmitter;
    
    public SoundEmitter soundEmitter;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }
    
    void Start()
    {
        if (_boosterType == BoosterType.Car || _boosterType == BoosterType.Ship)
        {
            StartCoroutine(SelfDestruct(11));
        }
        else StartCoroutine(SelfDestruct(10));
    }

    public void SetLevel(int lvl)
    {
        level = lvl;
        _image.sprite = _levels[level];
    }

    public void OnClick()
    {
        soundEmitter.Play();
        if (_boosterType == BoosterType.Ship)
        {
            _popupOnClick.OnClick();
            CurrencyManager.InstanceClicker.AddMoneyAmount(machines[0].levels[level].machineBonus);
        }
        else if (_boosterType == BoosterType.Car)
        {
            _popupOnClick.OnClick();
            CurrencyManager.InstanceClicker.AddMoneyAmount(machines[1].levels[level].machineBonus);
        }
        else if (_boosterType == BoosterType.Plane)
        {
            _popupOnClick.OnClick();
            CurrencyManager.InstanceClicker.AddMoneyAmount(machines[2].levels[level].machineBonus);
        }
        else if (_boosterType == BoosterType.Chest)
        {
            _popupOnClick.OnClick();
            CurrencyManager.InstanceClicker.AddMoneyAmount(CurrencyManager.InstanceClicker.TotalMoneyPerClick * 250);
        }
        else if (_boosterType == BoosterType.Clock)
        {
            GameObject g = Instantiate(text5X);
            g.transform.parent = transform.parent;
            g.transform.localPosition = new Vector3(0, 0, 0);
            Clock_UI.Instance.Launch(false);
            clockTickSoundEmitter.Play();
            Booster_Afterlife.Instance.StartCoroutine(Booster_Afterlife.Instance.OnSpeedUp(g, 5, 5));
        }
        Destroy(gameObject);
    }

    

    private IEnumerator SelfDestruct(int num)
    {
        yield return new WaitForSeconds(num);
        Destroy(gameObject);
    }
}
