using EmbASP4Unity.it.unical.mat.objectsMapper.ActuatorsScripts;
using EmbASP4Unity.it.unical.mat.objectsMapper.Mappers;
using EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts;
/*using it.unical.mat.embasp.@base;
using it.unical.mat.embasp.languages.asp;
using it.unical.mat.embasp.platforms.desktop;
using it.unical.mat.embasp.specializations.dlv.desktop;*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts
{
    public class EmbASPManager
    {
        public Brain brain;
        string factsPath;
        MappingManager mapper;
        public bool reason;

        public EmbASPManager(Brain b)
        {
            brain = b;
            factsPath = "Assets/Resources/" + brain.gameObject.name + "Facts.asp";
            mapper = MappingManager.getInstance();
        }
        
        public void Run()
        {
            reason = true;
            IMapper sensorMapper = mapper.getMapper(typeof(AdvancedSensor));
            //Debug.Log("mapper " + sensorMapper);
            Debug.Log(Path.GetTempPath());
            while (reason)
            {
                Thread.Sleep(5000);
                //Debug.Log("executing thread");
                lock (brain.toLock)
                {
                    try
                    {
                        factsPath = Path.GetTempFileName();
                        using (StreamWriter fs = new StreamWriter(factsPath, true))
                        {
                            //Debug.Log("creating file");
                            foreach (AdvancedSensor sensor in brain.getSensors())
                            {
                                //Debug.Log("sensor " + sensor.sensorName);
                                if (!sensor.dataAvailable)
                                {
                                    Monitor.Wait(brain.toLock);
                                }
                                string toAppend = sensorMapper.Map(sensor);
                                //Debug.Log(toAppend);
                                fs.Write(toAppend);

                            }
                            //Debug.Lof(fs.)
                            fs.Close();
                            //Debug.Log("closing stream");
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }

                }
                string test = "setOnActuator(Player(AI(AssetsScriptsAIPlayer(numOfMove(2)))))" +
                    " setOnActuator(Player(AI(AssetsScriptsAIPlayer(typeOfLateralMove(left)))))" +
                    " setOnActuator(Player(AI(AssetsScriptsAIPlayer(numOfLateralMove(1)))))" +
                    " setOnActuator(Player(AI(AssetsScriptsAIPlayer(numOfRotation(1)))))";

                lock (brain.toLock)
                {
                    foreach(SimpleActuator actuator in brain.getActuators())
                    {
                        actuator.parse(test);
                    }
                    brain.setActuatorsReady(true);
                }
                /* Handler handler = new DesktopHandler(new DLVDesktopService(@".\lib\dlv.exe"));
                 InputProgram encoding = new ASPInputProgram();
                 encoding.AddFilesPath(brain.ASPFilePath);
                 InputProgram facts = new ASPInputProgram();
                 facts.AddFilesPath(factsPath);
                 handler.AddProgram(encoding);
                 handler.AddProgram(facts);
                 Output o = handler.StartSync();*/
                //File.Delete(factsPath);
            }
        }
    }
}
