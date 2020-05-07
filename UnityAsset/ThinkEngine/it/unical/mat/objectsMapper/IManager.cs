using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper
{
    public interface IManager: ISerializationCallbackReceiver
    {
        ref List<string> configuredGameObject();
        ref List<string> usedNames();
        ref List<AbstractConfiguration> confs();
        AbstractConfiguration findConfiguration(string v);
    }
}

