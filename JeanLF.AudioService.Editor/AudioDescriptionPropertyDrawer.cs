using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace JeanLF.AudioService.Editor
{
    [CustomPropertyDrawer(typeof(AudioDescription))]
    public class AudioDescriptionPropertyDrawer : PropertyDrawer
    {
        private GUIStyle _separatorStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter
        };

        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float propSize = EditorGUIUtility.singleLineHeight * 3f + EditorGUIUtility.standardVerticalSpacing;
            float standardVerticalSpacing = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            return property.isExpanded ? propSize : standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            property.isExpanded = EditorGUI.Foldout(NextPropertyRect(ref position),property.isExpanded,label, true);

            if (property.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUI.BeginChangeCheck();
                    SerializedProperty tempoProperty = property.FindPropertyRelative(AudioDescription.TempoPath);
                    EditorGUI.DelayedIntField(NextPropertyRect(ref position), tempoProperty);

                    SerializedProperty signatureProperty = property.FindPropertyRelative(AudioDescription.SignaturePath);

                    int controlId = "EditorTextField".GetHashCode();
                    Rect vectorRect = EditorGUI.PrefixLabel(NextPropertyRect(ref position), controlId,new GUIContent("Time Signature"));

                    DrawTimeSignatureFields(vectorRect, signatureProperty);

                    if (EditorGUI.EndChangeCheck())
                    {
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            }
            EditorGUI.EndProperty();
        }

        private void DrawTimeSignatureFields(Rect rect, SerializedProperty signatureProperty)
        {
            const float separatorWidth = 35;
            const float halfSeparatorWidth = separatorWidth / 2;

            Rect control1 = new Rect(rect)
            {
                width = rect.width / 2 - halfSeparatorWidth
            };
            Rect control2 = new Rect(rect)
            {
                x = rect.x + rect.width / 2 + halfSeparatorWidth,
                width = rect.width / 2 - halfSeparatorWidth
            };
            Rect separator = new Rect(rect)
            {
                x = rect.x + rect.width / 2 - halfSeparatorWidth,
                width = separatorWidth
            };

            using (new EditorGUI.IndentLevelScope(-2)) // The controls use the indent internally
            {
                Vector2Int signature = Vector2Int.zero;
                signature.x = EditorGUI.IntField(control1, signatureProperty.vector2IntValue.x);
                signature.y = EditorGUI.IntField(control2, signatureProperty.vector2IntValue.y);
                EditorGUI.LabelField(separator, "/", _separatorStyle);
                signatureProperty.vector2IntValue = signature;
            }
        }

        private Rect NextPropertyRect(ref Rect rect, float? height = null)
        {
            Rect propertyRect = new Rect(rect);
            propertyRect.height = height ?? EditorGUIUtility.singleLineHeight;
            rect.y += propertyRect.height;

            return propertyRect;
        }
    }
}
