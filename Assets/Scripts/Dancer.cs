using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Dancer : MonoBehaviour
{
    private Animator _animator;
    private void OnEnable()
    {
        DanceManager.Instance.StartDance += StartDanceAnimation;
    }

    private void OnDisable()
    {
        DanceManager.Instance.StartDance -= StartDanceAnimation;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    

    void StartDanceAnimation()
    {
        _animator.SetTrigger("StartDance");
    }
}
