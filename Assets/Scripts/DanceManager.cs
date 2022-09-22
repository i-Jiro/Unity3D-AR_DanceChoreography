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
        StartCoroutine(DelayPlay(3f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnQueueEvent(KoreographyEvent koreoEvent)
    {
        string text = koreoEvent.GetTextValue();
        if (text == "Start")
        {
            OnStartDance();
        }

        if (text == "End")
        {
            _simpleMusicPlayer.Stop();
        }
    }

    private void OnStartDance()
    {
        StartDance?.Invoke();
    }

    IEnumerator DelayPlay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _simpleMusicPlayer.Play();
    }
}
