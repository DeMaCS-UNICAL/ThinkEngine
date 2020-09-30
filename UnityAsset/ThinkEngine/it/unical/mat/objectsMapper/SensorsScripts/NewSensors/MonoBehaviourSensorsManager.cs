﻿using UnityEngine;
using System.Collections;
using EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class MonoBehaviourSensorsManager : MonoBehaviour
{
    public Brain brain;
    private int step=0;
    public bool ready;
    public List<SensorConfiguration> configurations;
    private string currentPropertyType;
    private string currentSubProperty;
    private int sensorsAdded = 0;
    private Dictionary<List<string>, string> typeForCollectionProperty;
    private Dictionary<List<string>, string> sensorNameForCollectionProperty;
    private Dictionary<List<string>, List<string>> elementForCollectionProperty;
    private Dictionary<List<string>, List<int>> sizeToTrack;
    public bool executeRepeteadly;
    public float frequence;
    public MethodInfo updateMethod;
    internal object triggerClass;

    // Use this for initialization
    void Awake()
    {
        ////MyDebugger.MyDebug("Awakening");
        //Debug.unityLogger.logEnabled = true;
        sizeToTrack = new Dictionary<List<string>, List<int>>();
        typeForCollectionProperty = new Dictionary<List<string>, string>();
        elementForCollectionProperty = new Dictionary<List<string>, List<string>>();
        sensorNameForCollectionProperty = new Dictionary<List<string>, string>();
        configurations = new List<SensorConfiguration>();
    }

    void Start()
    {
        //Debug.unityLogger.logEnabled = true;
    }

    public List<IMonoBehaviourSensor> generateSensors()
    {
        List<IMonoBehaviourSensor> generatedSensors = new List<IMonoBehaviourSensor>();
        foreach (SensorConfiguration conf in configurations)
        {
            foreach(List<string> property in conf.properties.Distinct())
            {
                MyDebugger.MyDebug("ADDING SENSOR FOR " + property + " TO " + gameObject.name);
                object[] result=null;
                Type type=null;
                currentSubProperty = "";
                int currentOperationPerProperty = 0;
                currentPropertyType = "VALUE";//VALUE, ARRAY2, LIST
                foreach (SimpleGameObjectsTracker tracker in conf.advancedConf)
                {
                    if (tracker.propertyName.Equals(property)){
                        currentPropertyType= tracker.propertyType;
                        foreach(string subproperty in tracker.toSave)
                        {
                            MyDebugger.MyDebug("SUBPROPERTY " + subproperty);
                            currentSubProperty = subproperty;
                            result = ReadProperty(property);
                            configureSensor(result, type, conf, property, currentOperationPerProperty, generatedSensors);
                        }
                        break;
                    }
                }
                if (currentPropertyType.Equals("VALUE"))
                {
                    result = ReadProperty(property);
                    configureSensor(result, type, conf, property, currentOperationPerProperty, generatedSensors);
                    foreach (ListOfStringIntPair pair in conf.operationPerProperty)
                    {
                        if (pair.Key.Equals(property))
                        {
                            currentOperationPerProperty = pair.Value;
                            break;
                        }
                    }
                }
                
            }
        }
        ready = true;
        return generatedSensors;
    }

    private void configureSensor(object[] result, Type type, SensorConfiguration conf, List<string> property, int currentOperationPerProperty,List<IMonoBehaviourSensor> generatedSensors)
    {
        //MyDebugger.MyDebug("configuring sensors for " + property);
        if (result != null)
        {
            type = (Type)result[0];
        }
        if (type != null && currentPropertyType.Equals("VALUE"))
        {
            IMonoBehaviourSensor sensor = addSensor(conf.name, property, currentOperationPerProperty, type);
            //MyDebugger.MyDebug("AND IT'S VALUE");
            sensor.done();
            generatedSensors.Add(sensor);
        }
        else if (type != null)
        {
            List<int> currentSize = new List<int>();
            if (currentPropertyType.Equals("LIST"))
            {
                //MyDebugger.MyDebug("AND IT'S LIST");
                currentSize.Add((int)result[1]);
                for (int i = 0; i < (int)result[1]; i++)
                {
                    IMonoBehaviourSensor sensor = addSensor(conf.name, property, currentOperationPerProperty, type);
                    sensor.indexes.Add(i);
                    sensor.collectionElementProperty = currentSubProperty;
                    sensor.collectionElementType = ((Type)result[2]).Name;
                    sensor.done();
                    generatedSensors.Add(sensor);
                }
            }
            else if (currentPropertyType.Equals("ARRAY2"))
            {
                MyDebugger.MyDebug("AND IT'S ARRAY2");
                currentSize.Add((int)result[1]);
                currentSize.Add((int)result[2]);
                for (int i = 0; i < (int)result[1]; i++)
                {
                    for (int j = 0; j < (int)result[2]; j++)
                    {
                        IMonoBehaviourSensor sensor = addSensor(conf.name, property, currentOperationPerProperty, type);
                        sensor.indexes.Add(i);
                        sensor.indexes.Add(j);
                        sensor.collectionElementProperty = currentSubProperty;
                        sensor.collectionElementType = ((Type)result[3]).Name;
                        sensor.done();
                        generatedSensors.Add(sensor);
                    }
                }
            }
            if (!sizeToTrack.ContainsKey(property))
            {
                //MyDebugger.MyDebug("property " + property);
                //MyDebugger.MyDebug("size " + currentSize.Count);
                sizeToTrack.Add(property, currentSize);
                typeForCollectionProperty.Add(property, currentPropertyType);
                elementForCollectionProperty.Add(property, new List<string>());
                sensorNameForCollectionProperty.Add(property, conf.name);
            }
            elementForCollectionProperty[property].Add(currentSubProperty);
        }
        
    }

    private IMonoBehaviourSensor addSensor(string confName, List<string> property, int currentOperationPerProperty, Type type)
    {
        MyDebugger.MyDebug("ACTUALLY ADDING COMPONENT TO " + gameObject.name);
        sensorsAdded++;
        MyDebugger.MyDebug("Added " + sensorsAdded + "sensors to " + gameObject.name);
        Component component = gameObject.AddComponent(SensorsUtility.actualMonoBehaviourSensor[type]);
        //MyDebugger.MyDebug("component " + component);
        component.hideFlags = HideFlags.HideInInspector;
        IMonoBehaviourSensor sensor = component as IMonoBehaviourSensor;
        if (!sensor.ready)
        {
            sensor.propertyType = currentPropertyType;
            sensor.path = property;
            sensor.sensorName = confName;
            sensor.operationType = currentOperationPerProperty;
            sensor.brain = brain;
            if (executeRepeteadly)
            {
                sensor.executeRepeteadly = executeRepeteadly;
                sensor.frequency = frequence;
            }
            else
            {
                sensor.triggerClass = triggerClass;
                sensor.updateMethod = updateMethod;
            }
        }
        /*MyDebugger.MyDebug("adding " + sensor.path);
        if (sensor.indexes.Count > 0)
        {
            MyDebugger.MyDebug(" index " + sensor.indexes[0]);
        }
        */
        return sensor;
    }

    public object[] ReadProperty(List<string> _path)
    {
        object[] toReturn = null;
        if (_path.Count==1)
        {
            MyDebugger.MyDebug("SIMPLE PROPERTY");
            toReturn = new object[1];
            toReturn[0] = ReadSimpleValueProperty(_path[0], typeof(GameObject), gameObject);
        }
        else
        {
            MyDebugger.MyDebug("COMPOSED PROPERTY");
            toReturn = (object[])SensorsUtility.ReadComposedProperty(gameObject, _path, _path, typeof(GameObject), gameObject, ReadSimplePropertyMethod);
        }
        return toReturn;
    }
    public object[] ReadSimplePropertyMethod(string path, Type type, object obj)
    {
        MyDebugger.MyDebug("SWITCHING ON " + path);
        object[] toReturn = null;
        if (currentPropertyType.Equals("VALUE"))
        {
            MyDebugger.MyDebug("VALUE PROPERTY");
            toReturn = new object[1];
            toReturn[0] = ReadSimpleValueProperty(path, type, obj);
        }else if (currentPropertyType.Equals("ARRAY2"))
        {
            MyDebugger.MyDebug("ARRAY2 PROPERTY");
            toReturn =  ReadSimpleArrayProperty(path,currentSubProperty, type, obj,0,0);
        }
        else if (currentPropertyType.Equals("LIST"))
        {
            MyDebugger.MyDebug("LIST PROPERTY");
            toReturn =  ReadSimpleListProperty(path,currentSubProperty, type, obj,0);
        }
        return toReturn;

    }
    private object[] ReadSimpleArrayProperty(string path, string collectionElementProperty, Type type, object obj, int i, int j)
    {
        object[] matrixValue = SensorsUtility.GetArrayProperty(path, collectionElementProperty, type, obj, i, j);//matrixValue[0] is the actual matrix, matrixValue[1] is the Property
        object[] toReturn = new object[4];
        if (matrixValue[0] != null && matrixValue[1] != null)
        {
            MyDebugger.MyDebug("READING THE MATRIX!");
            FieldOrProperty toRead = (FieldOrProperty)matrixValue[1];
            Array matrix = (Array)matrixValue[0];
            toReturn[0] = toRead.GetValue(matrix.GetValue(i, j)).GetType(); //Property type of the element of the collection
            toReturn[1] = matrix.GetLength(0);
            toReturn[2] = matrix.GetLength(1);
            toReturn[3] = matrix.GetValue(i,j).GetType(); //Collection Element Type
            MyDebugger.MyDebug("LENGTH " + toReturn[1] + " " + toReturn[2]);
            return toReturn;
        }
        else if (matrixValue[0] != null)
        {
            toReturn[1] = 0;
            toReturn[2] = 0;
            toReturn[3] = matrixValue[0].GetType().GetGenericArguments()[0];
            foreach (FieldOrProperty member in ReflectionExecutor.GetFieldsAndProperties(toReturn[3]))
            {
                if (member.Name().Equals(currentSubProperty))
                {
                    toReturn[0] = member.Type();
                    return toReturn;
                }
            }
            
        }
        return null;
    }

    private object[] ReadSimpleListProperty(string path, string collectionElementProperty, Type type, object obj, int i)
    {
        object[] toReturn = new object[3];
        object[] listValue = SensorsUtility.GetListProperty(path, collectionElementProperty, type, obj, i);
        if (listValue[0] != null && listValue[1] != null)
        {
            FieldOrProperty toRead = (FieldOrProperty)listValue[1];
            IList list = (IList)listValue[0];
            toReturn[0] = toRead.GetValue(list[i]).GetType(); //Property type of the element of the collection
            toReturn[1] = list.Count;
            toReturn[2] = list[i].GetType(); //Collection Element Type
            return toReturn;
        }
        else if(listValue[0] != null)
        {
            toReturn[1] = 0;
            toReturn[2] = listValue[0].GetType().GetGenericArguments()[0];
            foreach (FieldOrProperty member in ReflectionExecutor.GetFieldsAndProperties(toReturn[2]))
            {
                if (member.Name().Equals(currentSubProperty))
                {
                    toReturn[0] = member.Type();
                    return toReturn;
                }
            }
        }
        return null;
    }

    private Type ReadSimpleValueProperty(string st, Type gOType, object obj)
    {
        MemberInfo[] members = gOType.GetMember(st, SensorsUtility.BindingAttr);
        if (members.Length == 0)
        {
            return null;
        }
        FieldOrProperty property = new FieldOrProperty(members[0]);
        return property.GetValue(obj).GetType();
    }
    // Update is called once per frame
    void Update()
    {
        step++;
        if (!ready)
        {
            return;
        }
        
        List<int> newSizes = new List<int>();
        
        foreach (List<string> property in sizeToTrack.Keys.ToList())
        {
            currentPropertyType = typeForCollectionProperty[property];
            foreach (string currentElement in elementForCollectionProperty[property])
            {
                newSizes = new List<int>();
                currentSubProperty = currentElement;
                MyDebugger.MyDebug("checking " + property + " sub " + currentSubProperty + " " + currentPropertyType);
                object[] result = ReadProperty(property);
                //MyDebugger.MyDebug(result.Length);
                //MyDebugger.MyDebug("Collection type "+result[result.Length - 1]);
                //MyDebugger.MyDebug("Collection length "+result[1]);
                //MyDebugger.MyDebug("Property type " + result[0]);
                //MyDebugger.MyDebug("Old size dimensions: " + sizeToTrack[property].Count);
                for (int i = 0; i < sizeToTrack[property].Count; i++)
                {
                    newSizes.Add((int)result[i + 1]);
                    //MyDebugger.MyDebug("property " + property + " element " + currentSubProperty);
                    //MyDebugger.MyDebug("new size is "+newSizes[i]+" old one is "+ sizeToTrack[property][i]);
                    if (newSizes[i] > sizeToTrack[property][i])
                    {
                        if (currentPropertyType.Equals("LIST"))
                        {
                            enlargedList(property, newSizes[i], (Type)result[0], (Type)result[2]);
                        }
                        else if (currentPropertyType.Equals("ARRAY2"))
                        {

                            enlargedArray2(property, i, newSizes[i], (Type)result[0], (Type)result[3]);
                        }
                    }
                    //MyDebugger.MyDebug("Still alive at " + i);
                }
                //MyDebugger.MyDebug("New size dimensions: " + newSizes.Count);
            }
            sizeToTrack[property] = newSizes;
        }
        if (brain.debug)
        {
            if (step % 20 == 0 && gameObject.name.Equals("Brain1"))
            {
                //Debug.Break();
            }
            
        }
        

    }

    private void enlargedArray2(List<string> property, int increasedDimension, int newSize, Type propertyType, Type collectionElementType)
    {
        
        for (int i = sizeToTrack[property][increasedDimension]; i < newSize; i++)
        {
            for (int j = 0; j < sizeToTrack[property][(increasedDimension + 1) % 2]; j++)
            {
                IMonoBehaviourSensor newSensor = addSensor(sensorNameForCollectionProperty[property], property, 0, propertyType);
                newSensor.indexes.Add(i);
                newSensor.indexes.Add(j);
                newSensor.collectionElementProperty = currentSubProperty;
                newSensor.collectionElementType = collectionElementType.Name;
                newSensor.done();
                SensorsManager.GetInstance().addSensor(brain, newSensor);
            }
        }
    }

    private void enlargedList(List<string> property, int newSize, Type propertyType, Type collectionElementType)
    {

        for (int i = sizeToTrack[property][0]; i < newSize; i++)
        {
            IMonoBehaviourSensor newSensor = addSensor(sensorNameForCollectionProperty[property], property, 0, propertyType);
            newSensor.indexes.Add(i);
            newSensor.collectionElementProperty = currentSubProperty;
            newSensor.collectionElementType = collectionElementType.Name;
            newSensor.done();
            SensorsManager.GetInstance().addSensor(brain, newSensor);
        }
    }
                
}
