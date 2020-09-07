using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : ScriptableObject {

    public bool TriggerSensors() {
        return GameController.GameStarted && GameController.IaHasToPlay;
    }

}