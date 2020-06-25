using System;
using System.Diagnostics;
using UnityEngine;

// every method of this class returning a bool value can be used to trigger the sensors update.
 public class Trigger:ScriptableObject{
    Stopwatch watch;
    private void OnEnable()
    {
        watch = new Stopwatch();
        watch.Start();
    }

    public bool updateSensors()
    {
        watch.Stop();
        if (watch.ElapsedMilliseconds < 300)
        {
            watch.Start();
            return false;
        }
        watch.Restart();
        if (GameObject.FindObjectOfType<Player>().deadAnimation)
        {
            return false;
        }
        if (GameObject.FindObjectOfType<Player>().move != 0)
        {
            return false;
        }
        return true;
    }

}