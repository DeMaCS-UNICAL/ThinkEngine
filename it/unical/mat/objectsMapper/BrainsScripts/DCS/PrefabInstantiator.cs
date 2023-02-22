using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts.DCS
{
    [DisallowMultipleComponent]
    internal class PrefabInstantiator : MonoBehaviour
    {
        internal Dictionary<DCSBrain, List<int>> toInstantiate;
        internal Dictionary<DCSBrain, List<Vector3>> instantiationPositions;
        internal Dictionary<DCSBrain, List<Quaternion>> instantiationRotations;


        void OnEnable()
        {
            toInstantiate = new Dictionary<DCSBrain, List<int>>();
            instantiationPositions = new Dictionary<DCSBrain, List<Vector3>>();
            instantiationRotations = new Dictionary<DCSBrain, List<Quaternion>>();
        }
        internal void InstantiatePrefab(DCSBrain brain, int index, Vector3 position, Quaternion rotation)
        {
            if (!toInstantiate.ContainsKey(brain))
            {
                toInstantiate[brain]=new List<int>();
                instantiationPositions[brain]=new List<Vector3>();
                instantiationRotations[brain]=new List<Quaternion>();
            }
            toInstantiate[brain].Add(index);
            instantiationPositions[brain].Add(position);
            instantiationRotations[brain].Add(rotation);
        }

        void Update()
        {
            foreach(DCSBrain brain in toInstantiate.Keys)
            {
                for(int i =0; i<toInstantiate[brain].Count; i++)
                {
                    Instantiate(brain.instantiablePrefabs[toInstantiate[brain][i]], instantiationPositions[brain][i], instantiationRotations[brain][i]);
                }
            }
            toInstantiate.Clear();
        }
    }
}
