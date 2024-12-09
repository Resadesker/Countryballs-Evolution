using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SoundType
{
    Sound,
    Music
}

public class CheckmarkSound : MonoBehaviour
{
    public static CheckmarkSound InstanceSound { get; private set; }
    public static CheckmarkSound InstanceMusic { get; private set; }
    public bool enabledSound; // other scripts should refer to this one 
    [SerializeField] private SoundType soundType;
    [SerializeField] private Sprite[] checkmarkSprites; // 0 = disabled; 1 = enabled
    private Image _image;
    private string dbNaming;
    
    void Start()
    {
        _image = GetComponent<Image>();
        dbNaming = soundType == SoundType.Sound ? "sound" : "music";
        if (soundType == SoundType.Sound)
        {
            InstanceSound = this;
        }
        else
        {
            InstanceMusic = this;
        }
        enabledSound = Saves.Instance.LoadExtra(dbNaming, 1) == 1;
        _image.sprite = checkmarkSprites[enabledSound ? 1 : 0];
    }

    public void OnClick()
    {
        enabledSound = !enabledSound;
        _image.sprite = checkmarkSprites[enabledSound ? 1 : 0];
        Saves.Instance.SaveExtra(dbNaming, enabledSound ? 1 : 0);
        if (soundType == SoundType.Music)
        {
            if (enabledSound)
            {
                MusicManager.Instance.EnableMusic();
            }
            else
            {
                MusicManager.Instance.DisableMusic();
            }
        }
    }
}
