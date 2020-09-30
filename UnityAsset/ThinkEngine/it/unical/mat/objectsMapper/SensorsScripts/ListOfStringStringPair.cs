using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts
{
    [Serializable]
    class ListOfStringStringPair
    {
        [SerializeField]
        public List<string> Key;
        [SerializeField]
        public string Value;
    }
}
