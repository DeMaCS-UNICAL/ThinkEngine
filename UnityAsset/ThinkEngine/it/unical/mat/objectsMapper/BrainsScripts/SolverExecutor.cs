using EmbASP4Unity.it.unical.mat.objectsMapper.ActuatorsScripts;
using EmbASP4Unity.it.unical.mat.objectsMapper.Mappers;
using EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts;
using EmbASP4Unity.it.unical.mat.embasp.@base;
using EmbASP4Unity.it.unical.mat.embasp.languages.asp;
using EmbASP4Unity.it.unical.mat.embasp.platforms.desktop;
using EmbASP4Unity.it.unical.mat.embasp.specializations.dlv.desktop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using EmbASP4Unity.it.unical.mat.embasp.specializations.dlv2.desktop;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts
{
    public class SolverExectuor
    {
        public Brain brain;
        string factsPath;
        MappingManager mapper;
        public bool reason;
        Stopwatch stopwatch = new Stopwatch();
        int factsSteps = 0;
        double factsAvgTime = 0;
        int asSteps = 0;
        double asAvgTime = 0;


        public SolverExectuor(Brain b)
        {
            brain = b;
            factsPath = "Assets/Resources/" + brain.gameObject.name + "Facts.asp";
            mapper = MappingManager.getInstance();
        }

        public void Run()
        {

            reason = true;
            IMapper sensorMapper = mapper.getMapper(typeof(AdvancedSensor));
            while (reason)
            {
                if (!brain.executeReasonerOn.Equals("When Sensors are ready"))
                {
                    lock (brain.toLock)
                    {
                        MyDebugger.MyDebug("going to wait for pulse by brain");
                        brain.solverWaiting = true;
                        Monitor.Wait(brain.toLock);
                    }
                }
                try
                {
                    factsPath = Path.GetTempFileName();

                    using (StreamWriter fs = new StreamWriter(factsPath, true))
                    {
                        string toAppend = SensorsManager.GetSensorsMapping(brain);
                        if (!reason)
                        {
                            return;
                        }
                        fs.Write(toAppend);
                        fs.Close();
                    }
                }
                catch (Exception e)
                {
                   MyDebugger.MyDebug("CAUGHT EXECPTION!!!!");
                    MyDebugger.MyDebug(e.Message);
                    MyDebugger.MyDebug(e.StackTrace);
                }


                Handler handler = new DesktopHandler(new DLV2DesktopService(@".\lib\dlv2.exe"));                //With DLV2DesktopService I get a Error during parsing: --> Invalid #show directive: setOnActuator/1--competition-output.
                //With DLVDesktopService the AS, obviously, are wrongly parsed
                InputProgram encoding = new ASPInputProgram();
                MyDebugger.MyDebug("adding encoding");
                encoding.AddFilesPath(Path.GetFullPath(brain.ASPFilePath));
                InputProgram facts = new ASPInputProgram();
                MyDebugger.MyDebug("adding facts");
                facts.AddFilesPath(factsPath);
                handler.AddProgram(encoding);
                handler.AddProgram(facts);
                handler.AddOption(new OptionDescriptor("--filter=setOnActuator/1 "));
                stopwatch.Restart();
                MyDebugger.MyDebug("starting sync");
                Output o = handler.StartSync();
                if (!o.ErrorsString.Equals(""))
                {
                    MyDebugger.MyDebug(o.ErrorsString + " " + o.OutputString);
                }
                AnswerSets answers = (AnswerSets)o;
                stopwatch.Stop();
                brain.asSteps++;
                brain.asTotalMS += stopwatch.ElapsedMilliseconds;
                MyDebugger.MyDebug("num of AS " + answers.Answersets.Count);
                if (answers.Answersets.Count > 0)
                {
                    lock (brain.toLock)
                    {
                        foreach (SimpleActuator actuator in brain.getActuators())
                        {
                            actuator.parse(answers.Answersets[0]);
                        }
                        brain.setActuatorsReady(true);
                    }
                }
                if (!brain.maintainFactFile)
                {
                    File.Delete(factsPath);
                }
            }
        }

    
    }
}
