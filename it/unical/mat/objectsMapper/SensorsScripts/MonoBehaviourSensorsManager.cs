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
        //hideFlags = HideFlags.HideInInspector;
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
            MyPropertyInfo result = ReadProperty(property);
            for (int i = 0; i < sizeToTrack[property].Count; i++)
            {
                newSizes.Add(result.Size(i));
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
                        EnlargedList(property, currentConfiguration, newSizes[i], result);
                    }
                    else if (currentPropertyType.Equals("ARRAY2"))
                    {

                        EnlargedArray2(property, currentConfiguration, i, newSizes[i], result);
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
            MyPropertyInfo result = null;
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
                    if (result == null)
                    {
                        break;
                    }
                    generatedSensors.AddRange(ConfigureSensor(result, sensorConfiguration, property, currentOperationPerProperty));
                    break;
                }
            }
            if (currentPropertyType.Equals("VALUE"))
            {
                result = ReadProperty(property);
                if (result == null)
                {
                    return generatedSensors;
                }
                generatedSensors.AddRange(ConfigureSensor(result, sensorConfiguration, property, currentOperationPerProperty));
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
    private List<MonoBehaviourSensor> ConfigureSensor(MyPropertyInfo result, SensorConfiguration conf, MyListString property, int currentOperationPerProperty)
    {
        List<MonoBehaviourSensor> sensors = new List<MonoBehaviourSensor>();
        if (result.elementType != null && currentPropertyType.Equals("VALUE"))
        {
            sensors.Add(AddSensor(conf.configurationName, property, currentOperationPerProperty, result.elementType));
            sensors[sensors.Count - 1].Done();
        }
        else if (result.propertiesType != null)
        {
            List<int> currentSize = new List<int>();
            if (currentPropertyType.Equals("LIST"))
            {
                currentSize.Add(result.Size(0));
                for (int i = 0; i < result.Size(0); i++)
                {
                    if (!result.isBasic)
                    {
                        sensors.Add(AddSensor(conf.configurationName, property, currentOperationPerProperty, result.propertiesType));
                    }
                    else
                    {
                        sensors.Add(AddSensor(conf.configurationName, property, currentOperationPerProperty, result.elementType));
                    }
                    sensors[sensors.Count - 1].indexes.Add(i);
                    sensors[sensors.Count - 1].collectionElementType = result.elementType.Name;
                    sensors[sensors.Count - 1].Done();
                }
            }
            else if (currentPropertyType.Equals("ARRAY2"))
            {
                currentSize.Add(result.Size(0));
                currentSize.Add(result.Size(1));
                for (int i = 0; i < result.Size(0); i++)
                {
                    for (int j = 0; j < result.Size(1); j++)
                    {
                        if (!result.isBasic)
                        {
                            sensors.Add(AddSensor(conf.configurationName, property, currentOperationPerProperty, result.propertiesType));
                        }
                        else
                        {
                            sensors.Add(AddSensor(conf.configurationName, property, currentOperationPerProperty, result.elementType));
                        }
                        sensors[sensors.Count - 1].indexes.Add(i);
                        sensors[sensors.Count - 1].indexes.Add(j);
                        if (result.elementType != null)
                        {
                            sensors[sensors.Count - 1].collectionElementType = result.elementType.Name;
                        }
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
    private MonoBehaviourSensor AddSensor(string confName, MyListString property, int currentOperationPerProperty, object types)
    {
        MonoBehaviourSensor sensor = gameObject.AddComponent<MonoBehaviourSensor>();
        sensor.hideFlags = HideFlags.HideInInspector;
        List<Type> typesCopy = new List<Type>();
        if(types is Type)
        {
            typesCopy.Add((Type)types);
        }
        else
        {
            typesCopy.AddRange((List<Type>)types);
        }
        if (!sensor.ready)
        {
            sensor.propertyType = currentPropertyType;
            sensor.collectionElementProperties = currentSubProperties;
            sensor.property = property;
            sensor.sensorName = confName;
            sensor.operationType = currentOperationPerProperty;
            for (int k = 0; k < typesCopy.Count; k++)
            {
                sensor.propertyValues.Add(SensorsUtility.GetSpecificList(typesCopy[k]));
            }
        }
        return sensor;
    }
    public MyPropertyInfo ReadProperty(MyListString propertyHierarchy)
    {
        MyPropertyInfo toReturn = null;
        if (propertyHierarchy.Count == 1)
        {
            toReturn = ReadSimpleValueProperty(propertyHierarchy[0], typeof(GameObject), gameObject);
        }
        else
        {
            toReturn = SensorsUtility.ReadComposedProperty(gameObject, propertyHierarchy, propertyHierarchy, typeof(GameObject), gameObject, ReadSimplePropertyMethod);
        }
        return toReturn;
    }
    public MyPropertyInfo ReadSimplePropertyMethod(string path, Type type, object obj)
    {
        MyPropertyInfo toReturn = null;
        if (currentPropertyType.Equals("VALUE"))
        {
            toReturn = ReadSimpleValueProperty(path, type, obj);
        }
        else if (currentPropertyType.Equals("ARRAY2"))
        {
            if (currentSubProperties.Count == 1 && currentSubProperties[0].Equals(""))
            {
                toReturn = ReadBasicArrayProperty(path, type, obj, 0, 0);
            }
            else
            {
                toReturn = ReadArrayProperty(path, currentSubProperties, type, obj, 0, 0);
            }
        }
        else if (currentPropertyType.Equals("LIST"))
        {
            if (currentSubProperties.Count == 1 && currentSubProperties[0].Equals(""))
            {
                toReturn = ReadBasicListProperty(path, type, obj, 0);
            }
            else
            {
                toReturn = ReadListProperty(path, currentSubProperties, type, obj, 0);
            }
        }
        return toReturn;

    }

    private ArrayInfo ReadBasicArrayProperty(string path, Type type, object obj, int x, int y)
    {
        ArrayInfo matrixValue = SensorsUtility.GetArrayProperty(path, type, obj, x, y);
        if (matrixValue.array != null && matrixValue.isBasic)
        {
            matrixValue.size[0] = matrixValue.array.GetLength(0);
            matrixValue.size[1] = matrixValue.array.GetLength(1);
            matrixValue.elementType = matrixValue.array.GetType().GetElementType();
        }
        return matrixValue;
    }

    private ArrayInfo ReadArrayProperty(string path, List<string> collectionElementProperties, Type type, object obj, int x, int y)
    {
        ArrayInfo matrixValue = SensorsUtility.GetArrayProperty(path, type, obj, x, y, collectionElementProperties);//matrixValue[0] is the actual matrix, matrixValue[1] is a list of Properties
        if (matrixValue.array != null && !matrixValue.isBasic)
        {
            if (matrixValue.properties != null)
            {
                List<FieldOrProperty> toRead = matrixValue.properties;
                Array matrix = matrixValue.array;
                List<Type> typesForProperties = new List<Type>();
                for (int i = 0; i < toRead.Count; i++)
                {
                    typesForProperties.Add(toRead[i].GetValue(matrix.GetValue(x, y)).GetType()); //Property type of the element of the collection
                }
                matrixValue.propertiesType = typesForProperties;
                matrixValue.size[0] = matrix.GetLength(0);
                matrixValue.size[1] = matrix.GetLength(1);
                matrixValue.elementType = matrix.GetValue(x, y).GetType(); //Collection Element Type
                return matrixValue;
            }
            else
            {
                matrixValue.size[0] = 0;
                matrixValue.size[1] = 0;
                matrixValue.elementType = matrixValue.array.GetType().GetElementType();
                for (int i = 0; i < currentSubProperties.Count; i++)
                {
                    foreach (FieldOrProperty member in ReflectionExecutor.GetFieldsAndProperties(matrixValue.elementType))
                    {
                        if (member.Name().Equals(currentSubProperties[i]))
                        {
                            matrixValue.propertiesType.Add(member.Type());
                            break;
                        }
                    }
                }
                return matrixValue;
            }
        }
        return null;
    }
    private ListInfo ReadBasicListProperty(string path, Type type, object obj, int x)
    {
        ListInfo listValue = SensorsUtility.GetListProperty(path, type, obj, x);
        if (listValue.list != null && listValue.isBasic)
        {
            listValue.elementType = listValue.list.GetType().GetGenericArguments()[0];
            listValue.count = listValue.list.Count;
        }
        return listValue;
    }

    private ListInfo ReadListProperty(string path, List<string> collectionElementProperties, Type type, object obj, int x)
    {
        ListInfo listValue = SensorsUtility.GetListProperty(path, type, obj, x, collectionElementProperties);
        List<Type> typesForProperties = new List<Type>();
        listValue.propertiesType = typesForProperties;
        if (listValue.list != null && !listValue.isBasic)
        {
            if (listValue.properties != null)
            {
                List<FieldOrProperty> toRead = listValue.properties;
                IList list = listValue.list;
                for (int i = 0; i < toRead.Count; i++)
                {
                    typesForProperties.Add(toRead[i].GetValue(list[x]).GetType()); //Property type of the element of the collection
                }
                listValue.count = list.Count;
                listValue.elementType = list[x].GetType();
                return listValue;
            }
            else
            {
                listValue.count = 0;
                listValue.elementType = listValue.list.GetType().GetGenericArguments()[0];
                for (int i = 0; i < currentSubProperties.Count; i++)
                {
                    foreach (FieldOrProperty member in ReflectionExecutor.GetFieldsAndProperties(listValue.elementType))
                    {
                        if (member.Name().Equals(currentSubProperties))
                        {
                            listValue.propertiesType.Add(member.Type());
                            break;
                        }
                    }
                }
                return listValue;
            }
        }
        return null;
    }
    private MyPropertyInfo ReadSimpleValueProperty(string st, Type gOType, object obj)
    {
        MemberInfo[] members = gOType.GetMember(st, SensorsUtility.BindingAttr);
        if (members.Length == 0)
        {
            return null;
        }
        FieldOrProperty property = new FieldOrProperty(members[0]);
        MyPropertyInfo toReturn = new MyPropertyInfo();
        toReturn.elementType = property.GetValue(obj).GetType();
        return toReturn;
    }
    private void EnlargedArray2(MyListString property, SensorConfiguration currentConfiguration, int increasedDimension, int newSize, MyPropertyInfo propertyInfo)
    {
        for (int i = sizeToTrack[property][increasedDimension]; i < newSize; i++)
        {
            for (int j = 0; j < sizeToTrack[property][(increasedDimension + 1) % 2]; j++)
            {
                MonoBehaviourSensor newSensor;
                if (!propertyInfo.isBasic)
                {
                    newSensor = AddSensor(sensorNameForCollectionProperty[property], property, 0, propertyInfo.propertiesType);
                }
                else
                {
                    newSensor = AddSensor(sensorNameForCollectionProperty[property], property, 0, propertyInfo.elementType);
                }
                newSensor.indexes.Add(i);
                newSensor.indexes.Add(j);
                newSensor.collectionElementProperties = currentSubProperties;
                newSensor.collectionElementType = propertyInfo.elementType.Name;
                newSensor.Done();
                configurations[currentConfiguration].Add(newSensor);
            }
        }
    }
    private void EnlargedList(MyListString property, SensorConfiguration currentConfiguration, int newSize, MyPropertyInfo propertyInfo)
    {

        for (int i = sizeToTrack[property][0]; i < newSize; i++)
        {
            MonoBehaviourSensor newSensor;
            if (!propertyInfo.isBasic)
            {
                newSensor = AddSensor(sensorNameForCollectionProperty[property], property, 0, propertyInfo.propertiesType);
            }
            else
            {
                newSensor = AddSensor(sensorNameForCollectionProperty[property], property, 0, propertyInfo.elementType);
            }
            newSensor.indexes.Add(i);
            newSensor.collectionElementProperties = currentSubProperties;
            newSensor.collectionElementType = propertyInfo.elementType.Name;
            newSensor.Done();
            configurations[currentConfiguration].Add(newSensor);
        }
    }
}            

