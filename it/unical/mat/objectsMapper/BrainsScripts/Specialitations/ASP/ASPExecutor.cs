using it.unical.mat.embasp.@base;
using it.unical.mat.embasp.languages.asp;
using it.unical.mat.embasp.platforms.desktop;
using it.unical.mat.embasp.specializations.dlv2.desktop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThinkEngine;
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
        protected override Handler GetHandler(out string file)
        {
            return SolversChecker.GetHandler(brain, out file);
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
            else
            {
                if (brain.debug)
                {
                    Debug.Log(o.OutputString);
                }
            }
        }
        protected override InputProgram GetInputProgram()
        {
            return new ASPInputProgram();
        }
        internal override bool TestSolver()
        {
            string file="";
            try
            {
                ASPInputProgram program = new ASPInputProgram();
                program.AddProgram("a :- b.");
                program.AddProgram("b.");
                Handler handler = GetHandler(out file);
                handler.AddProgram(program);
                Output output = handler.StartSync();
                Debug.Log("error: "+output.ErrorsString);
                Debug.Log("out: "+output.OutputString);
                List<string> aS = new List<string>();
                aS.AddRange(((AnswerSets)output).Answersets[0].GetAnswerSet());
                if (aS.Remove("b") && aS.Remove("a") && aS.Count == 0)
                {
                    return true;
                }
                Debug.LogError("Sorry,the " + brain.SolverName + " solver is not working properly. Check that you downloaded the right file\n"+file);
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
                Debug.LogError("Sorry,the " + brain.SolverName + " solver is not working properly. Check that you downloaded the right file\n" + file);
                return false;
            }
        }
        protected abstract void SpecificAnswerSetOperations(AnswerSet answer);
    }
}
