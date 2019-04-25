using EmbASP4Unity.it.unical.mat.objectsMapper.BrainsScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EmbASP4Unity.it.unical.mat.objectsMapper.EditorWindows
{
    [CustomEditor(typeof(Brain))]
    public class BrainEditor:Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Brain current = (Brain)target;
            if(GUILayout.Button("Generate ASP file", GUILayout.Width(300)))
            {
                current.generateFile();
            }
        }
    }
}
