using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NewStructures.NewMappers
{
    internal class InstantiationInformation
    {
        internal GameObject instantiateOn;
        internal object currentObjectOfTheHierarchy;
        internal Type currentType;
        internal MyListString propertyHierarchy;
        internal MyListString residualPropertyHierarchy;
        internal int firstPlaceholder;
        internal List<string> prependMapping;
        internal List<string> appendMapping;
        internal bool mappingDone;
        internal List<IInfoAndValue> hierarchyInfo;
        internal NewAbstractConfiguration configuration;

        internal InstantiationInformation()
        {
            hierarchyInfo = new List<IInfoAndValue>();
            prependMapping = new List<string>();
            appendMapping = new List<string>();
        }
        internal InstantiationInformation(InstantiationInformation original)
        {
            instantiateOn = original.instantiateOn;
            currentObjectOfTheHierarchy = original.currentObjectOfTheHierarchy;
            propertyHierarchy = original.propertyHierarchy;
            residualPropertyHierarchy = new MyListString( original.residualPropertyHierarchy.myStrings);
            firstPlaceholder = original.firstPlaceholder;
            prependMapping = new List<string>(original.prependMapping);
            appendMapping = new List<string>(original.appendMapping);
            configuration = original.configuration;
            hierarchyInfo = new List<IInfoAndValue>(original.hierarchyInfo);
            currentType = original.currentType;
        }

        internal string Mapping()
        {
            string toReturn = "";
            for (int i=0; i<prependMapping.Count;i++)
            {
                toReturn += prependMapping[i];
            }
            for (int i = 0; i < appendMapping.Count; i++)
            {
                toReturn += appendMapping[i];
            }
            return toReturn;
        }
    }
}
