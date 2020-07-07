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
                        Debug.Log("going to wait for pulse by brain");
                        brain.solverWaiting = true;
                        Monitor.Wait(brain.toLock);
                    }
                }
                try
                {
                    stopwatch.Restart();
                    factsPath = Path.GetTempFileName();

                    using (StreamWriter fs = new StreamWriter(factsPath, true))
                    {
                        string toAppend = SensorsManager.GetSensorsMapping(brain);
                        fs.Write(toAppend);
                        fs.Close();
                    }
                    stopwatch.Stop();
                    factsSteps++;
                    factsAvgTime += stopwatch.ElapsedMilliseconds;
                }
                catch (Exception e)
                {
                    Debug.Log("CAUGHT EXECPTION!!!!");
                    UnityEngine.Debug.LogError(e.Message);
                    UnityEngine.Debug.LogError(e.StackTrace);
                }


                Handler handler = new DesktopHandler(new DLVDesktopService(@".\lib\dlv2.exe"));
                InputProgram encoding = new ASPInputProgram();
                Debug.Log("adding encoding");
                encoding.AddFilesPath(Path.GetFullPath(brain.ASPFilePath));
                InputProgram facts = new ASPInputProgram();
                Debug.Log("adding facts");
                facts.AddFilesPath(factsPath);
                handler.AddProgram(encoding);
                handler.AddProgram(facts);
                handler.AddOption(new OptionDescriptor("--filter=setOnActuator/1"));
                stopwatch.Restart();
                Debug.Log("starting sync");
                Output o = handler.StartSync();
                if (!o.ErrorsString.Equals(""))
                {
                    Debug.Log(o.ErrorsString + " " + o.OutputString);
                }
                AnswerSets answers = (AnswerSets)o;
                stopwatch.Stop();
                asSteps++;
                asAvgTime += stopwatch.ElapsedMilliseconds;
                Debug.Log("num of AS " + answers.Answersets.Count);
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

    

        public void finalize()
        {
            Performance.writeOnFile("facts",factsAvgTime/factsSteps);
            Performance.writeOnFile("answer set",asAvgTime/asSteps);
        }
    }
}
