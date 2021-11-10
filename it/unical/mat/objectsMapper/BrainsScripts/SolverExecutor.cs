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

namespace ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts
{
    internal class SolverExectuor
    {
        Brain brain;
        string factsPath;
        internal bool reason;
        Stopwatch stopwatch = new Stopwatch();
        internal SolverExectuor(Brain b)
        {
            brain = b;
        }
        internal void Run()
        {
            if (!Directory.Exists(Path.GetTempPath() + @"ThinkEngineFacts\"))
            {
                Directory.CreateDirectory(Path.GetTempPath() + @"ThinkEngineFacts\");
            }
            reason = true;
            
            while (reason)
            {
                InputProgram encoding = new ASPInputProgram();
                foreach (string fileName in Directory.GetFiles(Application.streamingAssetsPath))
                {
                    string actualFileName = fileName.Substring(fileName.LastIndexOf(@"\") + 1);
                    if (actualFileName.StartsWith(brain.ASPFilesPrefix) && actualFileName.EndsWith(".asp"))
                    {
                        encoding.AddFilesPath(fileName);
                    }
                }
                if (encoding.FilesPaths.Count == 0)
                {
                    continue;
                }
                lock (brain.toLock)
                {
                    if (brain.reasonerMethod!=null)
                    {
                        brain.solverWaiting = true;
                        Monitor.Wait(brain.toLock);
                    }
                }
                try
                {
                    lock (brain.toLock)
                    {
                        if (brain.missingData)
                        {
                            brain.solverWaiting = true;
                            Monitor.Wait(brain.toLock);
                        }
                    }
                    if (!reason)
                    {
                        return;
                    }
                    ActuatorsManager.RequestObjectIndexes(brain);
                    if (!reason)
                    {
                        return;
                    }
                    SensorsManager.RequestSensorsMapping(brain);
                    if (!reason)
                    {
                        return;
                    }
                    factsPath = Path.GetTempPath() + @"ThinkEngineFacts\" + Path.GetRandomFileName() + ".txt";
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
                    Handler handler = new DesktopHandler(new DLV2DesktopService(Path.Combine(Application.streamingAssetsPath, @"lib\dlv2.exe")));
                    InputProgram facts = new ASPInputProgram();
                    facts.AddFilesPath(factsPath);
                    handler.AddProgram(encoding);
                    handler.AddProgram(facts);
                    handler.AddOption(new OptionDescriptor("--filter=setOnActuator/1 "));
                    stopwatch.Restart();
                    if (!reason)
                    {
                        return;
                    }
                    Output o = handler.StartSync();

                    if (!o.ErrorsString.Equals(""))
                    {
                        Debug.LogError(o.ErrorsString + " " + o.OutputString);
                    }
                    AnswerSets answers = (AnswerSets)o;
                    stopwatch.Stop();
                    if (answers.Answersets.Count > 0)
                    {
                        if (answers.GetOptimalAnswerSets().Count > 0)
                        {
                            ActuatorsManager.NotifyActuators(brain, answers.GetOptimalAnswerSets()[0]);
                        }
                        else
                        {
                            ActuatorsManager.NotifyActuators(brain, answers.answersets[0]);
                        }
                    }
                    if (!brain.maintainFactFile)
                    {
                        File.Delete(factsPath);
                    }
                }
                catch (Exception e)
                {
                    reason = false;
                    Debug.LogError("CAUGHT EXECPTION!!!! from "+Thread.CurrentThread.Name);
                    //Debug.LogError(e.Source);
                    Debug.LogError(e.Message);
                    Debug.LogError(e.StackTrace);
                }

            }

        }

    
    }
}
