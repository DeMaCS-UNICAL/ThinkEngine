using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NewStructures
{
    [ExecuteInEditMode, Serializable, RequireComponent(typeof(NewMonoBehaviourActuatorsManager))]
    class NewActuatorConfiguration : NewAbstractConfiguration
    {
        public Brain assignedTo;
        private object _triggerClass;
        private object TriggerClass
        {
            get
            {
                if (_triggerClass == null)
                {
                    _triggerClass = Utility.TriggerClass;
                }
                return _triggerClass;
            }
        }
        internal MethodInfo applyMethod;
        protected override void PropertyDeleted(MyListString property)
        {
            
        }

        protected override void PropertySelected(MyListString property)
        {
            
        }
        internal override string ConfigurationName
        {
            set
            {
                if (!Utility.actuatorsManager.IsConfigurationNameValid(value, this))
                {
                    throw new Exception("The chosen configuration name cannot be used.");
                }
                _configurationName = value;
            }
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
            return temporaryName.Equals(ConfigurationName) || Utility.actuatorsManager.IsConfigurationNameValid(temporaryName, this);
        }

        internal override bool IsSensor()
        {
            return false;
        }

        internal bool CheckIfApply()
        {
            if (applyMethod is null)
            {
                return true;
            }
            return (bool)applyMethod.Invoke(TriggerClass, null);
        }
    }
}
