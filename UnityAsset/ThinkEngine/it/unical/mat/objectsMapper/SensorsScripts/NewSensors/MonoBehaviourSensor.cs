using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Collections.Generic;
using EmbASP4Unity.it.unical.mat.objectsMapper.Mappers;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Linq;

public class MonoBehaviourSensor : MonoBehaviour, IMonoBehaviourSensor
{
    readonly object toLock = new object();
    bool _ready;
    public Brain brain { get; set; }
    public bool dataAvailable;
    public string sensorName { get; set; }
    public MyListString path { get; set; }
    public string propertyType { get; set; }
    public int operationType { get; set; }
    public List<string> collectionElementProperties { get; set; }
    public List<int> indexes { get; set; }
    public bool ready { get; set; }
    public List<IList> propertyValues { get; set; }
    public string collectionElementType { get; set; }

    public bool executeRepeteadly { get; set; }
    public float frequency { get; set; }
    public object triggerClass { get; set; }
    public MethodInfo updateMethod { get; set; }


    // Use this for initialization
    void Awake()
    {
        //Debug.unityLogger.logEnabled = false;
        
        propertyValues = new List<IList>();
        indexes = new List<int>();
    }
    public void addListToPropertyValues(IList list)
    {
        propertyValues.Add(list);
    }
    public void done()
    {
        ready = true;
        /*MyDebugger.MyDebug("sensor " + sensorName);
        MyDebugger.MyDebug("execute rep " + executeRepeteadly);
        MyDebugger.MyDebug("trigger class " + triggerClass);
        MyDebugger.MyDebug("update method " + updateMethod);
        MyDebugger.MyDebug("update frequency " + frequency);*/
    }

    

    public bool configured()
    {
        return ready;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Debug.Break();
        if (!ready)
        {
            return;
        }
        if (executeRepeteadly)
        {
            if(gameObject.GetComponent<MonoBehaviourSensorsManager>().brain.elapsedMS < frequency)
            {
                return;
            }
        }
        else if (!(bool)updateMethod.Invoke(triggerClass, null))
        {
            return;
        }
        //MyDebugger.MyDebug("updating " + sensorName);
        ReadProperty();
        if (propertyValues.Count == 100)
        {
            propertyValues.RemoveAt(0);
        }
        SensorsManager.AddUpdatedSensor(brain);
        
    }

    public void ReadProperty()
    {
        lock (toLock)
        {
            if (path.Count==-1)
                {

                    ReadSimplePropertyMethod(path[0], typeof(GameObject), gameObject);
                }
                else
                {
                    SensorsUtility.ReadComposedProperty(gameObject, path, path, typeof(GameObject), gameObject, ReadSimplePropertyMethod);
                }
            
            dataAvailable = true;
        }
    }

    private object ReadSimplePropertyMethod(string path, Type type, object obj)
    {
        //MyDebugger.MyDebug("SWITCHING");

        if (propertyType.Equals("VALUE"))
        {
            ReadSimpleValueProperty(path, type, obj);
            return null;
        }
        if (propertyType.Equals("LIST"))
        {
            ReadSimpleListProperty(path, type, obj);
            return null;
        }
        if (propertyType.Equals("ARRAY2"))
        {
            ReadSimpleArrayProperty(path, type, obj);
            return null;
        }
        return null;
    }

    private void ReadSimpleArrayProperty(string path, Type type, object obj)
    {
        //MyDebugger.MyDebug("READING");

        object[] matrixValue = SensorsUtility.GetArrayProperty(path, collectionElementProperties, type, obj, indexes[0], indexes[1]);
        //MyDebugger.MyDebug("Matrix: " + matrixValue[0]);
        //MyDebugger.MyDebug("Property: " + matrixValue[1]);
        if (matrixValue[0] != null && matrixValue[1] != null)
        {
            List<FieldOrProperty> toRead = (List<FieldOrProperty>)matrixValue[1];
            Array matrix = (Array)matrixValue[0];
            //MyDebugger.MyDebug("Casting " + toRead.GetValue(matrix.GetValue(indexes[0], indexes[1])).GetType() + " to " + typeof(T));
            for (int i = 0; i < collectionElementProperties.Count; i++)
            {
                IList currentValuesList = propertyValues[i];
                Type currentValuesListType = currentValuesList.GetType().GetGenericArguments()[0];
                currentValuesList.Add(Convert.ChangeType(toRead[i].GetValue(matrix.GetValue(indexes[0], indexes[1])), currentValuesListType));
            }
        }
        else
        {
            SensorsManager.GetInstance().removeSensor(brain, this);
            Destroy(this);
            return;
        }

    }

    private void ReadSimpleListProperty(string path, Type type, object obj)
    {
        object[] listValue = SensorsUtility.GetListProperty(path, collectionElementProperties, type, obj, indexes[0]);
        if (listValue[0] != null && listValue[1] != null)
        {
            List<FieldOrProperty> toRead = (List<FieldOrProperty>)listValue[1];
            IList list = (IList)listValue[0];
            for (int i = 0; i < collectionElementProperties.Count; i++)
            {
                IList currentValuesList = propertyValues[i];
                Type currentValuesListType = currentValuesList.GetType().GetGenericArguments()[0];
                currentValuesList.Add(Convert.ChangeType(toRead[i].GetValue(list[indexes[0]]), currentValuesListType));
            }
        }

        else
        {
           // MyDebugger.MyDebug("Destroing " + path);
            SensorsManager.GetInstance().removeSensor(brain, this);
            Destroy(this);
            return;
        }
    }
    
    public  void ReadSimpleValueProperty(string path, Type gOType, object obj)
    {
        MemberInfo[] members = gOType.GetMember(path, SensorsUtility.BindingAttr);
        if (members.Length == 0)
        {
            return;
        }
        FieldOrProperty property = new FieldOrProperty(members[0]);
        //MyDebugger.MyDebug("Casting " + property.GetValue(obj).GetType() + " to " + typeof(T));
        IList currentValuesList = propertyValues[0];
        Type currentValuesListType = currentValuesList.GetType().GetGenericArguments()[0];
        currentValuesList.Add(Convert.ChangeType(property.GetValue(obj), currentValuesListType));
    }

    public string Map()
    {
        List<string> myTemplate = getMyTemplate();
        string toReturn="";
        IMapper mapperForT;
        IList currentValuesList;
        Type currentValuesListType;
        string value;
        if (propertyType.Equals("VALUE"))
        {
            currentValuesList = propertyValues[0];
            currentValuesListType = currentValuesList.GetType().GetGenericArguments()[0];
            mapperForT  = MappingManager.getInstance().getMapper(currentValuesListType);
            value = mapperForT.basicMap(Operation.compute(operationType, propertyValues));
            return string.Format(myTemplate[0], value);
        }
        if (propertyType.Equals("LIST") || propertyType.Equals("ARRAY2"))
        {
            object[] parameters;
            if (propertyType.Equals("LIST"))
            {
                parameters = new object[2];
                parameters[0] = indexes[0];
            }
            else
            {
                parameters = new object[3];
                parameters[0] = indexes[0];
                parameters[1] = indexes[1];
            }
            for (int i = 0; i < collectionElementProperties.Count; i++)
            {
                currentValuesList = propertyValues[i];
                currentValuesListType = currentValuesList.GetType().GetGenericArguments()[0];
                mapperForT = MappingManager.getInstance().getMapper(currentValuesListType);
                value = mapperForT.basicMap(propertyValues[i][propertyValues.Count - 1]);
                parameters[parameters.Length - 1] = value;
                toReturn += string.Format(myTemplate[i], parameters) + Environment.NewLine;
            }

        }
        return toReturn;
    }

    private List<string> getMyTemplate()
    {
        SensorConfiguration myConfiguration = getCorrespondingConfiguration();
        int myPropertyIndex=-1;
        foreach(MyListString property in myConfiguration.properties)
        {
            if (property.SequenceEqual(path))
            {
                myPropertyIndex = myConfiguration.properties.IndexOf(property);
                break;
            }
        }
        if (myPropertyIndex ==-1)
        {
            return null;
        }
        return myConfiguration.aspTemplate[myPropertyIndex];
    }

    private SensorConfiguration getCorrespondingConfiguration()
    {
        foreach(SensorConfiguration sensorConfiguration in gameObject.GetComponents<SensorConfiguration>())
        {
            if (sensorConfiguration.configurationName.Equals(sensorName))
            {
                return sensorConfiguration;
            }
        }
        return null;
    }
}
