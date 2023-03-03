using it.unical.mat.embasp.@base;
using it.unical.mat.embasp.languages.asp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts.DCS;
using UnityEngine;
using Random = System.Random;

namespace ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts.Specialitations.ASP
{
    internal class ASPDCSExecutor : ASPExecutor
    {
        bool firstDone;
        string dataPath;
        public ASPDCSExecutor(DCSBrain dCSBrain)
        {
            brain = dCSBrain;
            dataPath = Application.dataPath;
        }

        protected override void SpecificAnswerSetOperations(AnswerSet answer)
        {
            firstDone = true;
            ((DCSBrain)brain).ContentReady(answer);
        }

        protected override bool SpecificFactsRetrieving(Brain brain)
        {
            return true;
        }

        protected override void SpecificFactsWriting(Brain brain, StreamWriter fs)
        {
            DCSBrain brain1 = ((DCSBrain)brain);
            fs.Write(brain1.FactsForExecutor);
            fs.Write(brain1.PrefabFacts()) ;
            if (!firstDone)
            {
                using (StreamReader sr = new StreamReader(brain1.initAIFile))
                {
                    fs.WriteLine(sr.ReadToEnd());
                }
            }
        }

        protected override List<OptionDescriptor> SpecificOptions()
        {
            List<OptionDescriptor> options = new List<OptionDescriptor>();
            options.Add(new OptionDescriptor("--filter=instantiatePrefab/4,Add/1,Delete/1,Update/2 -n="+((DCSBrain)brain).numberOfAnswerSet));
            return options;
        }
        protected override AnswerSet GetAnswerSet(AnswerSets answers)
        {
            return answers.answersets[new Random().Next(answers.answersets.Count)];
        }
        protected override AnswerSet GetOptimal(AnswerSets answers)
        {
            IList<AnswerSet> answerSets = answers.GetOptimalAnswerSets();
            return answerSets[new Random().Next(answerSets.Count)];
        }
        protected override void AddFileToEncoding(string fileName)
        {
            if (fileName != Path.Combine(dataPath,((DCSBrain)brain).initAIFile))
            {
                base.AddFileToEncoding(fileName);
            }
        }
    }
}
