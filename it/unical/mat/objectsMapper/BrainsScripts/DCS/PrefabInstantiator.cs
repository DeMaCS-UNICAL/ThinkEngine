using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts.DCS
{
    internal class PrefabInstantiator
    {
        internal GameObject toInstantiate;
        internal float x, y, z;


        internal void Instantiate()
        {
            GameObject.Instantiate(toInstantiate,new Vector3(x,y,z), new Quaternion());
        }
    }
}
