using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NewStructures
{
    [ExecuteInEditMode, Serializable, RequireComponent(typeof(NewMonoBehaviourActuatorsManager))]
    class NewActuatorConfiguration : NewAbstractConfiguration
    {
        protected override void PropertyDeleted(MyListString property)
        {
            
        }

        protected override void PropertySelected(MyListString property)
        {
            
        }

        internal override string GetAutoConfigurationName()
        {
            string name;
            string toAppend = "";
            int count = 0;
            do
            {
                name = char.ToLower(gameObject.name[0]).ToString() + gameObject.name.Substring(1) + "Actuator" + toAppend;
                toAppend += count;
                count++;
            }
            while (!Utility.actuatorsManager.IsConfigurationNameValid(name, this));
            return name;
        }

        internal override bool IsAValidName(string temporaryName)
        {
            return Utility.actuatorsManager.IsConfigurationNameValid(temporaryName, this);
        }

        internal override bool IsSensor()
        {
            return false;
        }
    }
}
