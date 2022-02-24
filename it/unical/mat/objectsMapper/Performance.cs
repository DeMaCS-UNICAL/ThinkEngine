using System;
using UnityEngine;
using System.IO;
using System.Globalization;
using Planner;
using ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts;

class Performance : MonoBehaviour
{
    private int steps;
    private static readonly string withBrainPath = @"Performance\withBrain.csv";
    private static readonly string withoutBrainPath = @"Performance\withoutBrain.csv";
    private static readonly string factsAndASPath = @"Performance\factsAndAS.csv";
    private static readonly string sensorsUpdateRate = @"Performance\sensors.csv";
    private static readonly string actuatorsUpdateRate = @"Performance\actuators.csv";
    private static readonly string plansUpdateRate = @"Performance\plans.csv";
    private static string path;
    public static bool updatedSensors;
    public static bool updatingActuators;
    static readonly NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
    public static bool initialized;



    void Start()
    {

        if (!Directory.Exists("Performance"))
        {
            Directory.CreateDirectory("Performance");
        }
        nfi.NumberDecimalSeparator = ".";
        Brain[] brains = GameObject.FindObjectsOfType<Brain>();
        bool found = false;
        foreach (Brain brain in brains)
        {
            if (brain != null && brain.enableBrain)
            {
                found = true;
                using (StreamWriter fs = new StreamWriter(withBrainPath, true))
                {
                    fs.Write("\nIteration; Rate; AvgRate\n");
                    fs.Close();
                }
                path = withBrainPath;

                using (StreamWriter fs = new StreamWriter(sensorsUpdateRate, true))
                {
                    fs.Write("\nIteration; Rate; AvgRate; NumberOfSensors \n");
                    fs.Close();
                }
                if (FindObjectOfType<ActuatorBrain>() != null)
                {
                    using (StreamWriter fs = new StreamWriter(actuatorsUpdateRate, true))
                    {
                        fs.Write("\nIteration; Rate\n");
                        fs.Close();
                    }
                }
                if (FindObjectOfType<PlannerBrain>() != null)
                {
                    using (StreamWriter fs = new StreamWriter(plansUpdateRate, true))
                    {
                        fs.Write("\nIteration; Rate; AvgRate; NumberOfPlansChecking; TotalNumberOfActions \n");
                        fs.Close();
                    }
                }
                break;
            }
        }
        if (!found)
        {
            using (StreamWriter fs = new StreamWriter(withoutBrainPath, true))
            {
                fs.Write("\nIteration; Rate; AvgRate\n");
                fs.Close();
            }
            path = withoutBrainPath;

        }
        initialized = true;
    }
    void LateUpdate()
    {

        double current = 1f / Time.unscaledDeltaTime;
        steps++;
        using (StreamWriter fs = new StreamWriter(path, true))
        {
            fs.Write(steps + ";" + Math.Round(current, 4).ToString("N", nfi) + ";" + Utility.SensorsManager.avgFps + ";" + "\n");

            fs.Close();
        }
        if (path.Equals(withBrainPath))
        {
            if (!Executor._canRead)//if Executors can not read, sensors are updating
            {
                using (StreamWriter fs = new StreamWriter(sensorsUpdateRate, true))
                {
                    fs.Write(steps + ";" + Math.Round(current, 4).ToString("N", nfi) + ";" + Utility.SensorsManager.avgFps + ";" + FindObjectsOfType<MonoBehaviourSensor>().Length + "\n");
                    fs.Close();
                }
            }
            if (Plan.checkingPlan > 0)
            {
                using (StreamWriter fs = new StreamWriter(plansUpdateRate, true))
                {
                    fs.Write(steps + ";" + Math.Round(current, 4).ToString("N", nfi) + ";" + Utility.SensorsManager.avgFps + ";" + Plan.checkingPlan + ";" + Plan.totalAction + "\n");
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
