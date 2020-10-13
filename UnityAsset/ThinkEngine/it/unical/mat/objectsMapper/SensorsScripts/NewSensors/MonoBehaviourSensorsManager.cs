using UnityEngine;
using System.Collections;
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
    internal List<SensorConfiguration> configurations;
    private string currentPropertyType;
    private List<string> currentSubProperties;
    private int sensorsAdded = 0;
    private Dictionary<MyListString, string> typeForCollectionProperty;
    private Dictionary<MyListString, string> sensorNameForCollectionProperty;
    private Dictionary<MyListString, List<string>> elementForCollectionProperty;
    private Dictionary<MyListString, List<int>> sizeToTrack;
    public bool executeRepeteadly;
    public float frequence;
    public MethodInfo updateMethod;
    internal object triggerClass;

    // Use this for initialization
    public void instantiateSensor(SensorConfiguration sensorConfiguration)
    {
        if (!configurations.Contains(sensorConfiguration))
        {
            if (sensorConfiguration.gameObject.Equals(gameObject))
            {
                generateSensor(sensorConfiguration);
                configurations.Add(sensorConfiguration);
            }
        }
    }

    void Awake()
    {
        ////MyDebugger.MyDebug("Awakening");
        //Debug.unityLogger.logEnabled = true;
        sizeToTrack = new Dictionary<MyListString, List<int>>();
        typeForCollectionProperty = new Dictionary<MyListString, string>();
        elementForCollectionProperty = new Dictionary<MyListString, List<string>>();
        sensorNameForCollectionProperty = new Dictionary<MyListString, string>();
        configurations = new List<SensorConfiguration>();
    }

    void Start()
    {
        //Debug.unityLogger.logEnabled = true;
    }

    public List<IMonoBehaviourSensor> generateSensor(SensorConfiguration sensorConfiguration)
    {
        List<IMonoBehaviourSensor> generatedSensors = new List<IMonoBehaviourSensor>();
        
        foreach(MyListString property in sensorConfiguration.properties.Distinct())
        {
            MyDebugger.MyDebug("ADDING SENSOR FOR " + property + " TO " + gameObject.name);
            object[] result=null;
            List<Type> type=null;
            currentSubProperties = new List<string>();
            int currentOperationPerProperty = 0;
            currentPropertyType = "VALUE";//VALUE, ARRAY2, LIST
            foreach (SimpleGameObjectsTracker tracker in sensorConfiguration.advancedConf)
            {
                if (tracker.propertyName.Equals(property)){
                    currentPropertyType= tracker.propertyType;
                    currentSubProperties = tracker.toSave;
                    result = ReadProperty(property);
                    configureSensor(result, type, sensorConfiguration, property, currentOperationPerProperty, generatedSensors);
                    break;
                }
            }
            if (currentPropertyType.Equals("VALUE"))
            {
                result = ReadProperty(property);
                configureSensor(result, type, sensorConfiguration, property, currentOperationPerProperty, generatedSensors);
                foreach (ListOfStringIntPair pair in sensorConfiguration.operationPerProperty)
                {
                    if (pair.Key.Equals(property))
                    {
                        currentOperationPerProperty = pair.Value;
                        break;
                    }
                }
            }
                
        }
        
        ready = true;
        return generatedSensors;
    }

    private void configureSensor(object[] result, List<Type> types, SensorConfiguration conf, MyListString property, int currentOperationPerProperty,List<IMonoBehaviourSensor> generatedSensors)
    {
        //MyDebugger.MyDebug("configuring sensors for " + property);
        if (result != null)
        {
            if (result[0].GetType() == typeof(Type))
            {
                types = new List<Type>();
                types.Add((Type)result[0]);
            }
            else
            {
                types = (List<Type>)result[0];
            }
        }
        if (types != null && currentPropertyType.Equals("VALUE"))
        {
            IMonoBehaviourSensor sensor = addSensor(conf.name, property, currentOperationPerProperty, types);
            //MyDebugger.MyDebug("AND IT'S VALUE");
            sensor.done();
            generatedSensors.Add(sensor);
        }
        else if (types != null)
        {
            List<int> currentSize = new List<int>();
            if (currentPropertyType.Equals("LIST"))
            {
                //MyDebugger.MyDebug("AND IT'S LIST");
                currentSize.Add((int)result[1]);
                for (int i = 0; i < (int)result[1]; i++)
                {
                    IMonoBehaviourSensor sensor = addSensor(conf.name, property, currentOperationPerProperty, types);
                    sensor.indexes.Add(i);
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
                        IMonoBehaviourSensor sensor = addSensor(conf.name, property, currentOperationPerProperty, types);
                        sensor.indexes.Add(i);
                        sensor.indexes.Add(j);
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
            elementForCollectionProperty[property].AddRange(currentSubProperties);
        }
        
    }

    private IMonoBehaviourSensor addSensor(string confName, MyListString property, int currentOperationPerProperty, List<Type> types)
    {
        MyDebugger.MyDebug("ACTUALLY ADDING COMPONENT TO " + gameObject.name);
        sensorsAdded++;
        MyDebugger.MyDebug("Added " + sensorsAdded + "sensors to " + gameObject.name);
        MonoBehaviourSensor sensor = gameObject.AddComponent<MonoBehaviourSensor>();
        //MyDebugger.MyDebug("component " + component);
        sensor.hideFlags = HideFlags.HideInInspector;
        if (!sensor.ready)
        {
            sensor.propertyType = currentPropertyType;
            sensor.collectionElementProperties = currentSubProperties;
            sensor.path = property;
            sensor.sensorName = confName;
            sensor.operationType = currentOperationPerProperty;
            sensor.brain = brain;
            for (int k = 0; k < types.Count; k++)
            {
                sensor.propertyValues.Add(SensorsUtility.getSpecificList(types[k]));
            }
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

    public object[] ReadProperty(MyListString _path)
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
            toReturn =  ReadSimpleArrayProperty(path,currentSubProperties, type, obj,0,0);
        }
        else if (currentPropertyType.Equals("LIST"))
        {
            MyDebugger.MyDebug("LIST PROPERTY");
            toReturn =  ReadSimpleListProperty(path,currentSubProperties, type, obj,0);
        }
        return toReturn;

    }
    private object[] ReadSimpleArrayProperty(string path, List<string> collectionElementProperties, Type type, object obj, int x, int y)
    {
        object[] matrixValue = SensorsUtility.GetArrayProperty(path, collectionElementProperties, type, obj, x, y);//matrixValue[0] is the actual matrix, matrixValue[1] is a list of Properties
        object[] toReturn = new object[4];
        if (matrixValue[0] != null && matrixValue[1] != null)
        {
            MyDebugger.MyDebug("READING THE MATRIX!");
            List<FieldOrProperty> toRead = (List<FieldOrProperty>)matrixValue[1];
            Array matrix = (Array)matrixValue[0];
            List<Type> typesForProperties = new List<Type>();
            toReturn[0] = typesForProperties;
            for(int i=0; i < toRead.Count; i++)
            {
                typesForProperties.Add(toRead[i].GetValue(matrix.GetValue(x, y)).GetType()); //Property type of the element of the collection
            }
            toReturn[1] = matrix.GetLength(0);
            toReturn[2] = matrix.GetLength(1);
            toReturn[3] = matrix.GetValue(x,y).GetType(); //Collection Element Type
            MyDebugger.MyDebug("LENGTH " + toReturn[1] + " " + toReturn[2]);
            return toReturn;
        }
        else if (matrixValue[0] != null)
        {
            toReturn[1] = 0;
            toReturn[2] = 0;
            toReturn[3] = matrixValue[0].GetType().GetGenericArguments()[0];
            for (int i = 0; i < currentSubProperties.Count; i++)
            {
                foreach (FieldOrProperty member in ReflectionExecutor.GetFieldsAndProperties(toReturn[3]))
                {
                    if (member.Name().Equals(currentSubProperties[i]))
                    {
                        ((List<Type>)toReturn[0]).Add(member.Type());
                        break;
                    }
                }
            }
            return toReturn;
        }
        return null;
    }

    private object[] ReadSimpleListProperty(string path, List<string> collectionElementProperties, Type type, object obj, int x)
    {
        object[] toReturn = new object[3];
        object[] listValue = SensorsUtility.GetListProperty(path, collectionElementProperties, type, obj, x);
        List<Type> typesForProperties = new List<Type>();
        toReturn[0] = typesForProperties;
        if (listValue[0] != null && listValue[1] != null)
        {
            List<FieldOrProperty> toRead = (List<FieldOrProperty>)listValue[1];
            IList list = (IList)listValue[0];
            for (int i = 0; i < toRead.Count; i++)
            {
                typesForProperties.Add(toRead[i].GetValue(list[x]).GetType()); //Property type of the element of the collection
            }
            toReturn[1] = list.Count;
            toReturn[2] = list[x].GetType(); //Collection Element Type
            return toReturn;
        }
        else if(listValue[0] != null)
        {
            toReturn[1] = 0;
            toReturn[2] = listValue[0].GetType().GetGenericArguments()[0];
            for (int i = 0; i < currentSubProperties.Count; i++)
            {
                foreach (FieldOrProperty member in ReflectionExecutor.GetFieldsAndProperties(toReturn[2]))
                {
                    if (member.Name().Equals(currentSubProperties))
                    {
                        ((List<Type>)toReturn[0]).Add(member.Type());
                        break;
                    }
                }
            }
            return toReturn;
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
        
        foreach (MyListString property in sizeToTrack.Keys.ToList())
        {
            currentPropertyType = typeForCollectionProperty[property];
            newSizes = new List<int>();
            currentSubProperties = elementForCollectionProperty[property];
            MyDebugger.MyDebug("checking " + property + " sub " + currentSubProperties + " " + currentPropertyType);
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
                        enlargedList(property, newSizes[i], (List<Type>)result[0], (Type)result[2]);
                    }
                    else if (currentPropertyType.Equals("ARRAY2"))
                    {

                        enlargedArray2(property, i, newSizes[i], (List<Type>)result[0], (Type)result[3]);
                    }
                }
                //MyDebugger.MyDebug("Still alive at " + i);
            }
                //MyDebugger.MyDebug("New size dimensions: " + newSizes.Count);
            
            sizeToTrack[property] = newSizes;
        }
    }

    private void enlargedArray2(MyListString property, int increasedDimension, int newSize, List<Type> propertiesType, Type collectionElementType)
    {
        
        for (int i = sizeToTrack[property][increasedDimension]; i < newSize; i++)
        {
            for (int j = 0; j < sizeToTrack[property][(increasedDimension + 1) % 2]; j++)
            {
                IMonoBehaviourSensor newSensor = addSensor(sensorNameForCollectionProperty[property], property, 0, propertiesType);
                newSensor.indexes.Add(i);
                newSensor.indexes.Add(j);
                newSensor.collectionElementProperties = currentSubProperties;
                newSensor.collectionElementType = collectionElementType.Name;
                newSensor.done();
                SensorsManager.GetInstance().addSensor(brain, newSensor);
            }
        }
    }

    private void enlargedList(MyListString property, int newSize, List<Type> propertiesType, Type collectionElementType)
    {

        for (int i = sizeToTrack[property][0]; i < newSize; i++)
        {
            IMonoBehaviourSensor newSensor = addSensor(sensorNameForCollectionProperty[property], property, 0, propertiesType);
            newSensor.indexes.Add(i);
            newSensor.collectionElementProperties = currentSubProperties;
            newSensor.collectionElementType = collectionElementType.Name;
            newSensor.done();
            SensorsManager.GetInstance().addSensor(brain, newSensor);
        }
    }
                
}
