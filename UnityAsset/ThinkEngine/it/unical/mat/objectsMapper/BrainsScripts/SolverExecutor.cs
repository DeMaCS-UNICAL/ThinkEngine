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
            //Debug.Log("mapper " + sensorMapper);
            while (reason)
            {
                //Thread.Sleep(1000);
                //Debug.Log("executing thread");
                lock (brain.toLock)
                {
                    brain.solverWaiting = true;
                    Monitor.Wait(brain.toLock);
                    try
                    {
                        stopwatch.Restart();
                        factsPath = Path.GetTempFileName();                    

                        using (StreamWriter fs = new StreamWriter(factsPath, true))
                        {
                            //Debug.Log("creating file "+ factsPath);
                            string toAppend = "";
                            foreach (AdvancedSensor sensor in brain.getSensors())
                            {
                                //Stopwatch temp = new Stopwatch();
                                //temp.Start();
                                 toAppend += sensor.Map();
                                //temp.Stop();
                                //Debug.Log(toAppend);
                                //Debug.Log(toAppend);

                            }
                            //Debug.Lof(fs.)
                            fs.Write(toAppend);
                            fs.Close();
                            //Debug.Log("closing stream");
                        }
                        stopwatch.Stop();
                        factsSteps++;
                        factsAvgTime += stopwatch.ElapsedMilliseconds;
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError(e.Message);
                        UnityEngine.Debug.LogError(e.StackTrace);
                    }

                }
                //Debug.Log(Path.GetFullPath(@".\lib\dlv.exe"));
                 Handler handler = new DesktopHandler(new DLVDesktopService(@".\lib\dlv.exe"));
                 InputProgram encoding = new ASPInputProgram();
                 encoding.AddFilesPath(Path.GetFullPath(brain.ASPFilePath));
                 InputProgram facts = new ASPInputProgram();
                 facts.AddFilesPath(factsPath);
                 handler.AddProgram(encoding);
                 handler.AddProgram(facts);
                handler.AddOption(new OptionDescriptor("-filter=setOnActuator"));
                stopwatch.Restart();
                //Debug.Log("reasoning");
                Output o = handler.StartSync();
                if (!o.ErrorsString.Equals(""))
                {
                    Debug.Log(o.ErrorsString + " " + o.OutputString);
                }
                 AnswerSets answers = (AnswerSets)o;
                stopwatch.Stop();
                asSteps++;
                asAvgTime += stopwatch.ElapsedMilliseconds;
                 //Debug.Log("debugging answer set");
                 //Debug.Log("there are "+answers.Answersets.Count);
                 //Debug.Log("error: " + answers.ErrorsString);
                if (answers.Answersets.Count > 0)
                {
                    string asPath = Path.GetTempFileName();
                    using (StreamWriter fs = new StreamWriter(asPath, true))
                    {
                        fs.Write(o.OutputString);
                        fs.Close();
                    }
                        lock (brain.toLock)
                    {
                        foreach (SimpleActuator actuator in brain.getActuators())
                        {
                            //Debug.Log("parsing " + actuator.actuatorName);
                            //Debug.Log(answers.Answersets[0].GetAnswerSet()[0]);
                            actuator.parse(answers.Answersets[0]);
                        }
                        brain.setActuatorsReady(true);
                    }
                }
                File.Delete(factsPath);
            }
            
        }
        public void finalize()
        {
            Performance.writeOnFile("facts",factsAvgTime/factsSteps);
            Performance.writeOnFile("answer set",asAvgTime/asSteps);
        }
    }
}
