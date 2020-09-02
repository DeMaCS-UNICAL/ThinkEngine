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

    public bool applyActuators()
    {
        Debug.Log("checking if apply");
        if (GameObject.FindObjectOfType<Player>().deadAnimation)
        {
            return false;
        }
        if (!GameObject.FindObjectOfType<Player>().move.Equals("still"))
        {
            return false;
        }
        Debug.Log("returning true for actuators");
        return true;
    }

}