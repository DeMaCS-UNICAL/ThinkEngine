using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.Mappers
{
    class ASPAdvancedSensorMapper : ScriptableObject, IMapper
    {

        public static ASPAdvancedSensorMapper instance;
        public string Map(object o)//o is a Sensor
        {
            AdvancedSensor s = (AdvancedSensor)o;
            String sensorMapping = "";
            lock (s.toLock)
            {
                MappingManager manager = MappingManager.getInstance();
                //Debug.Log("mapping " + s.sensorName);
                sensorMapping = manager.getMapper(typeof(SimpleSensor)).Map(s);
                //Debug.Log("mapped as simple "+sensorMapping);
                sensorMapping = matrixMapping(s, sensorMapping, manager);
                sensorMapping = listMapping(s, sensorMapping, manager);

                s.dataAvailable = false;
            }
            //Debug.Log(s.sensorName+" sensor mapping: " + sensorMapping);
            return sensorMapping;
        }

        public string basicMap(object o)
        {
            return "";
        }

        private static string matrixMapping(AdvancedSensor s, string sensorMapping, MappingManager manager)
        {
            foreach (string matrixPath in s.matrixProperties.Keys)
            {
                //Debug.Log(matrixPath);
                if (s.matrixProperties.ContainsKey(matrixPath))
                {
                    //Debug.Log("matrix found");
                    string keyWithoutDotsAndSpaces = matrixPath.Replace(".", "");
                    keyWithoutDotsAndSpaces = keyWithoutDotsAndSpaces.Replace(" ", "");
                    keyWithoutDotsAndSpaces = keyWithoutDotsAndSpaces.Replace("_", "");

                    SimpleSensor[,] matrix = s.matrixProperties[matrixPath];
                    //Debug.Log("matrix size "+matrix.GetLength(0) + " " + matrix.GetLength(1));
                    int r = matrix.GetLength(0), c = matrix.GetLength(1);
                    string sensorNameNotCapital = char.ToLower(s.sensorName[0]) + s.sensorName.Substring(1);
                    
                    //Debug.Log("goname " + s.gOName);
                    string goNameNotCapital = "";
                    if (s.gOName.Length > 0)
                    {
                        goNameNotCapital = char.ToLower(s.gOName[0]) + s.gOName.Substring(1);
                    }
                    string prefix = sensorNameNotCapital + "(" + goNameNotCapital + "(";
                    string suffix = ")).";
                    int start = 0;
                    int indexOfCap = keyWithoutDotsAndSpaces.IndexOf('^', start);
                    while (indexOfCap != -1)
                    {
                        string toConcatL = keyWithoutDotsAndSpaces.Substring(start, indexOfCap - start);
                        prefix += char.ToLower(toConcatL[0]) + toConcatL.Substring(1) + "(";
                        suffix = ")" + suffix;
                        start = indexOfCap + 1;
                        indexOfCap = keyWithoutDotsAndSpaces.IndexOf('^', start);
                    }
                    string toConcat = keyWithoutDotsAndSpaces.Substring(start, keyWithoutDotsAndSpaces.Length - start);
                    prefix += char.ToLower(toConcat[0]) + toConcat.Substring(1) + "(";
                    suffix = ")" + suffix;
                    //Debug.Log("prefix " + prefix + " suffix " + suffix);
                    for (int i = 0; i < r; i++)
                    {
                        for (int j = 0; j < c; j++)
                        {

                            Type mapperType = typeof(SimpleSensor);
                            string[] innerSensorMapping = manager.getMapper(mapperType).Map(matrix[i, j]).Split(
        new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                            //Debug.Log("innerSensorMapping count " + innerSensorMapping.Length);
                            foreach (string partialMap in innerSensorMapping)
                            {
                                //Debug.Log(partialMap);
                                sensorMapping += prefix + i + "," + j + "," + partialMap + suffix + "\n";

                            }

                        }
                    }
                }
            }
            //Debug.Log(sensorMapping);

            //s.matrixProperties = new Dictionary<string, SimpleSensor[,]>();
            return sensorMapping;
        }

        private static string listMapping(AdvancedSensor s, string sensorMapping, MappingManager manager)
        {
            foreach (string listPath in s.listProperties.Keys)
            {
                //Debug.Log(listPath);
                if (s.listProperties.ContainsKey(listPath))
                {
                    string keyWithoutDotsAndSpaces = listPath.Replace(".", "");
                    keyWithoutDotsAndSpaces = keyWithoutDotsAndSpaces.Replace(" ", "");
                    keyWithoutDotsAndSpaces = keyWithoutDotsAndSpaces.Replace("_", "");

                    List<SimpleSensor> list = s.listProperties[listPath];
                    //Debug.Log(list.Count);
                    string sensorNameNotCapital = char.ToLower(s.sensorName[0]) + s.sensorName.Substring(1);
                    //Debug.Log("goname " + s.gOName);
                    string goNameNotCapital = "";
                    if (s.gOName.Length > 0)
                    {
                        goNameNotCapital = char.ToLower(s.gOName[0]) + s.gOName.Substring(1);
                    }
                    string prefix = sensorNameNotCapital + "(" + goNameNotCapital + "(";
                    string suffix = ")).";
                    int start = 0;
                    int indexOfCap = keyWithoutDotsAndSpaces.IndexOf('^', start);
                    while (indexOfCap != -1)
                    {
                        string toConcatL = keyWithoutDotsAndSpaces.Substring(start, indexOfCap - start);
                        prefix += char.ToLower(toConcatL[0]) + toConcatL.Substring(1) + "(";
                        suffix = ")" + suffix;
                        start = indexOfCap + 1;
                        indexOfCap = keyWithoutDotsAndSpaces.IndexOf('^', start);
                    }
                    string toConcat = keyWithoutDotsAndSpaces.Substring(start, keyWithoutDotsAndSpaces.Length - start);
                    prefix += char.ToLower(toConcat[0]) + toConcat.Substring(1) + "(";
                    suffix = ")" + suffix;
                    for (int i = 0; i < list.Count; i++)
                    {
                            Type mapperType = typeof(SimpleSensor);
                            string[] innerSensorMapping = manager.getMapper(mapperType).Map(list[i]).Split(
        new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string partialMap in innerSensorMapping)
                            {
                               // Debug.Log(partialMap);
                                sensorMapping += prefix + i + "," + partialMap + suffix + "\n";
                             }
                    }
                }
            }

            //s.matrixProperties = new Dictionary<string, SimpleSensor[,]>();
            //Debug.Log(sensorMapping);

            return sensorMapping;
        }

        internal static IMapper getInstance()
        {
            if (instance == null)
            {
                instance = new ASPAdvancedSensorMapper();
            }
            return instance;
        }

        public Dictionary<string,List<string>> getTemplateASPRepresentation(AdvancedSensor s)
        {
            MappingManager manager = MappingManager.getInstance();
            Dictionary<string,List<string>> rep = ((ASPSimpleSensorMapper)manager.getMapper(typeof(SimpleSensor))).getTemplateASPRepresentation(s);
            foreach (string p in s.advancedConf.Keys.Distinct())
            {
                List<string> elementConf = s.advancedConf[p].toSave;
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

                foreach (string p2 in elementConf)
                {
                    rep.Add(p + "$" + p2, new List<string>()); //each property of the inner element is referred as propertyName$elementPropertyName
                    List<string> partial = rep[p + "$" + p2];
                    partial.Add(sensorNameNotCapital + "(");
                    if (!s.gOName.Equals(""))
                    {
                        partial[0]+=goNameNotCapital + "(";
                    }
                    string elemType = s.advancedConf[p].name;
                    elemType = Char.ToLower(elemType[0]) + elemType.Substring(1);

                    List<string> inner = ASPMapperHelper.getInstance().buildTemplateMapping(p2, '^');
                    inner[0] = elemType+"("+inner[0];
                    inner[inner.Count - 1] += ")";

                    List<string> temp = ASPMapperHelper.getInstance().buildTemplateMapping(keyWithoutDotsAndSpaces, '^');
                    partial[0] = partial[0] + temp[0] + "(";
                    partial.Add(inner[0]);
                    partial.Add(inner[inner.Count - 1] + temp[temp.Count - 1] + "))");
                    /*Debug.Log("Advanced conf " + p);
                    if (s.matrixProperties.Count > 0)
                    {
                        //Debug.Log("matrix " + s.matrixProperties.First());
                    }
                    if (s.listProperties.Count > 0)
                    {
                        //Debug.Log("list " + s.listProperties.First());
                    }
                    if (s.matrixProperties.ContainsKey(p))
                    {
                        //Debug.Log("is a matrix");
                        List<string> temp= ASPMapperHelper.getInstance().buildTemplateMapping(keyWithoutDotsAndSpaces, '^');//TODO: improve whene map other types
                        partial[0] = partial[0] + temp[0]+"(" + inner[0];
                        partial.Add("");
                        partial.Add(inner[inner.Count - 1] + temp[temp.Count - 1] + "))");

                    }
                    else if (s.listProperties.ContainsKey(p))
                    {
                        //Debug.Log("is a list");
                        rep += ASPMapperHelper.getInstance().buildMapping(keyWithoutDotsAndSpaces, '^', "(X," + partial + ")") + ")";//TODO: improve whene map other types

                    }*/
                    if (!s.gOName.Equals(""))
                    {
                         partial[partial.Count-1]+= ").";
                    }

                }
            }
            return rep;
        }

        public string getASPRepresentation(AdvancedSensor s)
        {
            MappingManager manager = MappingManager.getInstance();
            string rep = ((ASPSimpleSensorMapper)manager.getMapper(typeof(SimpleSensor))).getASPRepresentation(s);
            foreach (string p in s.advancedConf.Keys.Distinct())
            {
                List<string> elementConf = s.advancedConf[p].toSave;
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

                foreach (string p2 in elementConf) {
                    rep += "%" + sensorNameNotCapital + "(";
                    if (!s.gOName.Equals(""))
                    {
                        rep += goNameNotCapital + "(";
                    }
                    string elemType = s.advancedConf[p].name;
                    elemType = Char.ToLower(elemType[0])+elemType.Substring(1);
                    string partial = elemType+"("+ ASPMapperHelper.getInstance().buildMapping(p2, '^', "(V)")+")";
                    //Debug.Log("Advanced conf " + p);
                    if (s.matrixProperties.Count > 0)
                    {
                        //Debug.Log("matrix " + s.matrixProperties.First());
                    }
                    if (s.listProperties.Count > 0)
                    {
                        //Debug.Log("list " + s.listProperties.First());
                    }
                    if (s.matrixProperties.ContainsKey(p))
                    {
                        //Debug.Log("is a matrix");
                        rep += ASPMapperHelper.getInstance().buildMapping(keyWithoutDotsAndSpaces, '^', "(X,Y," + partial + ")") + ")";//TODO: improve whene map other types

                    }
                    else if (s.listProperties.ContainsKey(p))
                    {
                        //Debug.Log("is a list");
                        rep += ASPMapperHelper.getInstance().buildMapping(keyWithoutDotsAndSpaces, '^', "(X," + partial + ")") + ")";//TODO: improve whene map other types

                    }
                    if (!s.gOName.Equals(""))
                    {
                        rep += ").";
                    }
                    rep += Environment.NewLine;
                }
            }
            return rep;
        }
    }
    
}
