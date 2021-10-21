using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts;
using UnityEngine;

namespace Planner
{
    [ExecuteAlways, RequireComponent(typeof(IndexTracker))]
    public abstract class Brain: MonoBehaviour
    {
        public bool enableBrain = true;
        public bool debug = true;
        public bool maintainFactFile;
        [SerializeField, HideInInspector]
        protected List<string> _chosenSensorConfigurations;
        internal List<string> ChosenSensorConfigurations
        {
            get
            {
                if (_chosenSensorConfigurations == null)
                {
                    _chosenSensorConfigurations = new List<string>();
                }
                return _chosenSensorConfigurations;
            }
        }
        [SerializeField, HideInInspector]
        internal string ASPFilesPath;
        [SerializeField, HideInInspector]
        internal string ASPFilesPrefix;
        [SerializeField, HideInInspector]
        protected string _ASPFileTemplatePath;

        [SerializeField, HideInInspector]
        internal bool prefabBrain;
        internal string ASPFileTemplatePath
        {
            get
            {
                if (_ASPFileTemplatePath == null || !_ASPFileTemplatePath.Equals(this.GetType().Name + "Template" + ASPFilesPrefix+ ".asp"))
                {
                    _ASPFileTemplatePath = @".\Assets\Resources\" + this.GetType().Name + "Template" + ASPFilesPrefix + ".asp";
                    if (!File.Exists(_ASPFileTemplatePath))
                    {
                        if (!Directory.Exists(@".\Assets\Resources"))
                        {
                            Directory.CreateDirectory(@".\Assets\Resources");
                        }
                        File.Create(_ASPFileTemplatePath);
                    }
                }
                return _ASPFileTemplatePath;
            }
        }
        [SerializeField, HideInInspector]
        protected string _executeReasonerOn;
        internal string ExecuteReasonerOn
        {
            get
            {
                if (_executeReasonerOn == null)
                {
                    _executeReasonerOn = "";
                }
                return _executeReasonerOn;
            }
            set
            {
                _executeReasonerOn = value;
            }
        }
        [SerializeField, HideInInspector]
        internal bool specificASPFile;
        [SerializeField, HideInInspector]
        internal bool globalASPFile;
        protected object triggerClass;
        internal readonly object toLock = new object();
        internal string sensorsMapping;
        internal bool sensorsConfigurationsChanged;
        protected string originalName;
        internal MethodInfo reasonerMethod;
        internal bool missingData;
        protected Thread executionThread;
        internal Executor executor;
        internal bool solverWaiting;

        void OnEnable()
        {
            originalName = gameObject.name;
        }

        protected void Start()
        {
            Utility.LoadPrefabs();
            if (Application.isPlaying && enableBrain)
            {
                triggerClass = Utility.TriggerClass;
                StartCoroutine(Init());
            }
        }
        
        protected virtual void Update()
        {
            if (reasonerMethod == null)
            {
                lock (toLock)
                {
                    if (!SomeConfigurationAvailable())
                    {
                        missingData = true;
                        return;
                    }
                    if (solverWaiting)
                    {
                        solverWaiting = false;
                        Monitor.Pulse(toLock);
                    }
                }
            }
        }
        internal void RemoveNullSensorConfigurations()
        {
            ChosenSensorConfigurations.RemoveAll(x => !Utility.SensorsManager.ExistsConfigurationWithName(x));
        }
        protected void PrepareSensors()
        {
            Utility.SensorsManager.RegisterBrainsSensorConfigurations(this, ChosenSensorConfigurations);
        }
        protected IEnumerator PulseOn()
        {
            while (true)
            {
                yield return new WaitUntil(() => solverWaiting && SomeConfigurationAvailable() && (bool)reasonerMethod.Invoke(triggerClass, null));
                lock (toLock)
                {
                    solverWaiting = false;
                    Monitor.Pulse(toLock);
                }
            }
        }
        internal virtual void GenerateFile()
        {
            using (StreamWriter fs = File.CreateText(ASPFileTemplatePath))
            {
                fs.Write("%For runtime instantiated GameObject, only the prefab mapping is provided. Use that one substituting the gameobject name accordingly.\n %Sensors.\n");
                HashSet<string> seenSensorConfNames = new HashSet<string>();
                
                foreach (SensorConfiguration sensorConf in Utility.SensorsManager.GetConfigurations(ChosenSensorConfigurations))
                {
                    if (seenSensorConfNames.Contains(sensorConf.ConfigurationName))
                    {
                        continue;
                    }
                    seenSensorConfNames.Add(sensorConf.ConfigurationName);
                    foreach (MyListString property in sensorConf.ToMapProperties)
                    {
                        fs.Write(MapperManager.GetASPTemplate(sensorConf.ConfigurationName, sensorConf.gameObject, property, true));
                    }
                }
            }
        }
        protected virtual IEnumerator Init()
        {
            if (specificASPFile && originalName.Equals(gameObject.name))
            {
                yield return new WaitUntil(() => !originalName.Equals(gameObject.name));
            }
            PrepareSensors();
            if (!SomeConfigurationAvailable())
            {
                missingData = true;
            }
            if (!ExecuteReasonerOn.Equals("When Sensors are ready"))
            {
                reasonerMethod = Utility.GetTriggerMethod(ExecuteReasonerOn);
                if (reasonerMethod != null)
                {
                    StartCoroutine(PulseOn());
                }
            }
        }
        protected virtual bool SomeConfigurationAvailable()
        {
            return Utility.SensorsManager.IsSomeActiveInScene(ChosenSensorConfigurations);
        }
    }
    
}
