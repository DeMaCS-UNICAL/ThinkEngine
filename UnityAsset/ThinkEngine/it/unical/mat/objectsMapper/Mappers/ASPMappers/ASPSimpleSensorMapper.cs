using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.Mappers
{
    public class ASPSimpleSensorMapper :  IMapper
    {
        public static ASPSimpleSensorMapper instance;

        public static ASPSimpleSensorMapper getInstance()
        {
            if(instance == null)
            {
                instance = new ASPSimpleSensorMapper();
            }
            return instance;
        }
        public string Map(object o)//o is a Sensor
        {
            SimpleSensor s = (SimpleSensor)o;
            String sensorMapping = "";
            lock (s.toLock)
            {
                MappingManager manager = MappingManager.getInstance();
                foreach (IDictionary dictionary in s.dictionaryPerType.Values.Distinct())
                {
                    foreach (DictionaryEntry entry in dictionary)
                    {
                        //Debug.Log(entry.Key);
                        string keyWithoutDotsAndSpaces = ((string)entry.Key).Replace(".", "");
                        keyWithoutDotsAndSpaces = keyWithoutDotsAndSpaces.Replace(" ", "");
                        keyWithoutDotsAndSpaces = keyWithoutDotsAndSpaces.Replace("_", "");
                        if (!s.unityASPVariationNames.ContainsKey(keyWithoutDotsAndSpaces))
                        {
                            s.unityASPVariationNames.Add(keyWithoutDotsAndSpaces, (string)entry.Key);
                        }
                        Type mapperType = entry.Value.GetType().GetGenericArguments()[0];//entry is a List<SOMETHING>
                        DictionaryEntry toMap = new DictionaryEntry();
                        toMap.Key = keyWithoutDotsAndSpaces;
                        string sensorNameNotCapital = char.ToLower(s.sensorName[0]) + s.sensorName.Substring(1);
                        //Debug.Log("goname " + s.gOName);
                        string goNameNotCapital = "";
                        if (s.gOName.Length > 0)
                        {
                            goNameNotCapital = char.ToLower(s.gOName[0]) + s.gOName.Substring(1);
                        }
                        if (s.operationPerProperty.ContainsKey((string)entry.Key))
                        {
                            sensorMapping += sensorNameNotCapital + "(";
                            if (!s.gOName.Equals(""))
                            {
                                sensorMapping += goNameNotCapital + "(";
                            }
                            //Debug.Log(sensorMapping + " " + entry.Value + " " + s.operationPerProperty[(string)toMap.Key]);
                            toMap.Value = Operation.compute(s.operationPerProperty[(string)entry.Key], entry.Value);
                            //Debug.Log("toMap: " + toMap.Key + " " + toMap.Value);
                            sensorMapping += manager.getMapper(mapperType).Map(toMap) + ")";
                            if (!s.gOName.Equals(""))
                            {
                                sensorMapping += ").";
                            }
                            sensorMapping += Environment.NewLine;
                        }
                        //Debug.Log("sensorMapping done for " + sensorMapping);
                    }
                }
                foreach (IDictionary dic in s.dictionaryPerType.Values.Distinct())
                {
                    dic.Clear();
                }
            }
            return sensorMapping;
        }


        public string basicMap(object o)
        {
            return "";
        }

        public string getASPRepresentation(SimpleSensor s)
        {
            String sensorMapping = "";
            foreach (string p in s.properties.Distinct())
            {
                if (s.operationPerProperty.ContainsKey(p))
                {
                    sensorMapping += "%";
                    string keyWithoutDotsAndSpaces = p.Replace(".", "");
                    keyWithoutDotsAndSpaces = keyWithoutDotsAndSpaces.Replace(" ", "");
                    keyWithoutDotsAndSpaces = keyWithoutDotsAndSpaces.Replace("_", "");
                    string sensorNameNotCapital = char.ToLower(s.sensorName[0]) + s.sensorName.Substring(1);
                    //Debug.Log("goname " + s.gOName);
                    string goNameNotCapital = "";
                    if (s.gOName.Length > 0)
                    {
                        goNameNotCapital = char.ToLower(s.gOName[0]) + s.gOName.Substring(1);
                    }
                
                    sensorMapping += sensorNameNotCapital + "(";
                    if (!s.gOName.Equals(""))
                    {
                        sensorMapping += goNameNotCapital + "(";
                    }
                    sensorMapping += ASPMapperHelper.getInstance().buildMapping(keyWithoutDotsAndSpaces, '^', "(X)") + ")";
                    if (!s.gOName.Equals(""))
                    {
                        sensorMapping += ").";
                    }
                    sensorMapping += Environment.NewLine;
                }
            }
            return sensorMapping;
        }

        internal Dictionary<string, List<string>> getTemplateASPRepresentation(SimpleSensor s)
        {
            Dictionary<string, List<string>> sensorMapping = new Dictionary<string, List<string>>();
            foreach (string p in s.properties.Distinct())
            {
                sensorMapping.Add(p, new List<string>());
                if (s.operationPerProperty.ContainsKey(p))
                {
                    string keyWithoutDotsAndSpaces = p.Replace(".", "");
                    keyWithoutDotsAndSpaces = keyWithoutDotsAndSpaces.Replace(" ", "");
                    keyWithoutDotsAndSpaces = keyWithoutDotsAndSpaces.Replace("_", "");
                    string sensorNameNotCapital = char.ToLower(s.sensorName[0]) + s.sensorName.Substring(1);
                    //Debug.Log("goname " + s.gOName);
                    string goNameNotCapital = "";
                    if (s.gOName.Length > 0)
                    {
                        goNameNotCapital = char.ToLower(s.gOName[0]) + s.gOName.Substring(1);
                    }

                    sensorMapping[p].Add(sensorNameNotCapital + "(");
                    if (!s.gOName.Equals(""))
                    {
                        sensorMapping[p][0] += goNameNotCapital + "(";
                    }
                    List<string> temp = ASPMapperHelper.getInstance().buildTemplateMapping(keyWithoutDotsAndSpaces, '^');
                    sensorMapping[p][0] += temp[0];
                    sensorMapping[p].Add("");
                    sensorMapping[p].Add(temp[temp.Count - 1] + ")");
                    if (!s.gOName.Equals(""))
                    {
                        sensorMapping[p][2] += ").";
                    }
                }
            }
            return sensorMapping;
        }
    }
}
