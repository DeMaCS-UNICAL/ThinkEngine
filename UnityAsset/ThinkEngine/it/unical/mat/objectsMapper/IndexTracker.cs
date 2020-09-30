using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


class IndexTracker:MonoBehaviour
{
    public int currentIndex;

    void Awake()
    {
        currentIndex = GlobalIndexer.assignIndex();
    }
}

