using UnityEngine;
using System.Reflection;
using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.IO;


public class ReflectionExecutor : ScriptableObject
{
    public const BindingFlags BindingAttr = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

    public List<string> GetGameObjects()
    {
        object[] go = GameObject.FindObjectsOfType(typeof(GameObject));
        List<string> objectsNames = new List<string>();
        foreach(object o in go)
        {
          
            objectsNames.Add(((GameObject)o).name);
            
        }
        return objectsNames;
    }

    void Update()
    {
        //Debug.Log("Editor causes this Update");
        string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(mydocpath, "WriteLines.txt")))
        {
            //test(outputFile);
        }
        
    }
   /* public void test(StreamWriter o)
    {
        object[] go = GameObject.FindObjectsOfType(typeof(GameObject));

        //Debug.Log(go.Length);
        List<object> metObjects = new List<object>();
        foreach(object obj in go)
        {
            if (((GameObject)obj).name.Equals("test"))
            {
                 Debug.Log(((MonoBehaviour)go[0]).name + " has " + ((MonoBehaviour)go[0]).GetType().GetProperties().Length + " properties");
                 Debug.Log(((MonoBehaviour)go[0]).name + " has type: " + go[0].GetType());

                 // printBaseType(go[0].GetType(), 0, metTypes);
                 string description = describeObject(((MonoBehaviour)obj), 0, metTypes);

                 Debug.Log(description);

                //MonoBehaviour ob = ((GameObject)obj).GetComponent<TestingReflection>();
                o.WriteLine("DESCRIBING OBJECT " + ((GameObject)obj).name+" that is a "+obj.GetType());
                describeObject(obj, 0, metObjects, o);

               
                foreach (Component c in ((GameObject)obj).GetComponents(typeof(Component)))
                {
                    o.WriteLine("DESCRIBING COMPONENT of type" + c.GetType());
                    // Debug.Log(c.name+" "+c.GetType());
                    describeObject(c, 0, metObjects, o);
                }

            }
        }
       
        
    }*/

    public bool IsBaseType(FieldOrProperty obj)
    {
        List<Type> signedInteger = SignedIntegerTypes();
        List<Type> unsignedInteger = UnsignedIntegerTypes();
        List<Type> floatingPoint = FloatingPointTypes();
        //Debug.Log(" level " + level);
        Type objType = obj.Type();
        uint u = 1;
        byte b = 1;
        ulong l = 1;
        ushort s = 1;
        l = s;
        //Debug.Log(obj.GetProperties()[0].PropertyType+" with name "+ obj.GetProperties()[0].Name);
        bool isBase = signedInteger.Contains(objType) || unsignedInteger.Contains(objType) || floatingPoint.Contains(objType);
        isBase |= objType == typeof(char) || objType == typeof(bool) || objType == typeof(Enum) || objType == typeof(string);
        return  isBase ;
    }

    public int isArrayOfRank(FieldOrProperty obj)
    {
        Type objType = obj.Type();
        return objType.IsArray? objType.GetArrayRank():-1;
    }

    public static List<Type> FloatingPointTypes()
    {
        return new List<Type> { typeof(double), typeof(float) };
    }

    public static List<Type> UnsignedIntegerTypes()
    {
        return new List<Type> { typeof(byte), typeof(ushort), typeof(uint), typeof(ulong) };
    }

    public static List<Type> SignedIntegerTypes()
    {
        return new List<Type> { typeof(sbyte), typeof(short), typeof(int), typeof(long) };
    }

    /*public void printBaseType(Type obj, int level, List<object> met)
    {
        List<Type> signedInteger = new List<Type> { typeof(sbyte), typeof(short), typeof(int), typeof(long) };
        List<Type> unsignedInteger = new List<Type> { typeof(byte), typeof(ushort), typeof(uint), typeof(ulong) };
        List<Type> floatingPoint = new List<Type> { typeof(double), typeof(float) };
        Debug.Log(" level " + level);
        //Debug.Log(obj.GetProperties()[0].PropertyType+" with name "+ obj.GetProperties()[0].Name);
        met.Add(obj);
        if (level > 5)
        {
            Debug.Log("too many levels");
        }
        else if (signedInteger.Contains(obj))
        {
            Debug.Log(obj + " is a signed integer");
        }
        else if (unsignedInteger.Contains(obj))
        {
            Debug.Log(obj + " is an unsigned integer");
        }
        else if (floatingPoint.Contains(obj))
        {
            Debug.Log(obj + " is a floating point number");
        }
        else if (obj == typeof(char))
        {
            Debug.Log(obj + " is a char");
        }
        else if (obj == typeof(bool))
        {
            Debug.Log(obj + " is a bool");
        }
        else if (obj == typeof(Enum))
        {
            Debug.Log(obj + " is an Enum");
        }
        else
        {
            Debug.Log("Going deeper");
            ++level;
            foreach (PropertyInfo child in obj.GetProperties())
            {
                // Debug.Log("child type " + child.PropertyType + " with name "+child.Name);
                if (!met.Contains(child.PropertyType))
                {
                    printBaseType(child.PropertyType, level, met);
                }
                else
                {
                    Debug.Log("Skipping Type " + child.PropertyType);
                }
            }
             foreach (FieldInfo child in obj.GetFields())
             {
                 Debug.Log("child type " + child.GetType());
                 if (!met.Contains(child.GetType()))
                 {
                     printBaseType(child.GetType(), level, met);
                 }
                 else
                 {
                     Debug.Log("Skipping Type " + child.GetType());
                 }
             }
        }
    }*/

    internal bool isMappable(FieldOrProperty obj)
    {
        return IsBaseType(obj) || isMatrix(obj);
    }

    private bool isMatrix(FieldOrProperty obj)
    {
       
        return isArrayOfRank(obj)==2;
    }

    /*public void describeObject(object obj, int level, List<object> met, StreamWriter o)
    {
        Type objType = obj.GetType();
        //Debug.Log( objType + "has following fields: \n");
        o.WriteLine(objType + " has following fields:");
        met.Add(obj);
        // Debug.Log(objType.GetProperties().Length);
         foreach (PropertyInfo child in objType.GetProperties())
         {

             describing += child.PropertyType + " named " + child.Name + "\n";
         }
         
        // Debug.Log(objType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Length);
        MemberInfo[] fields = objType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        MemberInfo[] properties = objType.GetProperties();
        var members = fields.Union(properties);
        foreach (MemberInfo child in members)
        {
            if (child.MemberType == MemberTypes.Field)
            {

                if (met.Contains(((FieldInfo)child).GetValue(obj)))
                {
                    continue;
                }

            }
            else if (met.Contains(((PropertyInfo)child).GetValue(obj,null)))
            {
                continue;
            }

            describeField(new FieldOrProperty(child), obj, level + 1, met, o);
        }
        

    }*/

   /* public void describeField(FieldOrProperty m, object ob, int level, List<object> met, StreamWriter o)
    {
        
        List<Type> signedInteger = new List<Type> { typeof(sbyte), typeof(short), typeof(int), typeof(long) };
        List<Type> unsignedInteger = new List<Type> { typeof(byte), typeof(ushort), typeof(uint), typeof(ulong) };
        List<Type> floatingPoint = new List<Type> { typeof(double), typeof(float) };
        //Debug.Log(" level " + level);
        Type objType = m.Type();
        
        //Debug.Log(obj.GetProperties()[0].PropertyType+" with name "+ obj.GetProperties()[0].Name);
        met.Add(objType);
        if (level > 5)
        {
            // Debug.Log("too many levels");
            o.WriteLine("too many levels");

        }
        else if (signedInteger.Contains(objType))
        {
            //Debug.Log(m.Name + " is a signed integer");
            o.WriteLine(m.Name() + " is a signed integer with value "+m.GetValue(ob));
        }
        else if (unsignedInteger.Contains(objType))
        {
            // Debug.Log(m.Name + " is an unsigned integer");
            o.WriteLine(m.Name() + " is a unsigned integer with value " + m.GetValue(ob));
        }
        else if (floatingPoint.Contains(objType))
        {
            //Debug.Log(m.Name + " is a floating point number");
            o.WriteLine(m.Name() + " is a floating point number with value " + m.GetValue(ob));
        }
        else if (objType == typeof(char))
        {
            //Debug.Log(m.Name + " is a char");
            o.WriteLine(m.Name() + " is a char with value " + m.GetValue(ob));
        }
        else if (objType == typeof(bool))
        {
            //Debug.Log(m.Name + " is a bool");
            o.WriteLine(m.Name() + " is a bool with value " + m.GetValue(ob));
        }
        else if (objType == typeof(Enum))
        {
            //Debug.Log(m.Name + " is an Enum");
            o.WriteLine(m.Name() + " is an Enum with value " + m.GetValue(ob));
        }else if(objType == typeof(string))
        {
            o.WriteLine(m.Name() + " is a string with value" + m.GetValue(ob));
        }
        else if (objType.IsGenericType)
        {
            describeGeneric(m, ob, level,o);

        }
        else
        {
            o.WriteLine(m.Name() + " is " + objType);
        }
    }*/

   /* public void describeGeneric(FieldOrProperty m, object ob, int level, StreamWriter o)
    {
        //Debug.Log(m.Name()+" "+m.Type());
        if (m.Type().GetGenericTypeDefinition() == typeof(List<>))
        {
            //Debug.Log("a list of " + m.GetValue(ob).GetType().GetGenericArguments()[0] + " named " + m.Name() + "\n");
            o.WriteLine("a list of " + m.GetValue(ob).GetType().GetGenericArguments()[0] + " named " + m.Name());
        }else if (m.Type().GetGenericTypeDefinition() == typeof(HashSet<>))
        {
            // Debug.Log("an hashset of " + m.GetValue(ob).GetType().GetGenericArguments()[0] + " named " + m.Name() + "\n");
            o.WriteLine("an hashset of " + m.GetValue(ob).GetType().GetGenericArguments()[0] + " named " + m.Name());
        }
        else if (m.Type().GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            //Debug.Log("a dictionary of " + m.GetValue(ob).GetType().GetGenericArguments()[0]+","+ m.GetValue(ob).GetType().GetGenericArguments()[1] + " named " + m.Name() + "\n");
            o.WriteLine("a dictionary of " + m.GetValue(ob).GetType().GetGenericArguments()[0] + "," + m.GetValue(ob).GetType().GetGenericArguments()[1] + " named " + m.Name());
        }
    }*/

    internal Type TypeOf(FieldOrProperty f)
    {
        return f.Type();
    }

    public List<FieldOrProperty> GetFieldsAndProperties(object go)
    {
        List<FieldOrProperty> propertiesList = new List<FieldOrProperty>();
        Type t =go.GetType();
        if(go is Type)
        {
            t = (Type)go;
        }
        MemberInfo[] fields = t.GetFields(BindingAttr);
        MemberInfo[] properties = t.GetProperties();
        var members = fields.Union(properties);
        foreach (MemberInfo child in members)
        {
            propertiesList.Add(new FieldOrProperty(child));
        }
        
            return propertiesList;
    }

    public GameObject GetGameObjectWithName(string name)
    {
        return GameObject.Find(name);
    }

    public List<Component> GetComponents(GameObject go)
    {
        List<Component> components = new List<Component>();
        foreach (Component c in go.GetComponents(typeof(Component)))
        {
            if (!components.Contains(c) && c!=null)
            {
                components.Add(c);
            }
        }
        return components;
    }
}