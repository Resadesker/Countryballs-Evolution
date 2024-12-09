using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallAnimation : MonoBehaviour
{
    [SerializeField] private RuntimeAnimatorController[] skinAnimators; // 0 = default; 1 = cyborg; 2 = philosoph

    [SerializeField] private Image[] skinCheckmarks;

    private Animator _animator;
    private int instances = 0;

    public void Start()
    {
        _animator = GetComponent<Animator>();
        SetSkinAnimator((int)LoadSkinAnimator());
    }

    public long LoadSkinAnimator()
    {
        return Saves.Instance.LoadExtra("skin");
    }

    private void SetCheckmark(int skin)
    {
        foreach (Image checkmark in skinCheckmarks)
        {
            checkmark.enabled = false;
        }

        skinCheckmarks[skin].enabled = true;
    }

    public void SaveSkinAnimator(int skin)
    {
        Saves.Instance.SaveExtra("skin", skin);
        SetSkinAnimator(skin);
    }

    private void SetSkinAnimator(int skin)
    {
        SetCheckmark(skin);
        _animator.runtimeAnimatorController = skinAnimators[skin];
    }

    public void PlayAnimation()
    {
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        _animator.SetBool("Clicked", false);
        instances++;
        yield return new WaitForSeconds(0.01f);
        _animator.SetBool("Clicked", true);
        yield return new WaitForSeconds(0.35f);
        if (instances == 1)
            _animator.SetBool("Clicked", false);
        instances--;
    }
}