using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper
{
    public interface IManager
    {
        List<string> getConfiguredGameObject();
        List<string> getUsedNames();
        void delete(string v);
        void addConfiguration(AbstractConfiguration abstractConfiguration);
        bool existsConfigurationWithName(string name);
    }
}

