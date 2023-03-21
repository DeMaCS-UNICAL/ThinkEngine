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
    [CustomEditor(typeof(ContentBrain), true)]
    internal class ContentBrainEditor : BrainEditor
    {
        void OnInspectorGUI()
        {
            base.OnInspectorGUI();

        }

        override protected void ListAvailableConfigurations()
        {
            base.ListAvailableConfigurations();
            ContentBrain brain = ((ContentBrain)target);
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
                string temp = EditorUtility.OpenFilePanel("Choose Init AI File", Utility.StreamingAssetsContent, ((ContentBrain)target).FileExtension);
                if (temp.StartsWith(Application.dataPath))
                {
                    brain.initAIFile = "Assets" + temp.Substring(Application.dataPath.Length);
                }
            }
            if (brain.custom_bk_file_path == null)
            {
                EditorGUILayout.HelpBox("You don't have a custom background knowledge file for your Declarative Content Specification AI. You can add one by clicking on the button.", MessageType.Info);
            }
            else
            {
                using (new EditorGUI.DisabledScope())
                {
                    EditorGUILayout.TextField("Custom Background Knowledge File path", brain.custom_bk_file_path);
                }
            }
            if (GUILayout.Button("Choose BK File", GUILayout.Width(150)))
            {
                string temp = EditorUtility.OpenFilePanel("Choose Background Knowledge File", Utility.StreamingAssetsContent, ((ContentBrain)target).FileExtension);
                if (temp.StartsWith(Application.dataPath))
                {
                    brain.custom_bk_file_path = "Assets" + temp.Substring(Application.dataPath.Length);
                }
            }
        }
    }
}
