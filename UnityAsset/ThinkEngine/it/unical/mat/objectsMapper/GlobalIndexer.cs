using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class GlobalIndexer
{
    private static object toLock = new object();
    static internal int maxIndexUsed;

    internal static int assignIndex()
    {
        lock (toLock)
        {
            return ++maxIndexUsed;
        }
    }
    internal static GameObject find(int index)
    {
        foreach (IndexTracker tracker in GameObject.FindObjectsOfType<IndexTracker>())
        {
            if (tracker.currentIndex == index)
            {
                return tracker.gameObject;
            }
        }
        return null;
    }
}
