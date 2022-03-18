using System;
using System.Linq;
using System.Reflection;
using JeanLF.AudioService.Filters;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine.UIElements;

namespace JeanLF.AudioService.Editor
{
    [CustomPropertyDrawer(typeof(AudioEntry))]
    public class AudioEntryPropertyDrawer : PropertyDrawer
    {
        private float SingleLineHeight => EditorGUIUtility.singleLineHeight;
        private float VerticalSpacing => EditorGUIUtility.standardVerticalSpacing;
        private float DefaultControlHeight => SingleLineHeight + VerticalSpacing;
        private ReorderableList _list;
        private SerializedProperty _targetProperty;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            EditorGUI.PropertyField(position, property, label, true);
            position.y += EditorGUI.GetPropertyHeight(property) + VerticalSpacing;

            if (_list == null)
            {
                _list = new ReorderableList(property.serializedObject,
                    property.FindPropertyRelative(AudioEntry.FilterPropertyName),
                    false,
                    true,
                    true,
                    true);
                _list.drawElementCallback = DrawElementCallback;
                _list.elementHeightCallback = ElementHeightCallback;
                _list.onAddDropdownCallback = OnAddDropdownCallback;
            }
        
            var listProperty = property.FindPropertyRelative(AudioEntry.FilterPropertyName);
            if (property.isExpanded)
            {
                using var scope = new EditorGUI.IndentLevelScope();
                listProperty.isExpanded =
                    EditorGUI.Foldout(GetPropertyRect(position), listProperty.isExpanded, "Filters");
                position.y += DefaultControlHeight;
                
                if (listProperty.isExpanded)
                {
                    _list.DoList(EditorGUI.IndentedRect(position));
                }

                scope.Dispose();
            }

            EditorGUI.EndProperty();
        }

        private float ElementHeightCallback(int index)
        {
            var prop = _list.serializedProperty.GetArrayElementAtIndex(index);
            float foldout = DefaultControlHeight;
            float controls = 0;
            if (prop.isExpanded)
            {
                var endProp = prop.GetEndProperty();
                while (prop.Next(true) && !SerializedProperty.EqualContents(prop, endProp))
                {
                    controls += EditorGUI.GetPropertyHeight(prop) + VerticalSpacing;
                }
            }
            
            return foldout + controls;
        }

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isfocused)
        {
            using var scope = new EditorGUI.IndentLevelScope();
            var prop = _list.serializedProperty.GetArrayElementAtIndex(index);
            prop.isExpanded = EditorGUI.Foldout(GetPropertyRect(rect), prop.isExpanded, prop.managedReferenceFullTypename);
            rect.y += DefaultControlHeight;
            if (prop.isExpanded)
            {
                var endProp = prop.GetEndProperty();
                while (prop.Next(true) && !SerializedProperty.EqualContents(prop, endProp))
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUI.PropertyField(GetPropertyRect(rect), prop);
                    }
                    rect.y += DefaultControlHeight;
                }
            }

            scope.Dispose();
        }
        
        private void OnAddDropdownCallback(Rect buttonrect, ReorderableList reorderable)
        {
            GenericMenu menu = new GenericMenu();

            var types = Assembly.GetAssembly(typeof(IFilterProperty)).GetTypes()
                .Where(x => x.GetInterface(nameof(IFilterProperty)) != null);

            foreach (var item in types)
            {
                menu.AddItem(new GUIContent(item.Name), false, onAddClick, item);
            }

            void onAddClick(object type)
            {
                object obj = Activator.CreateInstance((Type) type);
                var pos = reorderable.serializedProperty.arraySize;
                reorderable.serializedProperty.arraySize++;
                var property = reorderable.serializedProperty.GetArrayElementAtIndex(pos);
                property.managedReferenceValue = obj;
                reorderable.serializedProperty.serializedObject.ApplyModifiedProperties();
                reorderable.serializedProperty.serializedObject.Update();
            }
            
            menu.ShowAsContext();
        }
        

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float defaultSize = EditorGUI.GetPropertyHeight(property);
            var listProperty = property.FindPropertyRelative(AudioEntry.FilterPropertyName);
            float filterFoldout = DefaultControlHeight;

            if (_list == null) return defaultSize + filterFoldout;
            float list = listProperty.isExpanded ? _list.GetHeight() + VerticalSpacing : 0;
            if (!property.isExpanded)
            {
                filterFoldout = 0;
                list = 0;
            }

            return defaultSize + filterFoldout + list;
        }

        private Rect GetPropertyRect(Rect rect, float? height = null)
        {
            Rect propertyRect = new Rect(rect);
            propertyRect.height = height ?? EditorGUIUtility.singleLineHeight;
            return propertyRect;
        }
    }
}