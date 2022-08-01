using System;
using System.Collections.Generic;
using ThinkEngine.Mappers;

namespace ThinkEngine
{
    internal class ObjectTracker
    {
        public delegate Dictionary<MyListString, KeyValuePair<Type, object>> RetrieveProperties(Type t, MyListString currentHierarchy, object currentObject = null);

        Dictionary<MyListString, int> propertiesIndex;
        Dictionary<int, List<MyListString>> derivedProperties; //for each property index, there is the list of the properties of the property; 

        Dictionary<int, object> propertiesValue; //for each property index, there is the actual instantiation of the property (if not null)
        Dictionary<int, Type> propertiesType;//for each property index, there is the type of the property

        internal Type PropertyType(MyListString property)
        {
            CheckPropertyExistence(property);
            return propertiesType[propertiesIndex[property]];
        }

        public ObjectTracker(object root, bool nullObject = false)
        {
            propertiesIndex = new Dictionary<MyListString, int>();
            derivedProperties = new Dictionary<int, List<MyListString>>();
            propertiesValue = new Dictionary<int, object>();
            propertiesType = new Dictionary<int, Type>();
            if (root == null || nullObject && !(root is Type))
            {
                throw new Exception("Invalid parameters.");
            }
            if (!nullObject)
            {
                propertiesValue.Add(int.MinValue, root);
                RetrieveObjectProperties(root);
            }
            else
            {
                RetrieveTypeProperties((Type)root);
            }
        }
        internal bool IsFinal(MyListString property)
        {
            return MapperManager.IsFinal(PropertyType(property));
        }
        private List<MyListString> GetRootObjectProperties()
        {
            List<MyListString> toReturn = new List<MyListString>();
            foreach (MyListString property in propertiesIndex.Keys)
            {
                if (property.Count == 1)
                {
                    toReturn.Add(property);
                }
            }
            return toReturn;
        }

        internal List<MyListString> GetMemberProperties(MyListString memberName)
        {
            if (memberName.Count == 0)
            {
                return GetRootObjectProperties();
            }
            if (propertiesIndex.ContainsKey(memberName) || GetMemberProperties(memberName.GetRange(0, memberName.Count - 1)) != null)
            {
                int index = propertiesIndex[memberName];
                if (derivedProperties.ContainsKey(index) || RetrieveMemberProperties(memberName))
                {
                    return derivedProperties[index];
                }
            }
            throw new Exception("Property " + memberName + " does not exist.");
        }

        private bool RetrieveMemberProperties(MyListString memberName)
        {
            if (!propertiesIndex.ContainsKey(memberName))
            {
                throw new Exception("Property " + memberName + " not yet discovered.");
            }
            int memberIndex = propertiesIndex[memberName];
            if (derivedProperties.ContainsKey(memberIndex))
            {
                return true;
            }
            if (propertiesValue.ContainsKey(memberIndex) && propertiesValue[memberIndex] != null)
            {
                derivedProperties.Add(memberIndex, RetrieveObjectProperties(propertiesValue[memberIndex], memberName));
                return true;
            }
            if (propertiesType.ContainsKey(memberIndex))
            {
                derivedProperties.Add(memberIndex, RetrieveTypeProperties(propertiesType[memberIndex], memberName));
                return true;
            }
            return false;
        }
        private List<MyListString> RetrieveObjectProperties(object currentObject, MyListString fatherName = null)
        {
            if (currentObject == null)
            {
                throw new Exception("Cannot retrieve properties of null.");
            }
            return RetrieveTypeProperties(currentObject.GetType(), fatherName, currentObject);
        }
        private List<MyListString> RetrieveTypeProperties(Type type, MyListString fatherMember = null, object currentObject = null)
        {
            if (currentObject != null && !currentObject.GetType().Equals(type))
            {
                throw new Exception("Actual object type differs from declared one.");
            }
            List<MyListString> toReturn = new List<MyListString>();
            MyListString baseMember = fatherMember ?? new MyListString();
            Dictionary<MyListString, KeyValuePair<Type, object>> retrievedProperties = MapperManager.RetrieveProperties(type, baseMember, currentObject);
            foreach (MyListString property in retrievedProperties.Keys)
            {
                if (!propertiesIndex.ContainsKey(property))
                {
                    int index = GetIndex(property);
                    propertiesIndex.Add(property, index);
                    propertiesType.Add(index, retrievedProperties[property].Key);
                    if (!propertiesValue.ContainsValue(retrievedProperties[property].Value))
                    {
                        propertiesValue.Add(index, retrievedProperties[property].Value);
                    }
                    else
                    {
                        propertiesValue.Add(index, null);
                    }
                }
                toReturn.Add(property);
            }
            return toReturn;
        }

        private static int GetIndex(MyListString property)
        {
            return property.GetHashCode();
        }

        private void CheckPropertyExistence(MyListString property)
        {
            if (!propertiesIndex.ContainsKey(property))
            {
                RetrieveMemberProperties(property.GetRange(0, property.Count - 1));
            }
            if (!propertiesIndex.ContainsKey(property))
            {
                throw new Exception("Property " + property + " does not exist.");
            }
        }

        internal bool IsPropertyExpandable(MyListString property)
        {
            CheckPropertyExistence(property);
            return MapperManager.IsTypeExpandable(propertiesType[propertiesIndex[property]]);
        }
    }
}