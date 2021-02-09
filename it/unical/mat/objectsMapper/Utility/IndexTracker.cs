using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
class IndexTracker:MonoBehaviour
{
    public int currentIndex=-1;
    void Start()
    {
        currentIndex = GlobalIndexer.assignIndex();
    }
}

