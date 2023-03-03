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
        internal List<int> toInstantiate;
        internal List<Vector3> instantiationPositions;
        internal List<Quaternion> instantiationRotations;


        void OnEnable()
        {
            toInstantiate = new List<int>();
            instantiationPositions = new List<Vector3>();
            instantiationRotations = new List<Quaternion>();
        }
        internal void InstantiatePrefab(int index, Vector3 position, Quaternion rotation)
        {
            lock (this)
            {
                toInstantiate.Add(index);
                instantiationPositions.Add(position);
                instantiationRotations.Add(rotation);
            }
        }

        void Update()
        {
            lock (this)
            {
                for (int i = 0; i < toInstantiate.Count; i++)
                {
                    Instantiate(DCSPrefabConfigurator.instances[toInstantiate[i]], instantiationPositions[i], instantiationRotations[i]);
                }
                toInstantiate.Clear();
            }
        }
    }
}
