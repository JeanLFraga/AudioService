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
            DrawPropertiesExcluding(serializedObject, "m_Script");
        }
    }
}
