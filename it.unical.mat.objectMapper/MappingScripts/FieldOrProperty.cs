using System;
using System.Reflection;
using UnityEngine;

public class FieldOrProperty
{
    private FieldInfo f;
    private PropertyInfo p;

    public FieldOrProperty(MemberInfo m)
    {
        //Debug.Log(m.MemberType);
        if(m.MemberType == MemberTypes.Field)
        {
            f = (FieldInfo)m;
        }
        else
        {
            p = (PropertyInfo)m;
        }
    }

    public  Type Type()
    {
        return f == null ? p.PropertyType : f.FieldType;
    }

    public string Name()
    {
        return f == null ? p.Name : f.Name;
    }

    public object GetValue(object ob)
    {
        return f == null ? p.GetValue(ob, null) : f.GetValue(ob);
    }
}