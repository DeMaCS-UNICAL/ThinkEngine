using EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts;
using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

// every method of this class returning a bool value can be used to trigger the sensors update.
public class Trigger:ScriptableObject{
    Brain brain;
    Stopwatch stopWatch;
    PlayerController contr;
    long waitingTime;
    AISupportScript ai;

    private void OnEnable()
    {
        stopWatch = new Stopwatch();
        brain = GameObject.FindObjectOfType<Brain>();
        contr = GameObject.FindObjectOfType<PlayerController>();
        ai = GameObject.FindObjectOfType<AISupportScript>();
        stopWatch.Start();
        //waitingTime = 0;
    }


    public bool sensorsNeeded()
    {
        waitingTime = GameManager.scared ? 0 : 0;
         if (contr._deadPlaying)
         {
             return false;
         }
         return true;
         
    }

    public bool applyActuator()
    {
       // Debug.Log("requesting actuator " + Time.time);

        return !contr._deadPlaying;
    }
}