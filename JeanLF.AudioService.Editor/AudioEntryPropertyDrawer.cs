using System;
using System.Collections.Generic;
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
        private readonly List<Type> _typeList = new List<Type>();
        private Dictionary<string, ReorderableList> _reorderableLists = new Dictionary<string, ReorderableList>();

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

            if (!_reorderableLists.TryGetValue(property.propertyPath, out _list))
            {
                _list = new ReorderableList(property.serializedObject,
                                            property.FindPropertyRelative(AudioEntry.FilterPropertyName));

                _list.drawElementCallback = DrawElementCallback;
                _list.elementHeightCallback = ElementHeightCallback;
                _list.onAddDropdownCallback = OnAddDropdownCallback;
                _reorderableLists.Add(property.propertyPath, _list);
            }

            var listProperty = property.FindPropertyRelative(AudioEntry.FilterPropertyName);

            if (property.isExpanded)
            {
                using var scope = new EditorGUI.IndentLevelScope();
                EditorGUI.BeginChangeCheck();
                listProperty.isExpanded =
                    EditorGUI.Foldout(GetPropertyRect(position), listProperty.isExpanded, "Filters");

                if (EditorGUI.EndChangeCheck())
                {
                    _list.GetHeight();
                }

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
            _list.serializedProperty.serializedObject.Update();
            SerializedProperty prop = _list.serializedProperty.GetArrayElementAtIndex(index);
            float foldout = DefaultControlHeight;
            float controls = 0;

            if (prop.isExpanded)
            {
                SerializedProperty endProp = prop.GetEndProperty(true);

                while (prop.Next(true) && !SerializedProperty.EqualContents(prop, endProp))
                {
                    controls += EditorGUI.GetPropertyHeight(prop) + VerticalSpacing;
                }
            }

            return foldout + controls;
        }

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isfocused)
        {
            EditorGUIUtility.GetControlID(FocusType.Passive, rect);
            using EditorGUI.IndentLevelScope scope = new EditorGUI.IndentLevelScope();
            SerializedProperty prop = _list.serializedProperty.GetArrayElementAtIndex(index);
            string filterName = prop.managedReferenceFullTypename;
            filterName = filterName.Substring(filterName.LastIndexOf('.') + 1);
            EditorGUI.BeginChangeCheck();
            prop.isExpanded =
                EditorGUI.Foldout(GetPropertyRect(rect), prop.isExpanded, ObjectNames.NicifyVariableName(filterName));

            if (EditorGUI.EndChangeCheck())
            {
                _list.GetHeight();
            }

            rect.y += DefaultControlHeight;

            if (prop.isExpanded)
            {
                SerializedProperty endProp = prop.GetEndProperty(true);

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

            _typeList.Clear();
            TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<IFilterProperty>();

            for (int i = 0; i < reorderable.serializedProperty.arraySize; i++)
            {
                string fullName = reorderable.serializedProperty.GetArrayElementAtIndex(i).managedReferenceFullTypename;
                Assembly assembly = Assembly.Load(new AssemblyName(fullName.Remove(fullName.IndexOf(' '))));
                Type type = assembly.GetType(fullName.Substring(fullName.IndexOf(' ')));
                _typeList.Add(type);
            }

            foreach (Type item in types)
            {
                if (!_typeList.Contains(item))
                {
                    menu.AddItem(new GUIContent(item.Name), false, onAddClick, item);
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(item.Name), true);
                }
            }

            void onAddClick(object type)
            {
                object obj = Activator.CreateInstance((Type)type);
                int pos = reorderable.serializedProperty.arraySize;
                reorderable.serializedProperty.arraySize++;
                SerializedProperty property = reorderable.serializedProperty.GetArrayElementAtIndex(pos);
                property.managedReferenceValue = obj;
                reorderable.serializedProperty.serializedObject.ApplyModifiedProperties();
                reorderable.serializedProperty.serializedObject.Update();

                GetPropertyHeight(reorderable.serializedProperty, GUIContent.none);
            }

            menu.ShowAsContext();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float defaultSize = EditorGUI.GetPropertyHeight(property);
            SerializedProperty listProperty = property.FindPropertyRelative(AudioEntry.FilterPropertyName);
            float filterFoldout = DefaultControlHeight;
            _reorderableLists.TryGetValue(property.propertyPath, out ReorderableList value);

            if (value == null)
            {
                return defaultSize + filterFoldout;
            }

            float list = listProperty.isExpanded ? value.GetHeight() + VerticalSpacing : 0;

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
