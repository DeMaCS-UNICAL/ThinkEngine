using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

 internal static class MyDebugger
{
    internal static bool enabled;

    internal static void MyDebug(string message)
    {
        if (enabled)
        {
            Debug.Log(message);
        }
    }
}