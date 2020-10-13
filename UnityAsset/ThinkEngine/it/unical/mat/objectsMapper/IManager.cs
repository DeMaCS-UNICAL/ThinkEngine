using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public interface IManager
{
    List<string> getConfiguredGameObject();
    List<string> getUsedNames();
    void addConfiguration(AbstractConfiguration abstractConfiguration);
    bool existsConfigurationWithName(string name);
    void deleteConfiguration(AbstractConfiguration abstractConfiguration);
}


