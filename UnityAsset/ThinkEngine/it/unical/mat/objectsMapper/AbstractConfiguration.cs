using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEditor;
namespace EmbASP4Unity.it.unical.mat.objectsMapper
{
    [Serializable]
    public abstract class AbstractConfiguration :ScriptableObject
    {
        protected GameObjectsTracker tracker;
        private GameObject gO;
        [SerializeField]
        public string gOName;
        [SerializeField]
        public List<string> properties;
        [SerializeField]
        public List<string> propertiesNames;
        [SerializeField]
        public string gOType;
        [SerializeField]
        public string configurationName;
        [SerializeField]
        public List<SimpleGameObjectsTracker> advancedConf;

        public void SaveConfiguration(GameObjectsTracker tr)
        {
            properties = new List<string>();
            propertiesNames = new List<string>();
            advancedConf = new List<SimpleGameObjectsTracker>();
            cleanSpecificDataStructure();
            gO = tr.GO;
            gOName = gO.name;
            configurationName = tr.configurationName;
            UnityEngine.Object parent = PrefabUtility.GetCorrespondingObjectFromSource(gO);
            gOType = parent != null ? parent.ToString() : "";
            tracker = tr;
            Dictionary<string, FieldOrProperty> gOProperties = tr.ObjectsProperties[gO];
            List<Component> comp = tr.GOComponents[gO];
            foreach (string s in gOProperties.Keys)
            {
                
                if (tr.ObjectsToggled[gOProperties[s]])
                {
                    //MyDebugger.MyDebug("property " + s + " toggled");
                    if (tracker.IsMappable(gOProperties[s]))
                    {
                        //MyDebugger.MyDebug("adding " + gOProperties[s].Name());
                        properties.Add(s);
                        if (!tracker.IsBaseType(gOProperties[s]))
                        {
                            tracker.basicTypeCollectionsConfigurations[gOProperties[s]].propertyName = s;
                            advancedConf.Add(tracker.basicTypeCollectionsConfigurations[gOProperties[s]]);
                        }
                        else
                        {
                            specificConfiguration(gOProperties[s], s);
                        }
                    }
                    else if (tracker.ObjectDerivedFromFields.ContainsKey(gO))
                    {
                        //MyDebugger.MyDebug("recursing on " + gOProperties[s].Name());
                        recursevelyAdd(gO, gOProperties[s], "");
                    }
                }
            }
            foreach (Component c in comp)
            {
                if (tr.ObjectsToggled[c])
                {
                    //MyDebugger.MyDebug("adding " + c.GetType().ToString());
                   // properties.Add(c.GetType().ToString());
                    Dictionary<string, FieldOrProperty> componentProperties = tr.ObjectsProperties[c];
                    foreach (string s in componentProperties.Keys)
                    {
                        if (tr.ObjectsToggled[componentProperties[s]])
                        {
                            if (tracker.IsMappable(componentProperties[s]))
                            {
                                string adding = c.GetType() + "^" + s;
                                // MyDebugger.MyDebug("adding " + c.GetType().ToString() + "^" + s);
                                properties.Add(adding);
                                if (tracker.IsBaseType(componentProperties[s]))
                                {
                                    specificConfiguration(componentProperties[s], adding);
                                }
                                int startFrom = 0;
                                while (startFrom < adding.Length - 1 && adding.IndexOf("^", startFrom) != -1)
                                {
                                    int currentIndex = adding.IndexOf("^", startFrom);
                                    properties.Add(adding.Substring(0, currentIndex));
                                    startFrom = currentIndex + 1;
                                }
                                properties.Add(adding);
                                if (!tracker.IsBaseType(componentProperties[s]))
                                {
                                    tracker.basicTypeCollectionsConfigurations[componentProperties[s]].propertyName = adding;
                                    advancedConf.Add(tracker.basicTypeCollectionsConfigurations[componentProperties[s]]);
                                }
                            }
                            else if (tracker.ObjectDerivedFromFields.ContainsKey(c))// && !tracker.IsBaseType(componentProperties[s]))
                            {
                                //MyDebugger.MyDebug("recursing on " + c.name);
                                recursevelyAdd(c, componentProperties[s], c.GetType() + "^");
                            }

                        }
                        
                    }
                }
            }
            if (properties.Count == 0)
            {
                throw new Exception("No properties selected, invalid configuration to save.");
            }
            //MyDebugger.MyDebug("success");
        }

        

        protected void recursevelyAdd(object obj, FieldOrProperty fieldOrProperty, string parent)
        {
            object derivedObj = tracker.ObjectDerivedFromFields[obj][fieldOrProperty.Name()];
            if (derivedObj == null || (tracker.ObjectsOwners.ContainsKey(derivedObj) && !tracker.ObjectsOwners[derivedObj].Key.Equals(obj)) || !tracker.ObjectsOwners[derivedObj].Value.Equals(fieldOrProperty.Name()) || derivedObj.Equals(tracker.GO))
            {
               // MyDebugger.MyDebug(fieldOrProperty.Name()+" returning ");
                return;
            }
            if (tracker.ObjectsProperties.ContainsKey(derivedObj))
            {
                Dictionary<string, FieldOrProperty> derivedObjProperties = tracker.ObjectsProperties[derivedObj];
                foreach (string s in derivedObjProperties.Keys)
                {

                    if (tracker.ObjectsToggled[derivedObjProperties[s]])
                    {
                        //MyDebugger.MyDebug(s + " is toggled");
                        if (tracker.IsMappable(derivedObjProperties[s]))
                        {
                            //MyDebugger.MyDebug(derivedObjProperties[s].Name() + " toggled");
                            // MyDebugger.MyDebug("adding " + parent + fieldOrProperty.Name() + "^" + s);
                            string adding = parent + fieldOrProperty.Name() + "^" + s;
                            properties.Add(adding);
                            if (tracker.IsBaseType(derivedObjProperties[s]))
                            {
                                specificConfiguration(derivedObjProperties[s], adding);
                            }
                            int startFrom = 0;
                            while (startFrom < adding.Length && adding.IndexOf("^", startFrom) != adding.LastIndexOf("^"))
                            {
                                int currentIndex = adding.IndexOf("^", startFrom);
                                properties.Add(adding.Substring(0, currentIndex));
                                startFrom = currentIndex + 1;
                            }
                            if (!tracker.IsBaseType(derivedObjProperties[s]))
                            {
                                tracker.basicTypeCollectionsConfigurations[derivedObjProperties[s]].propertyName = adding;
                                advancedConf.Add(tracker.basicTypeCollectionsConfigurations[derivedObjProperties[s]]);
                            }
                        }

                        else if (tracker.ObjectDerivedFromFields.ContainsKey(derivedObj) && !tracker.IsBaseType(derivedObjProperties[s]))
                        {
                            //MyDebugger.MyDebug("recursin on " + parent + fieldOrProperty.Name() + "^" + derivedObjProperties[s].Name());

                            recursevelyAdd(derivedObj, derivedObjProperties[s], parent + fieldOrProperty.Name() + "^");

                        }
                    }
                }
            }
        }
        internal abstract void specificConfiguration(FieldOrProperty fieldOrProperty, string s);
        internal abstract void cleanSpecificDataStructure();
    }
    
}
