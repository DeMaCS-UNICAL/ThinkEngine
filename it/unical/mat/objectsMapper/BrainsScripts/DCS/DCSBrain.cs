using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts.Specialitations.ASP;

namespace ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts.DCS
{
    internal class DCSBrain : Brain
    {

        IActualDCSBrain _dcsBrain;
        IActualDCSBrain DcsBrain
        {
            get
            {
                if (_dcsBrain == null)
                {
                    if (FileExtension.Equals("asp"))
                    {
                        _dcsBrain = new ASPDCSBrain();
                    }
                }
                return _dcsBrain;
            }
        }
        protected override HashSet<string> SupportedFileExtensions
        {
            get
            {
                return new HashSet<string> { "asp" };
            }
        }

        protected override string SpecificFileParts()
        {
            throw new NotImplementedException();
        }

        internal override string ActualSensorEncoding(string sensorsAsASP)
        {
            throw new NotImplementedException();
        }
    }
}
