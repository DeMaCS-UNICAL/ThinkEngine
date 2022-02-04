using it.unical.mat.embasp.@base;
using it.unical.mat.embasp.languages.asp;
using it.unical.mat.embasp.platforms.desktop;
using it.unical.mat.embasp.specializations.dlv2.desktop;
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
    abstract class ASPExecutor : Executor
    {
        protected override InputProgram GetProgramInstance()
        {
            return new ASPInputProgram();
        }
        protected override string GetCurrentFileExtension()
        {
            return ".asp";
        }
        protected override Handler GetHandler()
        {
            return new DesktopHandler(new DLV2DesktopService(Path.Combine(Application.streamingAssetsPath, @"lib\dlv2.exe")));
        }
        protected override void OutputParsing(Output o)
        {
            AnswerSets answers = (AnswerSets)o;
            stopwatch.Stop();
            if (answers.Answersets.Count > 0)
            {
                foreach(string s in answers.answersets[0].GetAnswerSet())
                {
                    Debug.Log(s);
                }
                SpecificAnswerSetOperations(answers);
            }
        }
        protected abstract void SpecificAnswerSetOperations(AnswerSets answers);
    }
}
