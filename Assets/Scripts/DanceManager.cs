using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using UnityEngine;

public class DanceManager : MonoBehaviour
{
    public static DanceManager Instance;

    [SerializeField] private string _danceQueueEventID;
    [SerializeField] private SimpleMusicPlayer _simpleMusicPlayer;
    
    public delegate void StartDanceEventHandler();
    public event StartDanceEventHandler StartDance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Koreographer.Instance.RegisterForEvents(_danceQueueEventID, OnQueueEvent);
    }

    private void OnQueueEvent(KoreographyEvent koreoEvent)
    {
        string tag = koreoEvent.GetTextValue();
        switch (tag)
        {
            case "Start":
                StartDance?.Invoke();
                break;
            case "End":
                _simpleMusicPlayer.Stop();
                break;
        }
    }

    public void StartDanceSequence()
    {
        _simpleMusicPlayer.Play();
    }

    IEnumerator DelayPlay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _simpleMusicPlayer.Play();
    }
}
