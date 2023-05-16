using it.unical.mat.embasp.languages.asp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts.DCS;
using UnityEngine;

namespace ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts.Specializations.ASP
{
    internal class ASPDCSBrain : IActualDCSBrain
    {
        string facts;
        string updatePattern = @"^Update\(([^, ()]+(\([^()]*\)[^,()]*)?)+,?([^, ()]+(\([^()]*\)[^,()]*)?)+\)$";
        string customInstantiationPattern = @"([^()]+)(\(([^, ()]+(\([^()]*\))?)(,([^, ()]+(\([^()]*\))?))*\))*";
        Regex _updateRegex;
        Regex UpdateRegex
        {
            get
            {
                if(_updateRegex == null)
                {
                    _updateRegex = new Regex(updatePattern);
                }
                return _updateRegex;
            }
        }
        Regex _customInstRegex;
        Regex CustomInstRegex
        {
            get
            {
                if (_customInstRegex == null)
                {
                    _customInstRegex = new Regex(customInstantiationPattern);
                }
                return _customInstRegex;
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
                PrefabInstantiator instantiator = Utility.PrefabInstantiator;
                if (literal.StartsWith("custom_instantiation("))
                {
                    string toMatch = literal.Substring(21, literal.Length - 22);
                    CustomInstantiator customInst = brain.customInstantiator;
                    if (customInst  != null)
                    {
                        Match match = CustomInstRegex.Match(toMatch);
                        if (match.Success)
                        {
                            string[] arguments = new string[match.Groups[6].Captures.Count+1];
                            arguments[0] = match.Groups[3].Value;
                            for (int i=0; i<match.Groups[6].Captures.Count;i++)
                            {
                                arguments[i+1] = match.Groups[6].Captures[i].Value;
                            }
                            customInst.ParseLiteral(match.Groups[1].Value,arguments);
                        }
                        //customInst.ParseLiteral(literal);
                    }
                    else
                    {
                        Debug.LogError("custom_instantiation required but a CustomInstantiator is missing.");
                    }
                }
                /*else if (literal.StartsWith("current_asset("))
                {
                    CheckCurrentAsset(brain, literal, tileWidth, tileHeight, instantiator);
                }
                else if (literal.StartsWith("reachable(tile("))
                {
                    CheckReachable(brain, literal, tileWidth, tileHeight, instantiator);
                }
                else if (literal.StartsWith("has_state(tile("))
                {
                    CheckHasState(brain, literal, tileWidth, tileHeight, instantiator);
                }*/
                else if (literal.StartsWith("Add("))
                {
                    string temp = literal.Remove(0, 4);
                    temp = temp.Remove(temp.Length - 1, 1);
                    brain.FactsToAdd(temp);
                }
                else if (literal.StartsWith("Delete("))
                {
                    string temp = literal.Remove(0, 7);
                    temp = temp.Remove(temp.Length - 1, 1);
                    brain.FactsToDelete(temp);
                }
                else if (literal.StartsWith("Update("))
                {
                    Match match = UpdateRegex.Match(literal);
                    if (match.Success)
                    {
                        brain.FactsToUpdate(match.Groups[1].Value, match.Groups[3].Value);
                    }
                }
            }
            brain.ApplyChangesToFacts();
        }
        /*
        private static string CheckHasState(ContentBrain brain, string literal, float tileWidth, float tileHeight, PrefabInstantiator instantiator)
        {
            string substring = "has_state(tile(";
            string temp = literal.Substring(substring.Length);
            temp = temp.Remove(temp.Length - 1, 1);
            string[] values = temp.Split(',');
            if (values.Length == 3)
            {
                values[1] = values[1].Remove(values[1].Length - 1);
                if (int.TryParse(values[0], out int stripe) && int.TryParse(values[1], out int tile))
                {
                    float pX = stripe * tileWidth - tileWidth / 2;
                    float pY = brain.sceneHeight * tileHeight - tile * tileHeight + tileHeight / 2;
                    //Debug.Log("Requisting to instantiate in " + pX + " " + pY);
                    instantiator.InstantiateCircle(new Vector3(pX, pY, 0), values[2]);
                }
            }

            return substring;
        }

        private static string CheckReachable(ContentBrain brain, string literal, float tileWidth, float tileHeight, PrefabInstantiator instantiator)
        {
            string substring = "reachable(tile(";
            string temp = literal.Substring(substring.Length);
            temp = temp.Remove(temp.Length - 2, 2);
            string[] values = temp.Split(',');
            if (values.Length == 2)
            {
                if (int.TryParse(values[0], out int stripe) && int.TryParse(values[1], out int tile))
                {
                    float pX = stripe * tileWidth - tileWidth / 2;
                    float pY = brain.sceneHeight * tileHeight - tile * tileHeight + tileHeight / 2;
                    //Debug.Log("Requisting to instantiate in " + pX + " " + pY);
                    instantiator.InstantiateCircle(new Vector3(pX, pY, 0), "reach");
                }
            }

            return substring;
        }

        private static string CheckCurrentAsset(ContentBrain brain, string literal, float tileWidth, float tileHeight, PrefabInstantiator instantiator)
        {
            string substring = "current_asset(";
            string temp = literal.Substring(substring.Length);
            temp = temp.Remove(temp.Length - 1, 1);
            string[] values = temp.Split(',');
            if (values.Length == 3)
            {
                if (int.TryParse(values[0], out int stripe) && int.TryParse(values[1], out int tile) && int.TryParse(values[2], out int index))
                {
                    if (ContentPrefabConfigurator.instances.ContainsKey(index))
                    {
                        float pX = stripe * tileWidth - tileWidth / 2;
                        float pY = brain.sceneHeight * tileHeight - tile * tileHeight + tileHeight / 2;
                        //Debug.Log("Requisting to instantiate in " + pX + " " + pY);
                        instantiator.InstantiatePrefab(index, new Vector3(pX, pY, 0), new Quaternion(0, 0, 0, 0));
                    }
                }
            }

            return substring;
        }
        */
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
