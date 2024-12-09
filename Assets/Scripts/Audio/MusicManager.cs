using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] audios;
    public static MusicManager Instance { get; private set; }
    private AudioSource _audioSource;
    private int currentIndex = -1;
    
    void Start()
    {
        Instance = this;
        _audioSource = GetComponent<AudioSource>();
        CheckForMusic(AgeManager.Instance.GetEra());
        if (Saves.Instance.LoadExtra("music", 1) == 0)
            _audioSource.Stop();
    }

    public void DisableMusic()
    {
        _audioSource.Stop();
        Saves.Instance.SaveExtra("music", 0);
    }

    public void EnableMusic()
    {
        _audioSource.Play();
        Saves.Instance.SaveExtra("music", 1);
    }

    public void CheckForMusic(int era)
    {
        if (currentIndex == era) return;
        
        currentIndex = era;
        _audioSource.clip = audios[era];
        _audioSource.Stop();
        _audioSource.Play();
    }
}
