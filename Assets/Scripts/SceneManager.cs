using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    void Start()
    {
        //Stops mobile screen from timing out from inactivity.
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}
