using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


class TestingAddMonobehaviour:MonoBehaviour
{
    public List<GameObject> toTrack;

    void Awake()
    {
        if(toTrack is null)
        {
            toTrack = new List<GameObject>();
        }
    }

    void Start()
    {
        foreach(GameObject go in toTrack)
        {
            if(go.GetComponent<IndexTracker>() is null)
            {
                go.AddComponent<IndexTracker>();
            }
        }
    }
}

