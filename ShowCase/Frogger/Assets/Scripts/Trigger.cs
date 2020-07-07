using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

// every method of this class returning a bool value can be used to trigger the sensors update.
public class Trigger:ScriptableObject{
    Stopwatch watch;
    private void OnEnable()
    {
        watch = new Stopwatch();
        watch.Start();
    }

    public bool runReasoner()
    {
        if (GameObject.FindObjectOfType<Player>().deadAnimation)
        {
            return false;
        }
        if (GameObject.FindObjectOfType<Player>().move != 0)
        {
            return false;
        }
        Debug.Log("returning true for sensors");
        return true;
    }

}