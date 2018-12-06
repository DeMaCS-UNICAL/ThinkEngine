using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEditor;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.MappingScripts
{
    [Serializable]
    public  class SensorConfiguration
    {

        GameObjectsTracker tracker;
        GameObject gO;
        [SerializeField]
        public string gOName;
        [SerializeField]
        public List<string> properties;
        [SerializeField]
        public string gOType;

        //public SensorConfiguration() { }

        public void SaveSensorConfiguration(GameObjectsTracker tr)
        {
            properties = new List<string>();
            gO = tr.GO;
            gOName = gO.name;
            UnityEngine.Object parent = PrefabUtility.GetCorrespondingObjectFromSource(gO);
            gOType = parent!=null?parent.ToString():"";
            tracker = tr;
            Dictionary<string, FieldOrProperty> gOProperties = tr.ObjectsProperties[gO];
            List<Component> comp = tr.GOComponents[gO];
            foreach (string s in gOProperties.Keys)
            {
                if (tr.ObjectsToggled[gOProperties[s]])
                {
                    Debug.Log("adding " + gOProperties[s].Name());
                    properties.Add(s);
                    if (!tr.IsBaseType(gOProperties[s]))
                    {                        
                        recursevelyAdd(gO, gOProperties[s], "");                        
                    }
                }
            }
            foreach(Component c in comp)
            {
                if (tr.ObjectsToggled[c])
                {
                    Debug.Log("adding " + c.GetType().ToString());
                    properties.Add(c.GetType().ToString());
                    Dictionary<string, FieldOrProperty> componentProperties = tr.ObjectsProperties[c];
                    foreach(string s in componentProperties.Keys)
                    {
                        if (tr.ObjectsToggled[componentProperties[s]])
                        {
                            Debug.Log("adding " + c.GetType().ToString() + "^" + s);
                            properties.Add(c.GetType().ToString()+"^"+s);
                            if (!tr.IsBaseType(componentProperties[s]))
                            {
                                recursevelyAdd(c, componentProperties[s], c.GetType().ToString()+"^");
                            }
                        }
                    }
                }
            }
            
            
        }

        private void recursevelyAdd(object obj, FieldOrProperty fieldOrProperty, string parent)
        {
            object derivedObj = tracker.ObjectDerivedFromFields[obj][fieldOrProperty.Name()];
            Dictionary<string, FieldOrProperty>  derivedObjProperties = tracker.ObjectsProperties[derivedObj];
            foreach (string s in derivedObjProperties.Keys)
            {
                
                if (tracker.ObjectsToggled[derivedObjProperties[s]])
                {
                    Debug.Log(derivedObjProperties[s].Name() + " toggled");
                    Debug.Log("adding " + parent+ fieldOrProperty.Name()+"^" + s);
                    properties.Add(parent+ fieldOrProperty.Name() + "^" + s);
                    if (!tracker.IsBaseType(derivedObjProperties[s]))
                    {
                        Debug.Log("recurse");
                        recursevelyAdd(derivedObj, derivedObjProperties[s], parent+fieldOrProperty.Name()+"^");
                        
                    }
                }
            }
        }
    }
}
