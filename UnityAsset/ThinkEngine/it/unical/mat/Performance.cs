using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using UnityEngine;
using System.IO;
using EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts;
using EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts;
using System.Globalization;
using Debug = UnityEngine.Debug;


class Performance : MonoBehaviour
{
    private int steps;
    private double avgFrames;
    private int minRate = int.MaxValue;
    private int maxRate = int.MinValue;
    private double totalFrames;
    private Stopwatch stopwatch;
    private static string withBrainPath = @"Performance\withBrain.csv";
    private static string withoutBrainPath = @"Performance\withoutBrain.csv";
    private static string factsAndASPath = @"Performance\factsAndAS.csv";
    private static string sensorsUpdateRate = @"Performance\sensors.csv";
    private static string actuatorsUpdateRate = @"Performance\actuators.csv";
    private static string path;
    public static bool updatingSensors;
    public static bool updatingActuators;
    static NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
    public static bool initialized;



    void Start()
    {
        //stopwatch = new Stopwatch();
        //stopwatch.Start();
        //Debug.Log("starting performace");
        if (!Directory.Exists("Performance"))
        {
            Directory.CreateDirectory("Performance");
        }
        nfi.NumberDecimalSeparator = ".";
        if (GameObject.FindObjectOfType<Brain>().enableBrain)
        {
            using (StreamWriter fs = new StreamWriter(withBrainPath, false))
            {
                fs.Write("Iteration; Rate\n");
                fs.Close();
            }
            path = withBrainPath;

        }
        else
        {
            using (StreamWriter fs = new StreamWriter(withoutBrainPath, false))
            {
                fs.Write("Iteration; Rate\n");
                fs.Close();
            }
        
        }
        initialized = true;
    }
    void Update()
    {

        double current = 1f / Time.unscaledDeltaTime;
        //UnityEngine.//Debug.Log(current);
        //totalFrames +=current;
        steps++;
        // avgFrames += current;
        // if (steps % 20 == 0)
        //{
        // avgFrames /= 20;
        using (StreamWriter fs = new StreamWriter(path, true))
        {
            fs.Write(steps + ";" + Math.Round(current, 4).ToString("N", nfi) + "\n");

            fs.Close();
        }
        if (updatingSensors)
        {
            updatingSensors = false;
            using (StreamWriter fs = new StreamWriter(sensorsUpdateRate, true))
            {
                fs.Write(steps + ";" + Math.Round(current, 4).ToString("N", nfi) + "\n");
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
        //avgFrames=0;
        /* if (minRate > current & steps>5)
            {
                minRate = (int)current;
            }
            if (maxRate < current)
            {
                maxRate = (int)current;
            }*/
        //  }
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

    public static void writeOnFile(string s, double d)
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
}
