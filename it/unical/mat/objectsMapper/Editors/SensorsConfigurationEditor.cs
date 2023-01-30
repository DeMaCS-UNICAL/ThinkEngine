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
        private bool configurePropertyMode;
        private MyListString actualProperty;

        void Reset()
        {
            sensorConfiguration = target as SensorConfiguration;
            base.Reset();
        }
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
            if (!configurePropertyMode)
            {
                DrawPropertiesExcluding(serializedObject, new string[] { });
                serializedObject.ApplyModifiedProperties();
                base.OnInspectorGUI();
            }
            else
            {
                ConfigureProperty();
            }
        }

        private void ConfigureProperty()
        {
            Debug.Log(actualProperty);
            PropertyFeatures features = configuration.PropertyFeatures.Find(x => x.property.Equals(actualProperty));
            Debug.Log(features);
            features.PropertyName = EditorGUILayout.TextField("Name:", features.PropertyName);
            features.windowWidth = EditorGUILayout.IntSlider("Aggregation Window:", features.windowWidth, 1, 200);
            ShowOperationChoice(features);
            serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button("Done"))
            {
                configurePropertyMode = false;
                actualProperty = null;
            }
        }

        private void ShowOperationChoice(PropertyFeatures features)
        {
            if (MapperManager.NeedsAggregates(configuration.ObjectTracker.PropertyType(actualProperty)))
            {
                //int propertyIndex = actualProperty.GetHashCode();
                int oldOperation = features.operation;
                Type propertyType = sensorConfiguration.ObjectTracker.PropertyType(actualProperty);
                string[] displayedOptions = Enum.GetNames(MapperManager.GetAggregationTypes(propertyType));
                int newOperation = EditorGUILayout.Popup(oldOperation, displayedOptions);
                if (newOperation != oldOperation)
                {
                    sensorConfiguration.SetOperationPerProperty(actualProperty, newOperation);
                }
                if (MapperManager.GetAggregationStreamOperationsIndexes(propertyType).Contains(newOperation) )
                {
                    string oldValue = features.specifValue;
                    string newValue = EditorGUILayout.TextField("Value to track", oldValue);
                    if (!oldValue.Equals(newValue))
                    {
                        try
                        {
                            Convert.ChangeType(features.specifValue, propertyType);
                            sensorConfiguration.SetSpecificValuePerProperty(actualProperty, newValue);
                        }
                        catch
                        {
                            Debug.LogError(newValue + " value not valid for property of type " + propertyType);
                        }
                    }
                    int oldCounter = features.counter;
                    int newCounter = EditorGUILayout.IntSlider("Steps", oldCounter,1, features.windowWidth);
                    if (!oldCounter.Equals(newCounter))
                    {
                        sensorConfiguration.SetCounterPerProperty(actualProperty, newCounter);
                    }
                    
                }
            }
        }

        protected override void SpecificFields(MyListString property)
        {
            if (GUILayout.Button("Configure"))
            {
                configurePropertyMode = true;
                actualProperty = property;
            }
        }
            /*
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
            */
        }
}
#endif