using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
public class TypographyBlock : MonoBehaviour
{
    //ID corresponds to the text payload in Typography track.
    [SerializeField] private string _iD;
    public string ID => _iD;
    
    [Tooltip("Order the text objects in order of desired appearance.")]
    [SerializeField] private List<GameObject> _textLineObjects;
    private int _currentIndex = 0;

    public void Step()
    {
        if (_currentIndex > _textLineObjects.Count) return;
        _textLineObjects[_currentIndex].SetActive(true);
        _currentIndex++;
    }

    //Hide all text in typography block. 
    public void ClearAll()
    {
        foreach (var textObject in _textLineObjects)
        {
            textObject.SetActive(false);
        }
    }
}
