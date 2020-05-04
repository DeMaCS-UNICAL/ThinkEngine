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
        if (stopWatch.ElapsedMilliseconds > waitingTime)
        {
            //UnityEngine.Debug.Log(stopWatch.ElapsedMilliseconds);
            stopWatch.Stop();
            stopWatch.Restart();
            if(brain.solverWaiting && !brain.areActuatorsReady())
            {
               // Debug.Log("sensors " + Time.time);

                //UnityEngine.Debug.Log("updating sensors "+contr.nextStep +" pellet "+ai.closestPelletX+" "+ai.closestPelletY+" pacman "+ai.Pacman.transform.position.x+" "+ ai.Pacman.transform.position.y);
                return true;
            }
            return false;
        }
        return false;
    }

    public bool applyActuator()
    {
       // Debug.Log("requesting actuator " + Time.time);

        return !contr._deadPlaying;
    }
}