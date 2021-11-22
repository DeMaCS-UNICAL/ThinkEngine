using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts.Actuator
{
    interface IActualActuatorBrain
    {
        string ActualSensorEncoding(string sensorsAsASP);
        string ActualActuatorEncoding(string actuatorMappingAsASP);
    }
}
