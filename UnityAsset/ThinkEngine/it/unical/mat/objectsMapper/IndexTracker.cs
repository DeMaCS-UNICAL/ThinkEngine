using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
class IndexTracker:MonoBehaviour
{
    public int currentIndex;

    void OnEnable()
    {
        currentIndex = GlobalIndexer.assignIndex();
    }
}

