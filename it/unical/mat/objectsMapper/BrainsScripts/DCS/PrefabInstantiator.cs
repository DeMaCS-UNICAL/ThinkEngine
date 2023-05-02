using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts.DCS
{
    [DisallowMultipleComponent]
    internal class PrefabInstantiator : MonoBehaviour
    {
        internal List<int> toInstantiate;
        internal List<Vector3> instantiationPositions;
        internal List<Quaternion> instantiationRotations;
        internal List<Vector3> circleToInstantiatePositions;
        internal List<string> circleToInstantiate;


        void OnEnable()
        {
            toInstantiate = new List<int>();
            instantiationPositions = new List<Vector3>();
            instantiationRotations = new List<Quaternion>();
            circleToInstantiatePositions = new List<Vector3>();
            circleToInstantiate = new List<string>();
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
                    //Debug.Log(ContentPrefabConfigurator.GetPrefab(toInstantiate[i]).gameObject.name + " in " + instantiationPositions[i]);

                    Instantiate(ContentPrefabConfigurator.GetPrefab(toInstantiate[i]), instantiationPositions[i], instantiationRotations[i]);
                }
                toInstantiate.Clear();
                instantiationPositions.Clear();
                instantiationRotations.Clear();
                for(int i = 0; i < circleToInstantiatePositions.Count; i++)
                {
                    UnityEngine.Object g = Resources.Load(circleToInstantiate[i]);
                    if (g != null)
                    {
                        Instantiate(g, circleToInstantiatePositions[i], new Quaternion(0, 0, 0, 0));
                    }
                    else
                    {
                        Debug.LogError("Prefab " + circleToInstantiate[i] + " does not exist!");
                    }
                }
                circleToInstantiatePositions.Clear();
                circleToInstantiate.Clear();
            }
        }

        internal void InstantiateCircle(Vector3 vector3, string s)
        {
            circleToInstantiatePositions.Add(vector3);
            circleToInstantiate.Add(s);
        }
    }
}
