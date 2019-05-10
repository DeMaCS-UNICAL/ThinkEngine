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
                foreach (string matrixPath in s.matrixProperties.Keys)
                {
                    if (s.matrixProperties.ContainsKey(matrixPath))
                    {
                        string keyWithoutDotsAndSpaces = matrixPath.Replace(".", "");
                        keyWithoutDotsAndSpaces = keyWithoutDotsAndSpaces.Replace(" ", "");
                        keyWithoutDotsAndSpaces = keyWithoutDotsAndSpaces.Replace("_", "");
                        
                        SimpleSensor[,] matrix = s.matrixProperties[matrixPath];
                        int r = matrix.GetLength(0), c = matrix.GetLength(1);
                        string sensorNameNotCapital = char.ToLower(s.sensorName[0]) + s.sensorName.Substring(1);
                        //Debug.Log("goname " + s.gOName);
                        string goNameNotCapital = "";
                        if (s.gOName.Length > 0)
                        {
                            goNameNotCapital = char.ToLower(s.gOName[0]) + s.gOName.Substring(1);
                        }
                        string prefix = sensorNameNotCapital + "("+goNameNotCapital+"(";
                        string suffix = ")).";
                        int start = 0;
                        int indexOfCap = keyWithoutDotsAndSpaces.IndexOf('^', start);
                        while (indexOfCap != -1)
                        {
                            string toConcatL = keyWithoutDotsAndSpaces.Substring(start, indexOfCap - start);
                            prefix += char.ToLower(toConcatL[0])+toConcatL.Substring(1) + "(";
                            suffix = ")" + suffix;
                            start = indexOfCap + 1;
                            indexOfCap = keyWithoutDotsAndSpaces.IndexOf('^', start);
                        }
                        string toConcat = keyWithoutDotsAndSpaces.Substring(start, keyWithoutDotsAndSpaces.Length - start);
                        prefix += char.ToLower(toConcat[0]) + toConcat.Substring(1) + "(";
                        suffix = ")" + suffix;
                        for (int i = 0; i < r; i++)
                        {
                            for (int j = 0; j < c; j++)
                            {

                                Type mapperType = typeof(SimpleSensor);
                                string[] innerSensorMapping = manager.getMapper(mapperType).Map(matrix[i, j]).Split(
            new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (string partialMap in innerSensorMapping)
                                {
                                    //Debug.Log(partialMap);
                                    sensorMapping += prefix + i + "," + j + "," + partialMap + suffix + "\n";
                                }

                            }
                        }
                    }
                }
                s.matrixProperties = new Dictionary<string, SimpleSensor[,]>();
                s.dataAvailable = false;
            }
            //Debug.Log("mapping " + sensorMapping);
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
                    rep += ASPMapperHelper.getInstance().buildMapping(keyWithoutDotsAndSpaces, '^', "(X,Y,"+partial+")") + ")";//TODO: improve whene map other types
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
