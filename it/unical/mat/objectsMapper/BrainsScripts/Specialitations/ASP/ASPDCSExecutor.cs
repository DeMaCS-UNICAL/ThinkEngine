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
        bool custom_bk_addedd;
        string dataPath;
        string bK_file_path;
        public ASPDCSExecutor(ContentBrain dCSBrain)
        {
            brain = dCSBrain;
            bK_file_path = Path.Combine(Utility.StreamingAssetsContent, dCSBrain.name + "_BackgroundKnowledge__ThinkEngine.asp");
            dataPath = Application.dataPath;
            using (StreamWriter fs = new StreamWriter(bK_file_path))
            {
                fs.WriteLine(dCSBrain.PrefabFacts());
                fs.WriteLine("height(" + dCSBrain.sceneHeight + ")."+Environment.NewLine);
            }
            
        }

        protected override void SpecificAnswerSetOperations(AnswerSet answer)
        {
            firstDone = true;
            ((ContentBrain)brain).ContentReady(answer);
        }

        protected override bool SpecificFactsRetrieving(Brain brain)
        {
            return true;
        }

        protected override void SpecificFactsWriting(Brain brain, StreamWriter fs)
        {
            ContentBrain brain1 = ((ContentBrain)brain);
            fs.Write(brain1.FactsForExecutor);
            //fs.Write(brain1.PrefabFacts()) ;
            if (!firstDone)
            {
                fs.WriteLine(new StreamReader(brain1.initAIFile).ReadToEnd());
                fs.WriteLine("current_stripe(" + brain1.initialStripe + ")."+Environment.NewLine);
            }
        }

        protected override List<OptionDescriptor> SpecificOptions()
        {
            List<OptionDescriptor> options = new List<OptionDescriptor>();
            if (!brain.debug)
            {
                options.Add(new OptionDescriptor("--filter=instantiatePrefab/4,Add/1,Delete/1,Update/2 -n " + ((ContentBrain)brain).numberOfAnswerSet));
            }
            else
            {
                Debug.Log("Adding -n "+ ((ContentBrain)brain).numberOfAnswerSet);
                options.Add(new OptionDescriptor("-n " + ((ContentBrain)brain).numberOfAnswerSet));
            }
            return options;
        }
        protected override AnswerSet GetAnswerSet(AnswerSets answers)
        {
            Debug.Log("No optimal: "+answers.answersets.Count);
            return answers.answersets[new Random().Next(answers.answersets.Count)];
        }
        protected override AnswerSet GetOptimal(AnswerSets answers)
        {
            IList<AnswerSet> answerSets = answers.GetOptimalAnswerSets();
            Debug.Log("Optimal: "+answerSets.Count);
            return answerSets[new Random().Next(answerSets.Count)];
        }
        protected override void AddFileToEncoding(string fileName)
        {
            if (((ContentBrain)brain).useCustomBK && !custom_bk_addedd)
            {
                custom_bk_addedd = true;
                base.AddFileToEncoding(((ContentBrain)brain).custom_bk_file_path);
            }
            string temp1 = fileName.Replace('\\', '/');
            string temp2 = Path.Combine(dataPath, ((ContentBrain)brain).initAIFile.Substring(7)).Replace('\\', '/');
            if (temp1 != temp2)
            {
                base.AddFileToEncoding(fileName);
            }
        }
        protected override void CleanUpOperations()
        {
            //File.Delete(bK_file_path);
        }
    }
}
