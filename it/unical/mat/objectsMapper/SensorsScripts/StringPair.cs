using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.SensorsScripts
{
    [Serializable]
    class StringIntPair
    {
        [SerializeField]
        public string Key;
        [SerializeField]
        public int Value;
    }
}
