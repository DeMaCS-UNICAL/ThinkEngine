using EmbASP4Unity.it.unical.mat.objectsMapper.Mappers;
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
        public bool reason;
        Stopwatch stopwatch = new Stopwatch();


        public SolverExectuor(Brain b)
        {
            brain = b;
            factsPath = "Assets/Resources/" + brain.gameObject.name + "Facts.asp";
        }

        public void Run()
        {

            reason = true;
            while (reason)
            {
                lock (brain.toLock)
                {
                    brain.solverWaiting = true;
                    Monitor.Wait(brain.toLock);
                }
                try
                {
                    SensorsManager.requestSensorsMapping(brain);
                    factsPath = Path.GetTempFileName();
                    using (StreamWriter fs = new StreamWriter(factsPath, true))
                    {
                        if (!reason)
                        {
                            return;
                        }
                        fs.Write(brain.sensorsMapping);
                        fs.Close();
                    }
                }
                catch (Exception e)
                {
                   MyDebugger.MyDebug("CAUGHT EXECPTION!!!!");
                    MyDebugger.MyDebug(e.Message);
                    MyDebugger.MyDebug(e.StackTrace);
                }


                Handler handler = new DesktopHandler(new DLV2DesktopService(@".\lib\dlv2.exe"));
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
