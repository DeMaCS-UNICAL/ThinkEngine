using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using UnityEngine;

namespace Assets.Scripts.MappingScripts
{
    public  class Serializator
    {
        ObjectIDGenerator iDGenerator;
        GameObjectsTracker tracker;


        public Serializator(GameObjectsTracker tr)
        {
            iDGenerator = new ObjectIDGenerator();
            tracker = tr;
        }

        internal void serialize(GameObject gO)
        {
            throw new NotImplementedException();
        }
    }
}
