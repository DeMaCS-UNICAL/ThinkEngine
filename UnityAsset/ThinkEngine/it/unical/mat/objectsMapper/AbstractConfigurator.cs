using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper
{

    public class AbstractConfigurator: MonoBehaviour
    {
         

        public virtual AbstractConfiguration newConfiguration(string n)
        {
            throw new NotImplementedException();
        }
    }
}
