using EmbASP4Unity.it.unical.mat.objectsMapper.ActuatorsScripts;
using EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;
using EmbASP4Unity.it.unical.mat.objectsMapper.Mappers;
using System.Threading;
using System.Timers;
using System.Reflection;
using System.Collections;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts
{
    [ExecuteInEditMode,Serializable]
    public class Brain :MonoBehaviour
    {
        public readonly object toLock = new object();
        public List<SensorConfiguration> sensorsConfigurations;
        public List<ActuatorConfiguration> actuatorsConfigurations;
        private List<AdvancedSensor> sensors;
        private List<SimpleActuator> actuators;
        private MappingManager mapper;
        private Thread executionThread;
        private EmbASPManager embasp;
        int count = 0;
        private static new System.Timers.Timer timer;
        private bool sensorsUpdated;
        private object triggerClass;
        public string triggerClassPath;
        public string ASPFilePath;       
        private bool updateSensors;
        private bool actuatorsReady;

        public bool updateSensorsRepeatedly;
        public float startIn;
        public float brainUpdateFrequency;

        public bool updateSensorsOnTrigger;
        public string sensorsTriggerMethod="";
        private MethodInfo sensorsUpdateMethod;

        public bool updateSensorsOnCollision;
        public List<string> sensorsCollidersGONames;

        public bool executeReasonerOnTrigger;
        public string reasonerTriggerMethod = "";
        public MethodInfo reasonerMethod;

        public bool executeReasonerOnCollision;
        public List<string> reasonerCollidersGONames;
        public bool executeReasonerAsSoonAsPossible;

        void Awake()
        {
            if (Application.isEditor && !File.Exists(triggerClassPath)) {
                using (FileStream fs = File.Create(triggerClassPath))
                {
                    string triggerClassContent = "using System;\n";
                    triggerClassContent+="using UnityEngine;\n\n" ;
                    triggerClassContent += @"// every method of this class returning a bool value can be used to trigger the sensors update.";
                    triggerClassContent += "\n public class Trigger:ScriptableObject{\n\n";
                    triggerClassContent += "}";
                    Byte[] info = new UTF8Encoding(true).GetBytes(triggerClassContent);
                    fs.Write(info, 0, info.Length);
                }
            }
            else if(Application.isPlaying)
            {
                initBrain();
            }

        }

        

        void Reset() {
            updateSensorsRepeatedly = true;
            triggerClassPath = @".\Assets\Game\Scripts\Trigger.cs";
            ASPFilePath = @".\Assets\Resources\" + gameObject.name + ".asp";
            brainUpdateFrequency = 500;
        }

        

        void OnValidate() {
            triggerClassPath = @".\Assets\Game\Scripts\Trigger.cs";
            ASPFilePath = @".\Assets\Resources\" + gameObject.name + ".asp";
        }

        internal void generateFile()
        {
            actuators = new List<SimpleActuator>();
            sensors = new List<AdvancedSensor>();
            mapper = MappingManager.getInstance();
            foreach (ActuatorConfiguration conf in actuatorsConfigurations)
            {
                actuators.Add(new SimpleActuator(conf));
            }
            foreach (SensorConfiguration conf in sensorsConfigurations)
            {
                sensors.Add(new AdvancedSensor(conf));
            }
            IMapper actuatorMapper = mapper.getMapper(typeof(SimpleActuator));
            ASPAdvancedSensorMapper sensorMapper = (ASPAdvancedSensorMapper)mapper.getMapper(typeof(AdvancedSensor));
            using (FileStream fs = File.Create(ASPFilePath))
            {
                foreach (SimpleActuator act in actuators)
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes(actuatorMapper.Map(act));
                    fs.Write(info, 0, info.Length);
                }
                foreach (AdvancedSensor sens in sensors)
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes(sensorMapper.getASPRepresentation(sens));
                    fs.Write(info, 0, info.Length);
                }
            }
        }

        internal bool sensorsReady()
        {
            if (sensorsUpdated)
            {
                sensorsUpdated = false;
                return true;
            }
            return false;
        }

        internal IEnumerable<AdvancedSensor> getSensors()
        {
            return sensors;
        }

        void initBrain()
        {
            sensors = new List<AdvancedSensor>();
            actuators = new List<SimpleActuator>();
            embasp = new EmbASPManager(this);
            triggerClass = ScriptableObject.CreateInstance("Trigger");
            MethodInfo[] methods = triggerClass.GetType().GetMethods();
            //Debug.Log("creating sensors");
            SensorsAndActuatorCreation();
            ReasonOn(methods);
            UpdateSensorsOn(methods);

            //Debug.Log("trigger method is "+triggerMethod);
            
            executionThread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                embasp.Run();
            });
            //executionThread.Start();

        }

        private void UpdateSensorsOn(MethodInfo[] methods)
        {
            if (updateSensorsRepeatedly)
            {
                InvokeRepeating("UpdateSensors", startIn, brainUpdateFrequency);
            }
            else if (updateSensorsOnTrigger)
            {
                foreach (MethodInfo mI in methods)
                {
                    if (mI.Name.Equals(sensorsTriggerMethod))
                    {
                        //Debug.Log(mI.Name);
                        sensorsUpdateMethod = mI;
                        StartCoroutine("UpdateSensorsOnTrigger");
                    }
                }
            }else if (updateSensorsOnCollision)
            {
                foreach (string coll in sensorsCollidersGONames)
                {
                    GameObject temp = GameObject.Find(coll);
                    SensorsTrigger trigger = temp.AddComponent<SensorsTrigger>();
                    trigger.brain = this;
                }
            }
        }

        private void ReasonOn(MethodInfo[] methods)
        {
            if( executeReasonerAsSoonAsPossible)
            {
                StartCoroutine("pulseOnSensorsReady");
            }
            else if(executeReasonerOnTrigger)
            {
                foreach (MethodInfo mI in methods)
                {
                    if (mI.Name.Equals(sensorsTriggerMethod))
                    {
                        //Debug.Log(mI.Name);
                        reasonerMethod = mI;
                        StartCoroutine("pulseOn");
                    }
                }
            }else if (executeReasonerOnCollision)
            {
                foreach(string coll in reasonerCollidersGONames)
                {
                    GameObject temp = GameObject.Find(coll);
                    ReasonerTrigger trigger = temp.AddComponent<ReasonerTrigger>();
                    trigger.brain = this;
                }
            }
        }

        private void SensorsAndActuatorCreation()
        {
            foreach (SensorConfiguration conf in sensorsConfigurations)
            {
                sensors.Add(new AdvancedSensor(conf));
                //Debug.Log(conf.configurationName+" added");
            }
            foreach (ActuatorConfiguration conf in actuatorsConfigurations)
            {
                actuators.Add(new SimpleActuator(conf));
                //Debug.Log(conf.configurationName+" added");
            }
        }

        private IEnumerator pulseOn()
        {
            while (true)
            {
                yield return new WaitUntil(() => (bool)reasonerMethod.Invoke(triggerClass, null));
                lock (toLock)
                {
                    Monitor.Pulse(toLock);
                }
            }
        }
        private IEnumerator pulseOnSensorsReady()
        {
            while (true)
            {
                Debug.Log("waiting sensors");
                yield return new WaitUntil(() => sensorsReady());
                Debug.Log("pulsing");
                lock (toLock)
                {
                    Monitor.Pulse(toLock);
                }
            }
        }

        public void setActuatorsReady(bool v)
        {
            actuatorsReady = v;
        }

        public bool areActuatorsReady()
        {
            return actuatorsReady;
        }

        public IEnumerable<SimpleActuator> getActuators()
        {
            return actuators;
        }

        private void enableUpdateSensors(object sender, ElapsedEventArgs e)
        {
            updateSensors = true;
        }

        void OnApplicationQuit()
        {
            if (timer != null)
            {
                timer.Enabled = false;
            }
            if (embasp != null) {
                embasp.reason = false;
            }
        }

        


       void UpdateSensors()
       {
            
                lock (toLock)
                {
                    //Debug.Log("updating sensors");

                    foreach (AdvancedSensor sensor in sensors)
                    {
                        sensor.UpdateProperties();
                        //Debug.Log(sensor.sensorName + " updated");
                    }
                    sensorsUpdated = true;
                }
          

        }
        IEnumerator UpdateSensorsOnTrigger()
        {
            while (true)
            {
                //Debug.Log("waiting");
                yield return new WaitUntil(() => (bool)sensorsUpdateMethod.Invoke(triggerClass,null));
                UpdateSensors();
                //Debug.Log("sensors updated");
            }

        }
    }
}
