using System;
using UnityEngine;

//every method of this class returning a bool value can be used to trigger the sensors update.
public class Trigger : ScriptableObject
{
    player player;
    void Awake()
    {
        player = GameObject.FindObjectOfType<player>();
    }
    public bool spawned()
    {
        if(player.mioTurno)
            return true;
        return false;
    }
    public bool applyActuatorsForPlayer()
    {
        return player.mioTurno;
    }
}