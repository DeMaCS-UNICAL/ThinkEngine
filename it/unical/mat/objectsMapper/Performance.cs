﻿using System;
using UnityEngine;
using System.IO;
using System.Globalization;

class Performance : MonoBehaviour
{
    private int steps;
    private static readonly string withBrainPath = @"Performance\withBrain.csv";
    private static readonly string withoutBrainPath = @"Performance\withoutBrain.csv";
    private static readonly string factsAndASPath = @"Performance\factsAndAS.csv";
    private static readonly string sensorsUpdateRate = @"Performance\sensors.csv";
    private static readonly string actuatorsUpdateRate = @"Performance\actuators.csv";
    private static string path;
    public static bool updatedSensors;
    public static bool updatingActuators;
    static readonly NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
    public static bool initialized;



    void Start()
    {
        //stopwatch = new Stopwatch();
        //stopwatch.Start();
        //MyDebugger.MyDebug("starting performace");
        //Application.targetFrameRate = 60;
        if (!Directory.Exists("Performance"))
        {
            Directory.CreateDirectory("Performance");
        }
        nfi.NumberDecimalSeparator = ".";
        if (GameObject.FindObjectOfType<ActuatorBrain>().enableBrain)
        {
            using (StreamWriter fs = new StreamWriter(withBrainPath, false))
            {
                fs.Write("Iteration; Rate; AvgRate\n");
                fs.Close();
            }
            path = withBrainPath;

        }
        else
        {
            using (StreamWriter fs = new StreamWriter(withoutBrainPath, false))
            {
                fs.Write("Iteration; Rate; AvgRate\n");
                fs.Close();
            }
            path = withoutBrainPath;

        }
        using (StreamWriter fs = new StreamWriter(sensorsUpdateRate, false))
        {
            fs.Write("Iteration; Rate; AvgFPS; BestAvgFPS \n");
            fs.Close();
        }
        using (StreamWriter fs = new StreamWriter(actuatorsUpdateRate, false))
        {
            fs.Write("Iteration; Rate\n");
            fs.Close();
        }
        initialized = true;
    }
    void LateUpdate()
    {

        double current = 1f / Time.unscaledDeltaTime;
        steps++;
        using (StreamWriter fs = new StreamWriter(path, true))
        {
            fs.Write(steps + ";" + Math.Round(current, 4).ToString("N", nfi) + ";" + SensorsManager.avgFps + ";" + "\n");

            fs.Close();
        }
        if (updatedSensors)
        {
            updatedSensors = false;
            using (StreamWriter fs = new StreamWriter(sensorsUpdateRate, true))
            {
                fs.Write(steps + ";" + Math.Round(current, 4).ToString("N", nfi) + ";"+SensorsManager.avgFps+";"+SensorsManager.bestAvgFps+";"+ FindObjectsOfType<MonoBehaviourSensor>().Length+"\n");
                fs.Close();
            }
        }
        if (updatingActuators)
        {
            updatingActuators = false;
            using (StreamWriter fs = new StreamWriter(actuatorsUpdateRate, true))
            {
                fs.Write(steps + ";" + Math.Round(current, 4).ToString("N", nfi) + "\n");
                fs.Close();
            }
        }
    }

    void OnApplicationQuit()
    {
        /*stopwatch.Stop();
            
        avgFrames =(int)( totalFrames / steps);
        using (StreamWriter fs = new StreamWriter(path, true))
        {
            if (!initialized)
            {
                return;
            }
            using (StreamWriter fs = new StreamWriter(factsAndASPath, true))
            {
                fs.Write(s + " " + d.ToString("N", nfi) + "ms \n");
                fs.Close();
            }
        }
            fs.Write("Brain: "+(GameObject.FindObjectOfType<Brain>().enableBrain)+" "+(stopwatch.ElapsedMilliseconds/1000)+"s \n");
            fs.Write("Average Rate: "+avgFrames + "; Minimum Rate: "+minRate+"; Maximum Rate: "+maxRate+ "\n");
            fs.Close();
        }*/
    }

    public static void WriteOnFile(string s, double d, bool printDate=false)
    {
        if (!initialized)
        {
            return;
        }
        using (StreamWriter fs = new StreamWriter(factsAndASPath, true))
        {
            if (printDate)
            {
                fs.Write("date: " + System.DateTime.Today);
            }
            fs.Write(s + " " + d.ToString("N", nfi) + "ms \n");
            fs.Close();
        }
    }
}