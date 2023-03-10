// Simple helper class that allows you to serialize System.Type objects.
// Use it however you like, but crediting or even just contacting the author would be appreciated (Always 
// nice to see people using your stuff!)
//
// Written by Bryan Keiren (http://www.bryankeiren.com)

using UnityEngine;
using System;

[Serializable]
public class SerializableSystemType : ISerializationCallbackReceiver
{
    [SerializeField]
    private string m_Name;
    public string Name
    {
        get { return m_Name; }
    }

    [SerializeField]
    private string m_AssemblyQualifiedName;
    public string AssemblyQualifiedName
    {
        get { return m_AssemblyQualifiedName; }
    }

    [SerializeField]
    private string m_AssemblyName;
    public string AssemblyName
    {
        get { return m_AssemblyName; }
    }

    private Type m_SystemType;
    public Type SystemType
    {
        get
        {
            if (m_SystemType != null)
            {
                return m_SystemType;
            }
            return null;
        }
    }

    public SerializableSystemType(Type _SystemType)
    {
        m_SystemType = _SystemType;
    }

    public void OnBeforeSerialize()
    {
        if(m_SystemType != null)
        {
            m_Name = m_SystemType.Name;
            m_AssemblyQualifiedName = m_SystemType.AssemblyQualifiedName;
            m_AssemblyName = m_SystemType.Assembly.FullName;
        }
    }

    public void OnAfterDeserialize()
    {
        m_SystemType = Type.GetType(m_AssemblyQualifiedName);
    }

    public override bool Equals(System.Object obj)
    {
        SerializableSystemType temp = obj as SerializableSystemType;
        if ((object)temp == null)
        {
            return false;
        }
        return this.Equals(temp);
    }

    public bool Equals(SerializableSystemType _Object)
    {
        //return m_AssemblyQualifiedName.Equals(_Object.m_AssemblyQualifiedName);
        return _Object.SystemType.Equals(SystemType);
    }

    public override string ToString()
    {
        return m_AssemblyQualifiedName;
    }

    public static bool operator ==(SerializableSystemType a, SerializableSystemType b)
    {
        // If both are null, or both are same instance, return true.
        if (ReferenceEquals(a, b))
        {
            return true;
        }

        // If one is null, but not both, return false.
        if (((object)a == null) || ((object)b == null))
        {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(SerializableSystemType a, SerializableSystemType b)
    {
        return !(a == b);
    }
}
