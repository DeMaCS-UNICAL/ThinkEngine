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
            return new DesktopHandler(new DLV2DesktopService(Path.Combine(Application.streamingAssetsPath, @"lib"+ Utility.slash +"dlv2"+Utility.RunnableExtension)));
        }
        protected override void OutputParsing(Output o)
        {
            AnswerSets answers = (AnswerSets)o;
            stopwatch.Stop();
            if (answers.Answersets.Count > 0)
            {
                if (answers.GetOptimalAnswerSets().Count > 0)
                {
                    
                    AnswerSet answer = answers.GetOptimalAnswerSets()[0];
                    if (brain.debug)
                    {
                        Debug.Log("Computed AnswerSet:\n" + string.Join(" ; ", answer.GetAnswerSet()));
                        foreach(int level in answer.LevelWeight.Keys)
                        {
                            Debug.Log("level " + level + " cost " + answer.LevelWeight[level]);
                        }

                    }
                    SpecificAnswerSetOperations(answer);
                }
                else
                {
                    AnswerSet answer = answers.answersets[0];
                    if (brain.debug)
                    {
                        Debug.Log("Computed AnswerSet:\n" + string.Join(" ; ", answer.GetAnswerSet()));
                    }
                    SpecificAnswerSetOperations(answer);
                }
            }
        }
        protected abstract void SpecificAnswerSetOperations(AnswerSet answer);
    }
}
