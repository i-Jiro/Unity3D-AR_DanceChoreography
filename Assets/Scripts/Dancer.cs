using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Dancer : MonoBehaviour
{
    private Animator _animator;
    private AfterImageSkinnedMeshController _afterImageController;
    private void OnEnable()
    {
        if (DanceManager.Instance != null)
        {
            DanceManager.Instance.StartDance += OnStartDance;
            DanceManager.Instance.StartAfterImage += OnAfterImageSequenceStart;
            DanceManager.Instance.EndAfterImage += OnAfterImageSequenceEnd;
        }
    }

    private void OnDisable()
    {
        if (DanceManager.Instance != null)
        {
            DanceManager.Instance.StartDance -= OnStartDance;
            DanceManager.Instance.StartAfterImage -= OnAfterImageSequenceStart;
            DanceManager.Instance.EndAfterImage -= OnAfterImageSequenceEnd;
        }
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _afterImageController = GetComponent<AfterImageSkinnedMeshController>();
    }
    

    void OnStartDance()
    {
        _animator.SetTrigger("StartDance");
    }

    void OnAfterImageSequenceStart()
    {
        _afterImageController.Activate();
    }

    void OnAfterImageSequenceEnd()
    {
        _afterImageController.Deactivate();
    }
}
