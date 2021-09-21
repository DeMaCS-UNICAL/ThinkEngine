using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NewStructures
{
    class NewMonoBehaviourSensor : MonoBehaviour
    {
        internal string configurationName;
        internal MyListString property;
        internal Type currentPropertyType;
        internal InfoAndValue propertyInfo;
        public int count = 0;

        public void Configure(string name, MyListString property, Type currentType, InfoAndValue info)
        {
            configurationName = name;
            this.property = property;
            currentPropertyType = currentType;
            propertyInfo = info;
        }
        void Update()
        {
            count++;

            if (count % 1000 == 0)
            {
                Debug.Log(MapperManager.GetSensorBasicMap(this));
            }
        }
        void LateUpdate()
        {
            if (count % 100 == 0)
            {
                MapperManager.UpdateSensor(this);
            }
        }
    }
}
