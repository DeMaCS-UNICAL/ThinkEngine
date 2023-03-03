using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkEngine.Editors;
using ThinkEngine.it.unical.mat.objectsMapper.BrainsScripts.DCS;
using UnityEditor;
using UnityEngine;

namespace ThinkEngine.it.unical.mat.objectsMapper.Editors
{
    [CustomEditor(typeof(DCSBrain), true)]
    internal class DCSBrainEditor : BrainEditor
    {
        void OnInspectorGUI()
        {
            base.OnInspectorGUI();

        }

        override protected void ListAvailableConfigurations()
        {
            base.ListAvailableConfigurations();
            DCSBrain brain = ((DCSBrain)target);
            if (brain.initAIFile == null)
            {
                EditorGUILayout.HelpBox("You don't have an init file for your Declarative Content Specification AI. You can add one by clicking on the button.", MessageType.Info);
            }
            else
            {
                using (new EditorGUI.DisabledScope()) {
                    EditorGUILayout.TextField("Init Ai File path",brain.initAIFile);
                }
            }
            if (GUILayout.Button("Choose Init File", GUILayout.Width(150)))
            {
                string temp = EditorUtility.OpenFilePanel("Choose Init AI File", Utility.StreamingAssetsContent, ((DCSBrain)target).FileExtension);
                if (temp.StartsWith(Application.dataPath))
                {
                    brain.initAIFile = "Assets" + temp.Substring(Application.dataPath.Length);
                }
            }
        }
    }
}
