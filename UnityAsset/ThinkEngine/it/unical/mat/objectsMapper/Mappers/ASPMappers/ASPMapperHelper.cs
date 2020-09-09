using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.Mappers
{
    public class ASPMapperHelper
    {
        public static ASPMapperHelper instance;

        public static ASPMapperHelper getInstance()
        {
            if (instance == null)
            {
                instance = new ASPMapperHelper();
            }
            return instance;
        }
        public string buildMapping(string name, char c, string value)
        {
            //MyDebugger.MyDebug(name);
            name = char.ToLower(name[0]) + name.Substring(1);
            for (int i = 1; i < name.Length; i++)
            {
                //MyDebugger.MyDebug(name);
                if (name[i] == '^')
                {
                    name = name.Substring(0,i+1)+ char.ToLower(name[i + 1]) + name.Substring(i+2);
                }
            }
            string mapping = name.Replace('^', '(');
            int size = mapping.Length;
            mapping += value;
            for (int i = 0; i < size; i++)
            {
                if (mapping[i] == '(')
                {
                    mapping += ')';
                }
            }
            return mapping;
        }

        public List<string> buildTemplateMapping(string name, char c)
        {
            //MyDebugger.MyDebug(name);
            List<string> res = new List<string>();
            name = char.ToLower(name[0]) + name.Substring(1);
            for (int i = 1; i < name.Length; i++)
            {
                //MyDebugger.MyDebug(name);
                if (name[i] == '^')
                {
                    name = name.Substring(0, i + 1) + char.ToLower(name[i + 1]) + name.Substring(i + 2);
                }
            }
            string mapping = name.Replace('^', '(');
            int size = mapping.Length;
            res.Add(mapping);
            res.Add("");
            res.Add("");
            for (int i = 0; i < size; i++)
            {
                if (mapping[i] == '(')
                {
                    res[2] += ')';
                }
            }
            return res;
        }
    }
}
