using System;
using UnityEditor;
using UnityEngine;

namespace JeanLF.AudioService.Editor
{
    [CustomEditor(typeof(AudioConfig))]
    public class AudioConfigEditor : UnityEditor.Editor
    {
        
        private SerializedProperty _mixerProp;

        private void OnEnable()
        {
            
        }

        public override void OnInspectorGUI()
        {
            using (new EditorGUI.IndentLevelScope(-1))
            {
                EditorGUILayout.LabelField("Test");
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("More Test");
            }
        }
    }
}