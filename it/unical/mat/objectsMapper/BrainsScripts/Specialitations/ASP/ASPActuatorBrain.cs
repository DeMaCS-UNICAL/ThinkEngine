using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts.Actuator;

namespace BrainsScripts.Specialitations.ASP
{
    class ASPActuatorBrain : IActualActuatorBrain
    {
        public string ActualActuatorEncoding(string actuatorMappingAsASP)
        {
            return actuatorMappingAsASP;
        }

        public string ActualSensorEncoding(string sensorsAsASP)
        {
            return sensorsAsASP;
        }
    }
}
