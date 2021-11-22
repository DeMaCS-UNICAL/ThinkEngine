using it.unical.mat.embasp.@base;
using it.unical.mat.embasp.languages.asp;
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

namespace ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts
{
    abstract class Executor
    {
        protected Brain brain;
        protected string factsPath;
        protected readonly Stopwatch stopwatch = new Stopwatch();
        internal bool reason;
        InputProgram encoding;
        internal virtual void Run()
        {
            if (!Directory.Exists(Path.GetTempPath() + @"ThinkEngineFacts\"))
            {
                Directory.CreateDirectory(Path.GetTempPath() + @"ThinkEngineFacts\");
            }
            reason = true;

            while (reason)
            {
                encoding = GetProgramInstance();
                foreach (string fileName in Directory.GetFiles(Application.streamingAssetsPath))
                {
                    string actualFileName = fileName.Substring(fileName.LastIndexOf(@"\") + 1);
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
                    SensorsManager.RequestSensorsMapping(brain);
                    if (!reason)
                    {
                        return;
                    }
                    factsPath = Path.GetTempPath() + @"ThinkEngineFacts\" + Path.GetRandomFileName() + ".txt";
                    using (StreamWriter fs = new StreamWriter(factsPath, true))
                    {
                        SpecificFactsWriting(brain, fs);
                        fs.Write(brain.sensorsMapping);
                        fs.Close();
                    }
                    Handler handler = GetHandler();
                    InputProgram facts = new ASPInputProgram();
                    facts.AddFilesPath(factsPath);
                    handler.AddProgram(encoding);
                    handler.AddProgram(facts);
                    SpecificOptions(handler);
                    stopwatch.Restart();
                    Output o = handler.StartSync();
                    if (!reason)
                    {
                        return;
                    }
                    if (!o.ErrorsString.Equals(""))
                    {
                        UnityEngine.Debug.LogError(o.ErrorsString + " " + o.OutputString);
                    }

                    OutputParsing(o);
                    if (!brain.maintainInputFile)
                    {
                        File.Delete(factsPath);
                    }
                }
                catch (Exception e)
                {
                    reason = false;
                    if (!(e is ThreadAbortException))
                    {
                        UnityEngine.Debug.LogError(e);
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
