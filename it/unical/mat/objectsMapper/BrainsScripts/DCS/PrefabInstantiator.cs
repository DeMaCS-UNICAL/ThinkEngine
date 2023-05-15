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
    public class PrefabInstantiator : MonoBehaviour
    {
        internal List<int> toInstantiate;
        internal List<Vector3> instantiationPositions;
        internal List<Quaternion> instantiationRotations;
        internal List<Vector3> instantiateByNamePositions;
        internal List<string> instantiateByName;


        void OnEnable()
        {
            toInstantiate = new List<int>();
            instantiationPositions = new List<Vector3>();
            instantiationRotations = new List<Quaternion>();
            instantiateByNamePositions = new List<Vector3>();
            instantiateByName = new List<string>();
        }
        public void InstantiatePrefab(int index, Vector3 position, Quaternion rotation)
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
                for(int i = 0; i < instantiateByNamePositions.Count; i++)
                {
                    UnityEngine.Object g = Resources.Load(instantiateByName[i]);
                    if (g != null)
                    {
                        Instantiate(g, instantiateByNamePositions[i], new Quaternion(0, 0, 0, 0));
                    }
                    else
                    {
                        Debug.LogError("Prefab " + instantiateByName[i] + " does not exist!");
                    }
                }
                instantiateByNamePositions.Clear();
                instantiateByName.Clear();
            }
        }

        public void InstantiateByName(Vector3 vector3, string s)
        {
            instantiateByNamePositions.Add(vector3);
            instantiateByName.Add(s);
        }
    }
}
