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
                                instantiator.toInstantiate = brain.instantiablePrefabs[index].gameObject;
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
                    string currentMappings = brain.instantiablePrefabs[i].GetComponent<DCSPrefabConfigurator>().Mapping();
                    facts += string.Format(currentMappings,i);
                }
            }
            return facts;
        }

        public string SpecificFileParts()
        {
            string toReturn = "";
            toReturn += "% Facts assiociated with instantiable DCS Prefab"+Environment.NewLine;
            toReturn += "% "+DCSPrefabConfigurator.Facts;
            toReturn += "% Predicates for Prefab instantiation. PrefabListIndex is the index"+Environment.NewLine;
            toReturn += "% instantiatePrefab(PrefabListIndex,X,Y,Z)." + Environment.NewLine;
            return toReturn;
        }
    }
}
