using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace NewStructures.Editors
{
    [CustomEditor(typeof(NewSensorConfiguration))]
    class NewSensorsConfigurationEditor : NewAbstractConfigurationEditor
    {
        NewSensorConfiguration sensorConfiguration;
        void OnDestroy()
        {
            if (target == null && go!=null)
            {
                DestroyImmediate(go.GetComponent<MonoBehaviourSensorsManager>());
            }
        }
        protected override void SpecificFields(MyListString property)
        {
            sensorConfiguration = configuration as NewSensorConfiguration;
            if (MapperManager.NeedsAggregates(configuration.objectTracker.PropertyType(property)))
            {
                int propertyIndex = property.GetHashCode();
                int oldOperation = sensorConfiguration.operationPerProperty[propertyIndex];
                Type propertyType = sensorConfiguration.objectTracker.PropertyType(property);
                string[] displayedOptions = Enum.GetNames(MapperManager.GetAggregationTypes(propertyType));
                int newOperation = EditorGUILayout.Popup(oldOperation, displayedOptions);
                if (newOperation != oldOperation)
                {
                    sensorConfiguration.SetOperationPerProperty(property, newOperation);
                    if(newOperation == MapperManager.GetAggregationSpecificIndex(propertyType))
                    {
                        string oldValue = sensorConfiguration.specificValuePerProperty[propertyIndex];
                        string newValue = EditorGUILayout.TextField("Value to track", oldValue);
                        if (!oldValue.Equals(newValue))
                        {
                            sensorConfiguration.SetSpecificValuePerProperty(property, newValue);
                        }
                    }
                }
            }
        }
    }
}
