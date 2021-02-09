using System;
using System.Collections;
using System.Collections.Generic;

public class MyPropertyInfo
{
    public bool isBasic;
    public List<FieldOrProperty> properties;
    public object value;
    public List<Type> propertiesType;
    public Type elementType;

    public MyPropertyInfo()
    {
        propertiesType = new List<Type>();
    }
    public virtual bool IsNull() { return false; }
    public virtual object Collection() { return null; }
    public virtual int Size(int i) { return 0; }


}
public class ArrayInfo : MyPropertyInfo
{
    public Array array;
    public int[] size;
    public ArrayInfo() :base()
    {
        size = new int[2];
    }
    public override bool IsNull() 
    { 
        return array == null; 
    }
    public override object Collection() 
    {
        return array;
    }
    public override int Size(int i)
    {
        return size[i];
    }
}
public class ListInfo : MyPropertyInfo
{
    public IList list;
    public int count;
    public override bool IsNull()
    {
        return list == null;
    }
    public override object Collection()
    {
        return list;
    }
    public override int Size(int i)
    {
        return count;
    }
}