using it.unical.mat.embasp.@base;
using it.unical.mat.embasp.languages.asp;
using it.unical.mat.embasp.platforms.desktop;
using Planner;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts
{
    abstract class Executor
    {
        private class MyCallBack : ICallback
        {
            Executor executor;
            public MyCallBack(Executor executor)
            {
                this.executor = executor;
            }
            public void Callback(Output o)
            {
                if (!executor.reason)
                {
                    return;
                }
                if (!o.ErrorsString.Equals(""))
                {
                    UnityEngine.Debug.LogError(o.ErrorsString + " " + o.OutputString);
                }
                Performance.WriteOnFile("solver "+executor.brain.executorName, executor.stopwatch.ElapsedMilliseconds);
                executor.stopwatch.Stop();
                executor.OutputParsing(o);
                if (!executor.brain.maintainInputFile)
                {
                    File.Delete(executor.factsPath);
                }
                executor.solverDone = true;
            }
        }
        protected Brain brain;
        protected string factsPath;
        protected readonly Stopwatch stopwatch = new Stopwatch();
        internal bool reason;
        InputProgram encoding;
        private static object toLock = new object();
        internal static bool _canRead=false;
        
        private static int _readingSensors=0;
        internal bool solverDone;
        private static void IncrementReadingSensors()
        {
            lock (toLock)
            {
                if (!_canRead)
                {
                    Monitor.Wait(toLock);
                }
                _readingSensors++;
            }
        }
        private static void DecreaseReadingSensors()
        {
            lock (toLock)
            {
                _readingSensors--;
            }
        }
        internal static void ShutDownAll()
        {
            lock (toLock)
            {
                _canRead = false;
                Monitor.PulseAll(toLock);
            }
        }

        internal static bool Reading()
        {
            return _readingSensors > 0;
        }

        internal static bool CanRead(bool value)
        {
            lock (toLock)
            {
                if(_readingSensors>0 && !value)
                {
                    return false;
                }
                _canRead = value;
                if (_canRead)
                {
                    Monitor.PulseAll(toLock);
                }
                return true;
            }
        }
        internal virtual void Run()
        {
            if (!Directory.Exists(Path.GetTempPath() + @"ThinkEngineFacts"+Utility.slash))
            {
                Directory.CreateDirectory(Path.GetTempPath() + @"ThinkEngineFacts" + Utility.slash);
            }
            reason = true;

            while (reason)
            {
                encoding = GetProgramInstance();
                foreach (string fileName in Directory.GetFiles(Path.Combine(Application.streamingAssetsPath,"ThinkEngine")))
                {
                    string actualFileName = fileName.Substring(fileName.LastIndexOf(Utility.slash) + 1);
                    if (actualFileName.StartsWith(brain.AIFilesPrefix) && actualFileName.EndsWith(GetCurrentFileExtension()))
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
                    if (brain.reasonerMethod != null)
                    {
                        brain.solverWaiting = true;
                        Monitor.Wait(brain.toLock);
                        if (!reason)
                        {
                            return;
                        }

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
                    if (!SpecificFactsRetrieving(brain))
                    {

                        return;
                    }
                    IncrementReadingSensors();
                    if (!_canRead)
                    {
                        return;
                    }
                    stopwatch.Restart();
                    SensorsManager.ReturnSensorsMappings(brain);
                    DecreaseReadingSensors();
                    if (!reason)
                    {
                        return;
                    }
                    factsPath = Path.Combine(Path.GetTempPath(),"ThinkEngineFacts",Path.GetRandomFileName()+".txt");

                    using (StreamWriter fs = new StreamWriter(factsPath, true))
                    {
                        SpecificFactsWriting(brain, fs);
                        fs.Write(brain.sensorsMapping);
                        fs.Close();
                    }
                    Performance.WriteOnFile("facts executor", stopwatch.ElapsedMilliseconds);
                    Handler handler = GetHandler();
                    InputProgram facts = new ASPInputProgram();
                    facts.AddFilesPath(factsPath);
                    handler.AddProgram(encoding);
                    handler.AddProgram(facts);
                    if (!brain.debug)
                    {
                        SpecificOptions(handler);
                    }
                    stopwatch.Restart();

                    //Debug.Log("running dlv");
                    stopwatch.Restart();
                    if (!reason)
                    {
                        return;
                    }
                    handler.StartAsync(new MyCallBack(this));
                    while (!solverDone)
                    {
                        if (!reason)
                        {
                            if(handler is DesktopHandler dh)
                            {
                                dh.StopProcess();
                            }
                            return;
                        }
                    }
                    solverDone = false;
                }
                catch (Exception e)
                {
                    Debug.Log("there was an exception");
                    if (!(e is ThreadAbortException))
                    {
                        UnityEngine.Debug.LogError(e);
                    }
                    else
                    {
                        reason = false;
                    }
                }

            }

        }

        protected abstract void OutputParsing(Output o);

        protected abstract Handler GetHandler();

        protected abstract string GetCurrentFileExtension();
        protected abstract InputProgram GetProgramInstance();
        protected abstract void SpecificFactsWriting(Brain brain, StreamWriter fs);
        protected abstract bool SpecificFactsRetrieving(Brain brain);
        protected abstract void SpecificOptions(Handler handler);
    }
}
