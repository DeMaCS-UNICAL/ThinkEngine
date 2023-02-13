using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts.DCS
{
    internal interface IActualDCSBrain
    {
        string ActualSensorEncoding(string sensorsAsASP);
        string SpecificFileParts();

    }
}
