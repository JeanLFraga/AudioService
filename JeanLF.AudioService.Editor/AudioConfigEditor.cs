using System;
using UnityEditor;
using UnityEngine;

namespace JeanLF.AudioService.Editor
{
    [CustomEditor(typeof(AudioConfig))]
    public sealed class AudioConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Please, use the Audio Service edit window", MessageType.Info);

            if (GUILayout.Button("Edit"))
            {
                AudioServiceWindow.OpenConfigurationWindow((AudioConfig)target);
            }
        }
    }
}
