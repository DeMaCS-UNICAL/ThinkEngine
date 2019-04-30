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

namespace EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts
{
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

        public string ASPFilePath;
        public float brainUpdateFrequency;
        private bool updateSensors;
        private bool actuatorsReady;
        public float startIn;

        void Reset() {
            
            
            ASPFilePath = @".\Assets\Resources\" + gameObject.name + ".asp";
            brainUpdateFrequency = 500;
        }

        

        void OnValidate() {
            ASPFilePath = @".\Assets\Resources\" + gameObject.name + ".asp";
        }

        internal void generateFile()
        {
            
            actuators = new List<SimpleActuator>();
            mapper = MappingManager.getInstance();
            foreach (ActuatorConfiguration conf in actuatorsConfigurations)
            {
                actuators.Add(new SimpleActuator(conf));
            }
            IMapper actuatorMapper = mapper.getMapper(typeof(SimpleActuator));
            using (FileStream fs = File.Create(ASPFilePath))
            {
                foreach (SimpleActuator act in actuators)
                {
                   /*foreach (string s in act.properties)
                    {
                        Debug.Log("actuator "+act.actuatorName+" has property "+s );
                    }*/
                    Byte[] info = new UTF8Encoding(true).GetBytes(actuatorMapper.Map(act));
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }
            }
            
        }

        internal IEnumerable<AdvancedSensor> getSensors()
        {
            return sensors;
        }

        void OnEnable()
        {
            sensors = new List<AdvancedSensor>();
            actuators = new List<SimpleActuator>();
            embasp = new EmbASPManager(this);
            //Debug.Log("creating sensors");
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


            executionThread = new Thread(() =>{
                Thread.CurrentThread.IsBackground = true;
                embasp.Run();
            });
            executionThread.Start();
            InvokeRepeating("UpdateSensors", startIn, brainUpdateFrequency);
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
                    Monitor.Pulse(toLock);
                }
          

        }
    }
}
