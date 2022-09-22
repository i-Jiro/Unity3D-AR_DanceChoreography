using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public class TypographyManager : MonoBehaviour
{
    [SerializeField] private string _typographyTrackID;
    
    [SerializeField] private List<TypographyBlock> _blocks;
    private readonly Dictionary<string, TypographyBlock> _blockDictionary = new Dictionary<string, TypographyBlock>();

    private TypographyBlock _currentBlock;
    
    void Start()
    {
        Koreographer.Instance.RegisterForEvents(_typographyTrackID, OnTypographyEvent);
        foreach (var block in _blocks)
        {
            _blockDictionary.Add(block.ID, block);
        }
    }

    void OnTypographyEvent(KoreographyEvent koreoEvent)
    {
        string tag = koreoEvent.GetTextValue();
        if (tag == "Clear")
        {
            _currentBlock?.ClearAll();
        }
        else
        {
            TypographyBlock block;
            if (_blockDictionary.TryGetValue(tag, out block))
            {
                block?.Step();
                _currentBlock = block;
            }
            else
                Debug.Log("Could not find block with ID: " + "'" + tag +"'.");
        }
    }
}
