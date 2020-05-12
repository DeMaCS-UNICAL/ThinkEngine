using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper
{
    public interface IManager: ISerializationCallbackReceiver
    {
        List<string> getConfiguredGameObject();
        List<string> getUsedNames();
        List<AbstractConfiguration> getConfigurations();
        AbstractConfiguration findConfiguration(string v);
        void delete(string v);
        AbstractConfiguration newConfiguration(string n,string go);
        void addConfiguration(AbstractConfiguration abstractConfiguration);
    }
}

