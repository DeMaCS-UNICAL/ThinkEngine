using it.unical.mat.embasp.languages.asp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts.DCS;
using UnityEngine;

namespace ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts.Specialitations.ASP
{
    internal class ASPDCSBrain : IActualDCSBrain
    {
        string facts;
        string updatePattern = @"^Update\(([^, ()]+(\([^()]*\)[^,()]*)?)+,?([^, ()]+(\([^()]*\)[^,()]*)?)+\)$";
        Regex _regex;
        Regex Regex
        {
            get
            {
                if(_regex == null)
                {
                    _regex = new Regex(updatePattern);
                }
                return _regex;
            }
        }
        public string ActualSensorEncoding(string sensorsAsASP)
        {
            return sensorsAsASP;
        }

        public void ContentReady(object content, ContentBrain brain)
        {
            if (!(content is AnswerSet))
            {
                return;
            }
            AnswerSetAvailable((AnswerSet)content, brain);
        }

        private void AnswerSetAvailable(AnswerSet content, ContentBrain brain)
        {
            foreach (string literal in content.GetAnswerSet())
            {
                float tileWidth = brain.tileWidth;
                float tileHeight = brain.tileHeight;
                /*string substring = "instantiatePrefab(";
                if (literal.StartsWith(substring))
                {
                    string temp = literal.Substring(substring.Length);
                    temp = temp.Remove(temp.Length - 1, 1);
                    string[] values = temp.Split(',');
                    if (values.Length > 0)
                    {
                        if (int.TryParse(values[0], out int index))
                        {
                            if (DCSPrefabConfigurator.instances.ContainsKey(index))
                            {
                                PrefabInstantiator instantiator = Utility.PrefabInstantiator;
                                float.TryParse(values[1], out float pX);
                                float.TryParse(values[2], out float pY);
                                float.TryParse(values[3], out float pZ);
                                float.TryParse(values[1], out float rX);
                                float.TryParse(values[2], out float rY);
                                float.TryParse(values[3], out float rZ);
                                float.TryParse(values[3], out float rW);
                                instantiator.InstantiatePrefab( index, new Vector3(pX,pY,pZ), new Quaternion(rX,rY,rZ,rW));
                            }
                        }
                    }
                
                }*/
                PrefabInstantiator instantiator = Utility.PrefabInstantiator;
                string substring = "current_asset(";
                if (literal.StartsWith(substring))
                {
                    string temp = literal.Substring(substring.Length);
                    temp = temp.Remove(temp.Length - 1, 1);
                    string[] values = temp.Split(',');
                    if (values.Length == 3)
                    {
                        if (int.TryParse(values[0], out int stripe) && int.TryParse(values[1], out int tile) && int.TryParse(values[2], out int index))
                        {
                            if (ContentPrefabConfigurator.instances.ContainsKey(index))
                            {
                                float pX = stripe * tileWidth-tileWidth/2;
                                float pY = brain.sceneHeight*tileHeight - tile * tileHeight+tileHeight/2;
                                //Debug.Log("Requisting to instantiate in " + pX + " " + pY);
                                instantiator.InstantiatePrefab(index, new Vector3(pX, pY, 0), new Quaternion(0, 0, 0, 0));
                            }
                        }
                    }
                }
                else if (literal.StartsWith("Add("))
                {
                    string temp = literal.Remove(0, 4);
                    temp = temp.Remove(temp.Length - 1, 1);
                    //brain.AddFact(temp);
                    brain.FactsToAdd(temp);
                }
                else if (literal.StartsWith("Delete("))
                {
                    string temp = literal.Remove(0, 7);
                    temp = temp.Remove(temp.Length - 1, 1);
                    //brain.DeleteFact(temp);
                    brain.FactsToDelete(temp);
                }
                else if (literal.StartsWith("Update("))
                {
                    Match match = Regex.Match(literal);
                    if (match.Success)
                    {
                        //brain.UpdateFact(match.Groups[1].Value,match.Groups[3].Value);
                        brain.FactsToUpdate(match.Groups[1].Value, match.Groups[3].Value);
                    }
                }
            }
            brain.ApplyChangesToFacts();
        }

        public Executor GetDCSExecutor(ContentBrain dCSBrain)
        {
            return new ASPDCSExecutor(dCSBrain);
        }

        public string PrefabFacts(ContentBrain brain)
        {
            if(facts == null)
            {
                facts = "";
                foreach(ContentPrefabConfigurator configurator in ContentPrefabConfigurator.instances.Values)
                {
                    facts += configurator.Mapping();
                }
            }
            return facts;
        }

        public string SpecificFileParts()
        {
            string toReturn = "";
            toReturn += "% Facts assiociated with instantiable DCS Prefab"+Environment.NewLine;
            toReturn += ContentPrefabConfigurator.Facts;
            toReturn += "% Predicates for Prefab instantiation. PrefabListIndex is the index of the Prefabs list of the Brain, PX PY PZ reflect the position of the instantiation while RX RY RZ RW represent the rotation."+Environment.NewLine;
            toReturn += "% instantiatePrefab(PrefabListIndex,PX,PY,PZ, RX, RY, RZ, RW)." + Environment.NewLine;
            return toReturn;
        }

    }
}
