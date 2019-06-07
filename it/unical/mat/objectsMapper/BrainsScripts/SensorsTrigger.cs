﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts
{
    public class SensorsTrigger :MonoBehaviour
    {
        public Brain brain;

        void OnTriggerEnter2D(Collider2D col)
        {

            Debug.Log("collisione with " + col.gameObject.name+" sensors");
        }
        void OnTriggerEnter(Collider col)
        {
            Debug.Log("collisione with " + col.gameObject.name + " sensors");
        }
    }
}
