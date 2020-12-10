using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using static MonoBehaviourSensorHider;

[ExecuteInEditMode]
internal class MonoBehaviourSensorsManager : MonoBehaviour
{
    internal bool ready;
    internal Dictionary<SensorConfiguration, List<MonoBehaviourSensor>> configurations;
    bool destroyed;

    private string currentPropertyType;
    private List<string> currentSubProperties;
    private Dictionary<MyListString, string> typeForCollectionProperty;
    private Dictionary<MyListString, string> sensorNameForCollectionProperty;
    private Dictionary<MyListString, List<string>> elementForCollectionProperty;
    private Dictionary<MyListString, List<int>> sizeToTrack;
    #region Unity Messages
    void Awake()
    {
        Reset();
    }
    void OnEnable()
    {
        Reset();
    }
    void Reset()
    {
        hideFlags = HideFlags.HideInInspector;
        sizeToTrack = new Dictionary<MyListString, List<int>>();
        typeForCollectionProperty = new Dictionary<MyListString, string>();
        elementForCollectionProperty = new Dictionary<MyListString, List<string>>();
        sensorNameForCollectionProperty = new Dictionary<MyListString, string>();
        configurations = new Dictionary<SensorConfiguration, List<MonoBehaviourSensor>>();
        SensorsManager.configurationsChanged = true;
    }
    void Start()
    {
        if (Application.isPlaying)
        {
            foreach (SensorConfiguration configuration in GetComponents<SensorConfiguration>())
            {
                if (configuration.saved)
                {
                    InstantiateSensor(configuration);
                }
            }
            ready = true;
        }
    }
    void Update()
    {
        if (!ready || !Application.isPlaying)
        {
            return;
        }
        List<int> newSizes;
        foreach (MyListString property in sizeToTrack.Keys.ToList())
        {
            currentPropertyType = typeForCollectionProperty[property];
            newSizes = new List<int>();
            currentSubProperties = elementForCollectionProperty[property];
            object[] result = ReadProperty(property);
            for (int i = 0; i < sizeToTrack[property].Count; i++)
            {
                newSizes.Add((int)result[i + 1]);
                if (newSizes[i] > sizeToTrack[property][i])
                {
                    SensorConfiguration currentConfiguration = null;
                    foreach (SensorConfiguration sensorConfiguration in GetComponents<SensorConfiguration>())
                    {
                        if (sensorConfiguration.configurationName.Equals(sensorNameForCollectionProperty[property]))
                        {
                            currentConfiguration = sensorConfiguration;
                            break;
                        }
                    }
                    if (currentPropertyType.Equals("LIST"))
                    {
                        EnlargedList(property, currentConfiguration, newSizes[i], (List<Type>)result[0], (Type)result[2]);
                    }
                    else if (currentPropertyType.Equals("ARRAY2"))
                    {

                        EnlargedArray2(property, currentConfiguration, i, newSizes[i], (List<Type>)result[0], (Type)result[3]);
                    }
                }
            }
            sizeToTrack[property] = newSizes;
        }
    }
    void OnDestroy()
    {
        destroyed = true;
    }
    void OnDisable()
    {
        destroyed = true;
    }
    void OnApplicationQuit()
    {
        destroyed = true;
    }
    #endregion
    internal void AddConfiguration(SensorConfiguration configuration)
    {
        SensorsManager.configurationsChanged = true;
    }
    internal void DeleteConfiguration(SensorConfiguration configuration)
    {
        if (destroyed)
        {
            return;
        }
        SensorsManager.configurationsChanged = true;
        if (GetComponents<SensorConfiguration>().Length==1)
        {
            if (Application.isPlaying)
            {
                Destroy(this);
            }
            else
            {
                DestroyImmediate(this);
            }
        }
    }
    public void InstantiateSensor(SensorConfiguration sensorConfiguration)
    {
        if (!configurations.ContainsKey(sensorConfiguration))
        {
            if (sensorConfiguration.gameObject.Equals(gameObject))
            {
                configurations.Add(sensorConfiguration, GenerateSensors(sensorConfiguration));
            }
        }
    }
    internal List<MonoBehaviourSensor> GenerateSensors(SensorConfiguration sensorConfiguration)
    {
        List<MonoBehaviourSensor> generatedSensors = new List<MonoBehaviourSensor>();
        foreach (MyListString property in sensorConfiguration.properties)
        {
            object[] result = null;
            List<Type> type = null;
            currentSubProperties = new List<string>();
            int currentOperationPerProperty = 0;
            currentPropertyType = "VALUE";//VALUE, ARRAY2, LIST
            foreach (SimpleGameObjectsTracker tracker in sensorConfiguration.advancedConf)//this foreach will configure all and only LIST and ARRAY2 type properties
            {
                if (tracker.propertyName.Equals(property))
                {
                    currentPropertyType = tracker.propertyType;
                    currentSubProperties = tracker.toSave;
                    result = ReadProperty(property);
                    generatedSensors.AddRange(ConfigureSensor(result, type, sensorConfiguration, property, currentOperationPerProperty));
                    break;
                }
            }
            if (currentPropertyType.Equals("VALUE"))
            {
                result = ReadProperty(property);
                generatedSensors.AddRange(ConfigureSensor(result, type, sensorConfiguration, property, currentOperationPerProperty));
                foreach (ListOfMyListStringIntPair pair in sensorConfiguration.operationPerProperty)
                {
                    if (pair.Key.Equals(property))
                    {
                        currentOperationPerProperty = pair.Value;
                        break;
                    }
                }
            }
        }
        return generatedSensors;
    }
    private List<MonoBehaviourSensor> ConfigureSensor(object[] result, List<Type> types, SensorConfiguration conf, MyListString property, int currentOperationPerProperty)
    {
        List<MonoBehaviourSensor> sensors = new List<MonoBehaviourSensor>();
        if (result != null)
        {
            if (result[0] is Type)
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
            sensors.Add(AddSensor(conf.configurationName, property, currentOperationPerProperty, types));
            sensors[sensors.Count - 1].Done();
        }
        else if (types != null)
        {
            List<int> currentSize = new List<int>();
            if (currentPropertyType.Equals("LIST"))
            {
                currentSize.Add((int)result[1]);
                for (int i = 0; i < (int)result[1]; i++)
                {
                    sensors.Add(AddSensor(conf.configurationName, property, currentOperationPerProperty, types));
                    sensors[sensors.Count - 1].indexes.Add(i);
                    sensors[sensors.Count - 1].collectionElementType = ((Type)result[2]).Name;
                    sensors[sensors.Count - 1].Done();
                }
            }
            else if (currentPropertyType.Equals("ARRAY2"))
            {
                currentSize.Add((int)result[1]);
                currentSize.Add((int)result[2]);
                for (int i = 0; i < (int)result[1]; i++)
                {
                    for (int j = 0; j < (int)result[2]; j++)
                    {
                        sensors.Add(AddSensor(conf.configurationName, property, currentOperationPerProperty, types));
                        sensors[sensors.Count - 1].indexes.Add(i);
                        sensors[sensors.Count - 1].indexes.Add(j);
                        sensors[sensors.Count - 1].collectionElementType = ((Type)result[3]).Name;
                        sensors[sensors.Count - 1].Done();
                    }
                }
            }
            if (!sizeToTrack.ContainsKey(property))
            {
                sizeToTrack.Add(property, currentSize);
                typeForCollectionProperty.Add(property, currentPropertyType);
                elementForCollectionProperty.Add(property, new List<string>());
                sensorNameForCollectionProperty.Add(property, conf.configurationName);
            }
            elementForCollectionProperty[property].AddRange(currentSubProperties);
        }
        return sensors;
    }
    internal IEnumerable<string> GetAllConfigurationNames()
    {
        List<string> toReturn = new List<string>();
        foreach (SensorConfiguration configuration in GetComponents<SensorConfiguration>())
        {
            toReturn.Add(configuration.configurationName);
        }
        return toReturn;
    }
    internal SensorConfiguration GetConfiguration(string name)
    {
        foreach (SensorConfiguration configuration in GetComponents<SensorConfiguration>())
        {
            if (configuration.saved && configuration.configurationName.Equals(name))
            {
                return configuration;
            }
        }
        return null;
    }
    internal void RemoveSensor(MonoBehaviourSensor monoBehaviourSensor)
    {
        SensorConfiguration currentConfiguration = null;
        foreach (SensorConfiguration sensorConfiguration in GetComponents<SensorConfiguration>())
        {
            if (sensorConfiguration.configurationName.Equals(monoBehaviourSensor.sensorName))
            {
                currentConfiguration = sensorConfiguration;
                break;
            }
        }
        configurations[currentConfiguration].Remove(monoBehaviourSensor);
    }
    private MonoBehaviourSensor AddSensor(string confName, MyListString property, int currentOperationPerProperty, List<Type> types)
    {
        MonoBehaviourSensor sensor = gameObject.AddComponent<MonoBehaviourSensor>();
        sensor.hideFlags = HideFlags.HideInInspector;
        if (!sensor.ready)
        {
            sensor.propertyType = currentPropertyType;
            sensor.collectionElementProperties = currentSubProperties;
            sensor.property = property;
            sensor.sensorName = confName;
            sensor.operationType = currentOperationPerProperty;
            for (int k = 0; k < types.Count; k++)
            {
                sensor.propertyValues.Add(SensorsUtility.GetSpecificList(types[k]));
            }
        }
        return sensor;
    }
    public object[] ReadProperty(MyListString propertyHierarchy)
    {
        object[] toReturn = null;
        if (propertyHierarchy.Count == 1)
        {
            toReturn = new object[1];
            toReturn[0] = ReadSimpleValueProperty(propertyHierarchy[0], typeof(GameObject), gameObject);
        }
        else
        {
            toReturn = (object[])SensorsUtility.ReadComposedProperty(gameObject, propertyHierarchy, propertyHierarchy, typeof(GameObject), gameObject, ReadSimplePropertyMethod);
        }
        return toReturn;
    }
    public object[] ReadSimplePropertyMethod(string path, Type type, object obj)
    {
        object[] toReturn = null;
        if (currentPropertyType.Equals("VALUE"))
        {
            toReturn = new object[1];
            toReturn[0] = ReadSimpleValueProperty(path, type, obj);
        }
        else if (currentPropertyType.Equals("ARRAY2"))
        {
            toReturn = ReadSimpleArrayProperty(path, currentSubProperties, type, obj, 0, 0);
        }
        else if (currentPropertyType.Equals("LIST"))
        {
            toReturn = ReadSimpleListProperty(path, currentSubProperties, type, obj, 0);
        }
        return toReturn;

    }
    private object[] ReadSimpleArrayProperty(string path, List<string> collectionElementProperties, Type type, object obj, int x, int y)
    {
        object[] matrixValue = SensorsUtility.GetArrayProperty(path, collectionElementProperties, type, obj, x, y);//matrixValue[0] is the actual matrix, matrixValue[1] is a list of Properties
        object[] toReturn = new object[4];
        if (matrixValue[0] != null && matrixValue[1] != null)
        {
            List<FieldOrProperty> toRead = (List<FieldOrProperty>)matrixValue[1];
            Array matrix = (Array)matrixValue[0];
            List<Type> typesForProperties = new List<Type>();
            toReturn[0] = typesForProperties;
            for (int i = 0; i < toRead.Count; i++)
            {
                typesForProperties.Add(toRead[i].GetValue(matrix.GetValue(x, y)).GetType()); //Property type of the element of the collection
            }
            toReturn[1] = matrix.GetLength(0);
            toReturn[2] = matrix.GetLength(1);
            toReturn[3] = matrix.GetValue(x, y).GetType(); //Collection Element Type
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
        else if (listValue[0] != null)
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
    private void EnlargedArray2(MyListString property, SensorConfiguration currentConfiguration, int increasedDimension, int newSize, List<Type> propertiesType, Type collectionElementType)
    {
        for (int i = sizeToTrack[property][increasedDimension]; i < newSize; i++)
        {
            for (int j = 0; j < sizeToTrack[property][(increasedDimension + 1) % 2]; j++)
            {
                MonoBehaviourSensor newSensor = AddSensor(sensorNameForCollectionProperty[property], property, 0, propertiesType);
                newSensor.indexes.Add(i);
                newSensor.indexes.Add(j);
                newSensor.collectionElementProperties = currentSubProperties;
                newSensor.collectionElementType = collectionElementType.Name;
                newSensor.Done();
                configurations[currentConfiguration].Add(newSensor);
            }
        }
    }
    private void EnlargedList(MyListString property, SensorConfiguration currentConfiguration, int newSize, List<Type> propertiesType, Type collectionElementType)
    {

        for (int i = sizeToTrack[property][0]; i < newSize; i++)
        {
            MonoBehaviourSensor newSensor = AddSensor(sensorNameForCollectionProperty[property], property, 0, propertiesType);
            newSensor.indexes.Add(i);
            newSensor.collectionElementProperties = currentSubProperties;
            newSensor.collectionElementType = collectionElementType.Name;
            newSensor.Done();
            configurations[currentConfiguration].Add(newSensor);
        }
    }
}            

