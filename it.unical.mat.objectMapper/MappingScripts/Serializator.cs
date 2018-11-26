using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using UnityEngine;

namespace Assets.Scripts.MappingScripts
{
    class Serializator
    {
        ObjectIDGenerator iDGenerator;
        MyWindow myWindow;


        public Serializator(MyWindow window)
        {
            iDGenerator = new ObjectIDGenerator();
            myWindow = window;
        }

        internal void serialize(GameObject gO)
        {
            throw new NotImplementedException();
        }
    }
}
