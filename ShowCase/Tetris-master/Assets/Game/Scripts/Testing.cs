using EmbASP4Unity.it.unical.mat.objectsMapper;
using EmbASP4Unity.it.unical.mat.objectsMapper.ActuatorsScripts;
using EmbASP4Unity.it.unical.mat.objectsMapper.Mappers;
using EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts
{
    class Testing:MonoBehaviour
    {
        public SensorsManager sensorManager;
        public ActuatorsManager actuatorManager;
        public MappingManager mappingManager;
        public int toUpdate;

        void OnEnable()
        {
            /*mappingManager = MappingManager.getInstance();
            List<AdvancedSensor> sensors = new List<AdvancedSensor>();
            List<SimpleActuator> actuators = new List<SimpleActuator>();
            foreach(AbstractConfiguration s in sensorManager.confs())
            {
                sensors.Add(new AdvancedSensor((SensorConfiguration)s));
                sensors[sensors.Count - 1].UpdateProperties();
                Debug.Log(mappingManager.getMapper(typeof(AdvancedSensor)).Map(sensors[sensors.Count - 1]));
            }
            foreach (AbstractConfiguration s in actuatorManager.confs())
            {
                actuators.Add(new SimpleActuator((ActuatorConfiguration)s));
            }*/
        }
    }
}
