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
    public List<MonoBehaviour> testing2 = new List<MonoBehaviour>();
    public string[,] testing3 = new string[2, 2];
    void Start()
    {
        currentIndex = GlobalIndexer.assignIndex();
    }
    void Update()
    {
        if (testing.Count < 5)
        {
            testing.Add("ciao" + testing.Count);
            testing3[testing.Count % 2, testing.Count % 2] = (new System.Random()).Next() + "";
        }
    }
}

