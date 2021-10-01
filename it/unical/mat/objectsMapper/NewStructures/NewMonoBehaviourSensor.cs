using newMappers;
using NewStructures.NewMappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NewStructures
{
    [RequireComponent(typeof(IndexTracker))]
    class NewMonoBehaviourSensor : MonoBehaviour
    {
        internal string configurationName;
        private MyListString property;
        private List<IInfoAndValue> _propertyInfo;
        internal List<IInfoAndValue> PropertyInfo
        {
            get
            {
                if (_propertyInfo == null)
                {
                    _propertyInfo = new List<IInfoAndValue>();
                }
                return _propertyInfo;
            }
        }
        private string _mapping;
        internal string Mapping
        {
            get
            {
                return _mapping;
            }
        }
        public int count = 0;

        public void Configure(InstantiationInformation  information, string mapping)
        {
            configurationName = information.configuration.name;
            property = new MyListString(information.propertyHierarchy.myStrings);
            PropertyInfo.AddRange(information.hierarchyInfo);
            int index = GetComponent<IndexTracker>().CurrentIndex;
            this._mapping = NewASPMapperHelper.AspFormat(configurationName)+"("+NewASPMapperHelper.AspFormat(gameObject.name)+", objectIndex("+index+"),"+mapping;
        }
        void Update()
        {
            count++;

            if (count % 1000 == 0)
            {
                Debug.Log(MapperManager.GetSensorBasicMap(this,gameObject,new MyListString(property.myStrings),new List<object>(), 0));
            }
        }
        void LateUpdate()
        {
            if (count % 100 == 0)
            {
                MapperManager.UpdateSensor(this, gameObject, new MyListString(property.myStrings), 0);
            }
        }
    }
}
