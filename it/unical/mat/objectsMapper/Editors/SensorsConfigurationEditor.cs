#if UNITY_EDITOR
using System;
using ThinkEngine.Mappers;
using UnityEditor;
using UnityEngine;

namespace ThinkEngine.Editors
{
    [CustomEditor(typeof(SensorConfiguration))]
    class SensorsConfigurationEditor : AbstractConfigurationEditor
    {
        SensorConfiguration sensorConfiguration;
        void OnDestroy()
        {
            if (!Application.isPlaying && target == null && go != null)
            {
                if (go.GetComponent<SensorConfiguration>() == null)
                {
                    DestroyImmediate(go.GetComponent<MonoBehaviourSensorsManager>());
                }
            }
        }
        override public void OnInspectorGUI()
        {
            DrawPropertiesExcluding(serializedObject, new string[] { });
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
        protected override void SpecificFields(MyListString property)
        {
            sensorConfiguration = configuration as SensorConfiguration;
            if (MapperManager.NeedsAggregates(configuration.ObjectTracker.PropertyType(property)))
            {
                int propertyIndex = property.GetHashCode();
                int oldOperation = sensorConfiguration.OperationPerProperty[propertyIndex];
                Type propertyType = sensorConfiguration.ObjectTracker.PropertyType(property);
                string[] displayedOptions = Enum.GetNames(MapperManager.GetAggregationTypes(propertyType));
                int newOperation = EditorGUILayout.Popup(oldOperation, displayedOptions);
                if (newOperation != oldOperation)
                {
                    sensorConfiguration.SetOperationPerProperty(property, newOperation);
                    if (newOperation == MapperManager.GetAggregationSpecificIndex(propertyType))
                    {
                        string oldValue = sensorConfiguration.SpecificValuePerProperty[propertyIndex];
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
#endif