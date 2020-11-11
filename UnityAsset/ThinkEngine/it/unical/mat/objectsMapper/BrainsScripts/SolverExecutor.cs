using EmbASP4Unity.it.unical.mat.objectsMapper.Mappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using it.unical.mat.embasp.specializations.dlv2.desktop;
using it.unical.mat.embasp.platforms.desktop;
using it.unical.mat.embasp.@base;
using it.unical.mat.embasp.languages.asp;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts
{
    public class SolverExectuor
    {
        public Brain brain;
        string factsPath;
        public bool reason;
        Stopwatch stopwatch = new Stopwatch();


        public SolverExectuor(Brain b)
        {
            brain = b;
        }

        public void Run()
        {
            if(!Directory.Exists(Path.GetTempPath() + @"ThinkEngineFacts\"))
            {
                Directory.CreateDirectory(Path.GetTempPath() + @"ThinkEngineFacts\");
            }
            reason = true;
            while (reason)
            {
                //MyDebugger.MyDebug("aquiring the lock");
                lock (brain.toLock)
                {
                    if (brain.reasonerMethod!=null)
                    {
                        brain.solverWaiting = true;
                        //MyDebugger.MyDebug("going to wait");
                        Monitor.Wait(brain.toLock);
                    }
                }
                try
                {
                    //MyDebugger.MyDebug("awake");
                    lock (brain.toLock)
                    {
                        if (brain.missingData)
                        {
                            brain.solverWaiting = true;
                            //MyDebugger.MyDebug("going to wait");
                            Monitor.Wait(brain.toLock);
                        }
                    }
                    ActuatorsManager.requestObjectIndexes(brain);
                    SensorsManager.requestSensorsMapping(brain);
                    factsPath = Path.GetTempPath() + @"ThinkEngineFacts\" + Path.GetRandomFileName()+".txt";
                    using (StreamWriter fs = new StreamWriter(factsPath, true))
                    {
                        if (!reason)
                        {
                            return;
                        }
                        fs.Write(brain.objectsIndexes);
                        fs.Write(brain.sensorsMapping);
                        fs.Close();
                    }
                }
                catch (Exception e)
                {
                   MyDebugger.MyDebug("CAUGHT EXECPTION!!!!");
                    MyDebugger.MyDebug(e.Source);
                    MyDebugger.MyDebug(e.Message);
                    MyDebugger.MyDebug(e.StackTrace);
                }

                Handler handler = new DesktopHandler(new DLV2DesktopService(brain.dataPath+@"\lib\dlv2.exe"));
                InputProgram encoding = new ASPInputProgram();
                encoding.AddFilesPath(Path.GetFullPath(brain.ASPFilePath));
                InputProgram facts = new ASPInputProgram();
                facts.AddFilesPath(factsPath);
                handler.AddProgram(encoding);
                handler.AddProgram(facts);
                handler.AddOption(new OptionDescriptor("--filter=setOnActuator/1 "));
                stopwatch.Restart();
                Output o = handler.StartSync();
                
                if (!o.ErrorsString.Equals(""))
                {
                    Debug.LogError(o.ErrorsString + " " + o.OutputString);
                }
                AnswerSets answers = (AnswerSets)o;
                stopwatch.Stop();
                brain.asSteps++;
                brain.asTotalMS += stopwatch.ElapsedMilliseconds;
                if (answers.Answersets.Count > 0)
                {
                    ActuatorsManager.notifyActuators(brain,answers.answersets[0]);
                }
                if (!brain.maintainFactFile)
                {
                    File.Delete(factsPath);
                }
            }
        }

    
    }
}
