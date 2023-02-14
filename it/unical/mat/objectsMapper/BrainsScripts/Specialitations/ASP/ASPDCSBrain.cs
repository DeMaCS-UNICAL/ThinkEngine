using it.unical.mat.embasp.languages.asp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts.DCS;
using UnityEngine;

namespace ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts.Specialitations.ASP
{
    internal class ASPDCSBrain : IActualDCSBrain
    {
        string facts;
        public string ActualSensorEncoding(string sensorsAsASP)
        {
            return sensorsAsASP;
        }

        public void ContentReady(object content, DCSBrain brain)
        {
            if (!(content is AnswerSet))
            {
                return;
            }
            AnswerSetAvailable((AnswerSet)content, brain);
        }

        private void AnswerSetAvailable(AnswerSet content, DCSBrain brain)
        {
            foreach (string literal in content.GetAnswerSet())
            {
                string substring = "instantiatePrefab(";
                if (literal.StartsWith(substring))
                {
                    string temp = literal.Substring(substring.Length);
                    temp = temp.Remove(temp.Length - 1, 1);
                    string[] values = temp.Split(',');
                    if (values.Length > 0)
                    {
                        if (int.TryParse(values[0], out int index))
                        {
                            if (brain.instantiablePrefabs.Count > index)
                            {
                                PrefabInstantiator instantiator = new PrefabInstantiator();
                                instantiator.toInstantiate = brain.instantiablePrefabs[index];
                                float.TryParse(values[1], out instantiator.x);
                                float.TryParse(values[2], out instantiator.y);
                                float.TryParse(values[3], out instantiator.z);
                            }
                        }
                    }

                }
            }
        }

        public Executor GetDCSExecutor(DCSBrain dCSBrain)
        {
            return new ASPDCSExecutor(dCSBrain);
        }

        public string PrefabFacts(DCSBrain brain)
        {
            if(facts == null)
            {
                facts = "";
                for(int i=0; i< brain.instantiablePrefabs.Count;i++)
                {
                    facts += "prefab("+i+",SOMETHING)."+Environment.NewLine;
                }
            }
            return facts;
        }

        public string SpecificFileParts()
        {
            throw new NotImplementedException();
        }
    }
}
