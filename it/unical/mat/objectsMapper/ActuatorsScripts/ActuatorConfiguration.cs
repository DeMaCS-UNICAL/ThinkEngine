using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.ActuatorsScripts
{
    [Serializable]
    public class ActuatorConfiguration : AbstractConfiguration
    {
        internal override void cleanSpecificDataStructure()
        {
            throw new NotImplementedException();
        }

        internal override void specificConfiguration(FieldOrProperty fieldOrProperty, string s)
        {
            throw new NotImplementedException();
        }
    }
}
