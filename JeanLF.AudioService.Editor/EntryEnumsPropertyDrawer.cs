using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JeanLF.AudioService.Editor
{
    [CustomPropertyDrawer(typeof(EntryId))]
    public class EntryIdsPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);
            Rect remainingRect = EditorGUI.PrefixLabel(position, controlID, label);
            EditorGUI.HelpBox(remainingRect,"Use the Audio Reference struct to serialize Audio Services events", MessageType.Error);
        }
    }

    [CustomPropertyDrawer(typeof(GroupId))]
    public class GroupIdsPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);
            Rect remainingRect = EditorGUI.PrefixLabel(position, controlID, label);
            EditorGUI.HelpBox(remainingRect,"Use the Audio Reference struct to serialize Audio Services events", MessageType.Error);
        }
    }
}
