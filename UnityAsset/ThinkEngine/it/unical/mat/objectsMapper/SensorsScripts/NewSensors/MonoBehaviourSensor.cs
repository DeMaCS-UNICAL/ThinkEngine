using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Collections.Generic;
using EmbASP4Unity.it.unical.mat.objectsMapper.Mappers;
using EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class MonoBehaviourSensor<T> : MonoBehaviour, IMonoBehaviourSensor
{
    readonly object toLock = new object();
    bool _ready;
    public Brain brain { get; set; }
    public bool dataAvailable;
    public string _sensorName;    
    public string _path;
    public string _propertyType; //VALUE, ARRAY2, LIST
    public int _operationType;
    public string _collectionElementProperty; //for collections
    public List<int> _indexes; //for collections
    public string _collectionElementType; //for collections
    List<T> _propertyValues;
    public string _ASPRepPrefix;
    public string _ASPRepMid;
    public string _ASPRepSuffix;
    public string sensorName { get { return _sensorName; } set { _sensorName = value; } }
    public string path { get { return _path; } set { _path = value; } }
    public string propertyType { get { return _propertyType; } set { _propertyType = value; } }
    public int operationType { get { return _operationType; } set { _operationType = value; } }
    public string collectionElementProperty { get { return _collectionElementProperty; } set { _collectionElementProperty = value; } }
    public List<int> indexes { get { return _indexes; } set { _indexes = value; } }
    public bool ready { get { return _ready; } set { _ready = value; } }
    public IList propertyValues { get { return _propertyValues; } set { _propertyValues = (List<T>)value; } }
    public string collectionElementType { get { return _collectionElementType; } set { _collectionElementType = value; } }

    public bool executeRepeteadly { get; set; }
    public float frequency { get; set; }
    public object triggerClass { get; set; }
    public MethodInfo updateMethod { get; set; }


    // Use this for initialization
    void Awake()
    {
        //Debug.unityLogger.logEnabled = false;
        
        propertyValues = new List<T>();
        indexes = new List<int>();
    }

    public void done()
    {
        string[] ASPRepresentation = ASPRep();
        _ASPRepPrefix = ASPRepresentation[0];
        _ASPRepMid = ASPRepresentation[1];
        _ASPRepSuffix = ASPRepresentation[2];
        ready = true;
        /*Debug.Log("sensor " + sensorName);
        Debug.Log("execute rep " + executeRepeteadly);
        Debug.Log("trigger class " + triggerClass);
        Debug.Log("update method " + updateMethod);
        Debug.Log("update frequency " + frequency);*/
       
    }

    private string[] ASPRep()
    {
        string[] sensorMapping = new string[3];
        string keyWithoutDotsAndSpaces = path.Replace(".", "");
        keyWithoutDotsAndSpaces = keyWithoutDotsAndSpaces.Replace(" ", "");
        keyWithoutDotsAndSpaces = keyWithoutDotsAndSpaces.Replace("_", "");
        string sensorNameNotCapital = char.ToLower(sensorName[0]) + sensorName.Substring(1);
        ////Debug.Log("goname " + s.gOName);
        string goNameNotCapital = "";
        goNameNotCapital = char.ToLower(gameObject.name[0]) + gameObject.name.Substring(1);
        sensorMapping[0] = sensorNameNotCapital + "(";
        sensorMapping[0] += goNameNotCapital + "(";
        List<string> temp = ASPMapperHelper.getInstance().buildTemplateMapping(keyWithoutDotsAndSpaces, '^');
        sensorMapping[0] += temp[0];
        sensorMapping[2] = temp[temp.Count - 1] + ")).";
        if (!propertyType.Equals("VALUE"))
        {
            List<string> inner = ASPMapperHelper.getInstance().buildTemplateMapping(collectionElementProperty, '^');
            inner[0] = Char.ToLower(collectionElementType[0]) + collectionElementType.Substring(1) + "(" + inner[0];
            inner[inner.Count - 1] += ")";
            sensorMapping[1] = inner[0];
            sensorMapping[2] = inner[inner.Count - 1] + sensorMapping[2];
        }
        return sensorMapping;
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
            if(gameObject.GetComponent<MonoBehaviourSensorsManager>().elapsedMS < frequency)
            {
                return;
            }
        }else if (!(bool)updateMethod.Invoke(triggerClass, null))
        {
            return;
        }
        //Debug.Log("updating " + sensorName);
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
            if (!_path.Contains("^"))
                {

                    ReadSimplePropertyMethod(_path, typeof(GameObject), gameObject);
                }
                else
                {
                    SensorsUtility.ReadComposedProperty(gameObject, _path, _path, typeof(GameObject), gameObject, ReadSimplePropertyMethod);
                }
            
            dataAvailable = true;
        }
    }

    private object ReadSimplePropertyMethod(string path, Type type, object obj)
    {
        //Debug.Log("SWITCHING");

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
        //Debug.Log("READING");

        object[] matrixValue = SensorsUtility.GetArrayProperty(path, collectionElementProperty, type, obj, indexes[0], indexes[1]);
        //Debug.Log("Matrix: " + matrixValue[0]);
        //Debug.Log("Property: " + matrixValue[1]);
        if (matrixValue[0] != null && matrixValue[1] != null)
        {
            FieldOrProperty toRead = (FieldOrProperty)matrixValue[1];
            Array matrix = (Array)matrixValue[0];
            
            //Debug.Log("Casting " + toRead.GetValue(matrix.GetValue(indexes[0], indexes[1])).GetType() + " to " + typeof(T));
            propertyValues.Add((T)Convert.ChangeType(toRead.GetValue(matrix.GetValue(indexes[0], indexes[1])),typeof(T)));
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
        object[] listValue = SensorsUtility.GetListProperty(path, collectionElementProperty, type, obj, indexes[0]);
        if (listValue[0] != null && listValue[1] != null)
        {
            FieldOrProperty toRead = (FieldOrProperty)listValue[1];
            IList list = (IList)listValue[0];
            
            propertyValues.Add((T)Convert.ChangeType(toRead.GetValue(list[indexes[0]]), typeof(T)));
        }

        else
        {
           // Debug.Log("Destroing " + path);
            SensorsManager.GetInstance().removeSensor(brain, this);
            Destroy(this);
            return;
        }
    }
    
    public  void ReadSimpleValueProperty(string st, Type gOType, object obj)
    {
        MemberInfo[] members = gOType.GetMember(st, SensorsUtility.BindingAttr);
        if (members.Length == 0)
        {
            return;
        }
        FieldOrProperty property = new FieldOrProperty(members[0]);
        //Debug.Log("Casting " + property.GetValue(obj).GetType() + " to " + typeof(T));

        propertyValues.Add((T)Convert.ChangeType(property.GetValue(obj), typeof(T)));        
    }

    public string Map()
    {

        if (_propertyValues.Count == 0)
        {
            return "";
        }
        
        IMapper mapperForT = MappingManager.getInstance().getMapper(typeof(T));
        if (propertyType.Equals("VALUE"))
        {
            string temp = mapperForT.basicMap(Operation.compute(operationType, _propertyValues));
            return _ASPRepPrefix + temp +_ASPRepSuffix + Environment.NewLine;
        }
        if (propertyType.Equals("LIST"))
        {
            string temp = mapperForT.basicMap(_propertyValues[_propertyValues.Count-1]);
            return _ASPRepPrefix +"("+ indexes[0] + "," + _ASPRepMid + temp +")"+ _ASPRepSuffix + Environment.NewLine;
        }
        if (propertyType.Equals("ARRAY2"))
        {
            string temp = mapperForT.basicMap(_propertyValues[_propertyValues.Count - 1]);
            return _ASPRepPrefix +"("+ indexes[0] + "," + indexes[1] + "," + _ASPRepMid+ temp + ")"+_ASPRepSuffix + Environment.NewLine;
        }
        return "";
    }
    

}
