using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        if (Saves.Instance.LoadExtra("sound", 1) == 1)
            _audioSource.Play();
    }
}
