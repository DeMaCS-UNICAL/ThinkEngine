using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using it.unical.mat.embasp.specializations.dlv2.desktop;
using it.unical.mat.embasp.platforms.desktop;
using it.unical.mat.embasp.@base;
using it.unical.mat.embasp.languages.asp;
using UnityEngine;
using Planner;

namespace ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts
{
    internal class ASPActuatorExecutor:ASPExecutor
    {
        
        internal ASPActuatorExecutor(ActuatorBrain b)
        {
            brain = b;
        }

        protected override void SpecificAnswerSetOperations(AnswerSet answer)
        {
                ActuatorsManager.NotifyActuators((ActuatorBrain)brain,answer);
        }

        protected override bool SpecificFactsRetrieving(Brain brain)
        {
            ActuatorsManager.RequestObjectIndexes(brain as ActuatorBrain);
            return reason;
        }

        protected override void SpecificFactsWriting(Brain brain, StreamWriter fs)
        {
            fs.Write(((ActuatorBrain)brain).objectsIndexes);
        }

        protected override void SpecificOptions(Handler handler)
        {
            handler.AddOption(new OptionDescriptor("--filter=setOnActuator/1 "));
        }
       

    
    }
}
