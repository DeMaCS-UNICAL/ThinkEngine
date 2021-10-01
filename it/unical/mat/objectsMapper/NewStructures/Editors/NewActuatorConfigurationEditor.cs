using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace NewStructures.Editors
{
    [CustomEditor(typeof(NewActuatorConfiguration))]
    class NewActuatorConfigurationEditor : NewAbstractConfigurationEditor
    {
        protected override void SpecificFields(MyListString property)
        {
            
        }

        void OnDestroy()
        {
            if (target == null && go != null)
            {
                DestroyImmediate(go.GetComponent<MonoBehaviourActuatorsManager>());
            }
        }
    }
}
