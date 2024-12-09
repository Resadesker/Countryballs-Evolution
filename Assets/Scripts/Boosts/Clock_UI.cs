using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock_UI : MonoBehaviour
{
    public static Clock_UI Instance { get; private set; }
    private Animator _animator;
    private string animationName = "Spin";
    
    private void Start()
    {
        Instance = this;
        _animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }
    
    public void Launch(bool isMinuteTimer)
    {
        gameObject.SetActive(true);
        
        _animator.speed = isMinuteTimer ? 0.0833f : 1f;
        
        bool isPlaying = _animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);

        if (isPlaying)
        {
            // Reset the animation state to play it from the beginning
            _animator.Play(animationName, 0, 0f);
        }
        else
        {
            // Start playing the animation
            _animator.Play(animationName);
        }

        StartCoroutine(Hide(isMinuteTimer));
    }

    private IEnumerator Hide(bool isMinuteTimer)
    {
        yield return new WaitForSeconds(isMinuteTimer ? 60 : 5);
        gameObject.SetActive(false);
    }
}
