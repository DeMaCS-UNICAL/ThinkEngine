using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.ActuatorsScripts
{
    [Serializable]
    public class ActuatorsManager : ScriptableObject,IManager
    {
        [SerializeField]
        private List<AbstractConfiguration> brainsConfs;
        [SerializeField]
        private List<string> ConfiguredGameObject;

        public ref List<AbstractConfiguration> confs()
        {
            return ref brainsConfs;
        }

        public ref List<string> configuredGameObject()
        {
            return ref ConfiguredGameObject;
        }

        void OnEnable()
        {

            if (brainsConfs == null)
            {
                brainsConfs = new List<AbstractConfiguration>();
            }
            if (ConfiguredGameObject == null)
            {
                ConfiguredGameObject = new List<string>();
            }
        }

        public void OnBeforeSerialize()
        {
            throw new NotImplementedException();
        }

        public void OnAfterDeserialize()
        {
            throw new NotImplementedException();
        }
    }
}
