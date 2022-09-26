using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private ARMoveStage _moveStageComponent;
    [SerializeField] private GameObject _optionMenu;
    [SerializeField] private GameObject _startDanceButton;
    [SerializeField] private GameObject _reAnchorButton;
    [SerializeField] private GameObject _prompt;
    private bool isInOption;

    private void OnEnable()
    {
        if(_moveStageComponent != null)
            _moveStageComponent.PlacedDanceStage += OnPlacedDanceStage;
    }

    private void OnDisable()
    {
        if(_moveStageComponent != null)
            _moveStageComponent.PlacedDanceStage -= OnPlacedDanceStage;
    }

    private void Start()
    {
        HideAnchorAndStartButton();
    }

    public void HideAnchorAndStartButton()
    {
        _reAnchorButton.SetActive(false);
        _startDanceButton.SetActive(false);
    }
    
    private void OnPlacedDanceStage()
    {
        _startDanceButton.SetActive(true);
        _reAnchorButton.SetActive(true);
        _prompt.SetActive(false);
    }

    public void ToggleOptionMenu()
    {
        isInOption = !isInOption;
        _optionMenu.SetActive(isInOption);
    }
}
