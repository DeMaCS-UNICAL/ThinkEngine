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
    public int currentIndex=-1;
    public List<string> testing = new List<string>();
    void Start()
    {
        currentIndex = GlobalIndexer.assignIndex();
    }
}

