using NewStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NewMappers.IntermediateMappers
{
    interface SensorDataMapper : DataMapper, SensorMapper
    {
        bool NeedsSpecifications();
        string BasicMap(object value);
    }
}
