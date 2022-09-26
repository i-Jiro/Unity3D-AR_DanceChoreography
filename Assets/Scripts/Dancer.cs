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
        if(DanceManager.Instance != null)
            DanceManager.Instance.StartDance += OnStartDance;
    }

    private void OnDisable()
    {
        if(DanceManager.Instance != null)
            DanceManager.Instance.StartDance -= OnStartDance;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    

    void OnStartDance()
    {
        _animator.SetTrigger("StartDance");
    }
}
