﻿using EmbASP4Unity.it.unical.mat.objectsMapper.ActuatorsScripts;
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
    [ExecuteInEditMode]
    public class Brain :MonoBehaviour
    {
        public readonly object toLock = new object();
        public List<SensorConfiguration> sensorsConfigurations;
        public List<ActuatorConfiguration> actuatorsConfigurations;
        private List<AdvancedSensor> sensors;
        private List<SimpleActuator> actuators;
        private MappingManager mapper;
        private Thread executionThread;
        private SolverExectuor embasp;
        int count = 0;
        private static new System.Timers.Timer timer;
        private bool sensorsUpdated;

        public string ASPFilePath;
       
        private bool updateSensors;
        private bool actuatorsReady;
        public bool executeRepeatedly;
        public float startIn;
        public float brainUpdateFrequency;
        public bool executeOnTrigger;
        public string triggerClassPath;
        public string updateSensorsOn="";
        private MethodInfo sensorsUpdateMethod;
        private MethodInfo reasonerMethod;
        private object triggerClass;
        public string executeReasonerOn;
        public string applyActuatorsCondition;
        private MethodInfo applyActuatorsMethod;
        private SensorsManager sensorManager;

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

        internal bool actuatorsUpdateCondition()
        {
            if (applyActuatorsMethod != null)
            {
                return (bool)applyActuatorsMethod.Invoke(triggerClass, null);
            }else if (applyActuatorsCondition.Equals("Never"))
            {
                return false;
            }
            return true;
        }

        void Reset() {
            executeRepeatedly = true;
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
            embasp = new SolverExectuor(this);
            triggerClass = ScriptableObject.CreateInstance("Trigger");
            MethodInfo[] methods = triggerClass.GetType().GetMethods();
            //Debug.Log("creating sensors");
            foreach (SensorConfiguration conf in sensorsConfigurations)
            {
                sensors.Add(new AdvancedSensor(conf));
                //Debug.Log(conf.configurationName+" added");
            }
            sensorManager = SensorsManager.GetInstance();
            sensorManager.registerSensors(this, sensors);
            foreach (ActuatorConfiguration conf in actuatorsConfigurations)
            {
                actuators.Add(new SimpleActuator(conf));
                //Debug.Log(conf.configurationName+" added");
            }
            ActuatorsManager actuatorsManager = ActuatorsManager.GetInstance();
            actuatorsManager.registerActuators(this, actuators);
            if (!actuatorsManager.applyCoroutinStarted)
            {
                StartCoroutine(actuatorsManager.applyActuators());
                actuatorsManager.applyCoroutinStarted = true;
            }
            if(executeReasonerOn.Equals("When Sensors are ready"))
            {
                StartCoroutine("pulseOnSensorsReady");
            }
            else
            {
                foreach (MethodInfo mI in methods)
                {
                    if (mI.Name.Equals(updateSensorsOn))
                    {
                        //Debug.Log(mI.Name);
                        reasonerMethod = mI;
                        StartCoroutine("pulseOn");
                    }
                }
            }

            //Debug.Log("trigger method is "+triggerMethod);
            if (executeRepeatedly)
            {
                InvokeRepeating("UpdateSensors", startIn, brainUpdateFrequency);
            }
            else if (!updateSensorsOn.Equals(""))
            {
                foreach (MethodInfo mI in methods)
                {
                    if (mI.Name.Equals(updateSensorsOn))
                    {
                        //Debug.Log(mI.Name);
                        sensorsUpdateMethod = mI;
                        StartCoroutine("UpdateSensorsOnTrigger");
                    }
                }
            }
            foreach (MethodInfo mI in methods)
            {
                if (mI.Name.Equals(applyActuatorsCondition))
                {
                    //Debug.Log(mI.Name);
                    applyActuatorsMethod = mI;
                }
            }
            executionThread = new Thread(() => {
                Thread.CurrentThread.IsBackground = true;
                embasp.Run();
            });
            executionThread.Start();
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
                sensorManager.updateSensors(this);
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
