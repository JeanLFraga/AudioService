using System;
using Codice.Client.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace JeanLF.AudioService.Editor
{
    public class ReorderableArray : BindableElement
    {
        public event Action OnUpdate;
        
        private SerializedProperty _arrayProperty;
        private ReorderableList _reorderable;
        private IMGUIContainer _container;
        private GUIContent _listName;
        
        public new class UxmlFactory : UxmlFactory<ReorderableArray, BindableElement.UxmlTraits> { }

        public ReorderableArray(
            SerializedProperty arrayProperty,
            bool draggable = true,
            bool displayHeader = true,
            bool displayAddButton = true,
            bool displayRemoveButton = true)
        {
            _arrayProperty = arrayProperty;
            CreateReorderableList(draggable, displayHeader, displayAddButton, displayRemoveButton);
            _listName = new GUIContent(_arrayProperty.displayName);
            _container = new IMGUIContainer(DrawList);
            Add(_container);
        }

        public ReorderableArray()
        {
            _container = new IMGUIContainer();
            Add(_container);
        }

        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultAction(evt);
            Type type = evt.GetType();
            if (type.Name != "SerializedPropertyBindEvent")
            {
                return;
            }

            _arrayProperty = type.GetProperty("bindProperty").GetValue(evt) as SerializedProperty ;
            if (_reorderable != null)
            {
                _reorderable.serializedProperty = _arrayProperty;
            }
            else
            {
                CreateReorderableList(true,true,true,true);
                _container.onGUIHandler = DrawList;
                _listName = new GUIContent(_arrayProperty.displayName);
            }
        }

        private void CreateReorderableList(bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton)
        {
            _reorderable = new ReorderableList(_arrayProperty.serializedObject, _arrayProperty, draggable, displayHeader, displayAddButton, displayRemoveButton);
            _reorderable.drawHeaderCallback = DrawHeaderCallback;
            _reorderable.drawElementCallback = DrawElementCallback;
            _reorderable.elementHeightCallback = ElementHeightCallback;
            _reorderable.onChangedCallback = OnChangedCallback;
        }

        private void DrawHeaderCallback(Rect rect1)
        {
            EditorGUI.LabelField(rect1, _listName);
        }

        private void OnChangedCallback(ReorderableList list)
        {
            OnUpdate?.Invoke();
        }

        private float ElementHeightCallback(int index)
        {
            return EditorGUI.GetPropertyHeight(_arrayProperty.GetArrayElementAtIndex(index));
        }

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.height = EditorGUI.GetPropertyHeight(_arrayProperty.GetArrayElementAtIndex(index));
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(rect, _arrayProperty.GetArrayElementAtIndex(index));
            if (EditorGUI.EndChangeCheck())
            {
                _arrayProperty.serializedObject.ApplyModifiedProperties();
                OnUpdate?.Invoke();
            }
        }

        private void DrawList()
        {
            _reorderable.DoLayoutList();
        }

        
    }
}