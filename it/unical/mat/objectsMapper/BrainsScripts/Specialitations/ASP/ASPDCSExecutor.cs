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

        public ASPDCSExecutor(DCSBrain dCSBrain)
        {
            brain = dCSBrain;
        }

        protected override void SpecificAnswerSetOperations(AnswerSet answer)
        {
            
            ((DCSBrain)brain).ContentReady(answer);
        }

        protected override bool SpecificFactsRetrieving(Brain brain)
        {
            return true;
        }

        protected override void SpecificFactsWriting(Brain brain, StreamWriter fs)
        {
            fs.Write(((DCSBrain)brain).FactsToAdd);
            fs.Write(((DCSBrain)brain).PrefabFacts()) ;
        }

        protected override List<OptionDescriptor> SpecificOptions()
        {
            List<OptionDescriptor> options = new List<OptionDescriptor>();
            options.Add(new OptionDescriptor("--filter=instantiatePrefab/4,toPersists/2,Add/1,Delete/1,Update/2"));
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
    }
}
