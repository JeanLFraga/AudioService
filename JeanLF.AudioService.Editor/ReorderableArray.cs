using System;
using Codice.Client.Common;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace JeanLF.AudioService.Editor
{
    public class ReorderableArray : BindableElement
    {
        public delegate bool SearchDelegate(SerializedProperty property);

        public event ReorderableList.AddDropdownCallbackDelegate AddDropdownCallback;

        public event Action OnDataUpdate;

        public event Action OnItemAdd;

        private SerializedProperty _arrayProperty;
        private bool _draggable = true;
        private bool _displayHeader = true;
        private bool _displayAddButton = true;
        private bool _displayRemoveButton = true;
        private ReorderableList _reorderable;
        private IMGUIContainer _container;
        private GUIContent _listName;
        private SearchDelegate _matchingFunction;

        private bool _shouldDraw = true;

        public new class UxmlFactory : UxmlFactory<ReorderableArray, UxmlTraits> { }

        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            private UxmlBoolAttributeDescription _draggable = new UxmlBoolAttributeDescription
            {
                name = "draggable",
                defaultValue = true
            };
            private UxmlBoolAttributeDescription _displayHeader = new UxmlBoolAttributeDescription
            {
                name = "header",
                defaultValue = true
            };
            private UxmlBoolAttributeDescription _displayAddButton = new UxmlBoolAttributeDescription
            {
                name = "add-button",
                defaultValue = true
            };
            private UxmlBoolAttributeDescription _displayRemoveButton = new UxmlBoolAttributeDescription
            {
                name = "removebutton",
                defaultValue = true
            };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ((ReorderableArray)ve)._draggable = _draggable.GetValueFromBag(bag, cc);
                ((ReorderableArray)ve)._displayHeader = _displayHeader.GetValueFromBag(bag, cc);
                ((ReorderableArray)ve)._displayAddButton = _displayAddButton.GetValueFromBag(bag, cc);
                ((ReorderableArray)ve)._displayRemoveButton = _displayRemoveButton.GetValueFromBag(bag, cc);
            }
        }

        public ReorderableArray(SerializedProperty arrayProperty,
                                bool draggable = true,
                                bool displayHeader = true,
                                bool displayAddButton = true,
                                bool displayRemoveButton = true)
        {
            _arrayProperty = arrayProperty;
            _draggable = draggable;
            _displayHeader = displayHeader;
            _displayAddButton = displayAddButton;
            _displayRemoveButton = displayRemoveButton;
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

            _arrayProperty = type.GetProperty("bindProperty").GetValue(evt) as SerializedProperty;

            if (_arrayProperty == null)
            {
                _shouldDraw = false;

                return;
            }

            if (_reorderable != null)
            {
                _reorderable.serializedProperty = _arrayProperty;
            }
            else
            {
                CreateReorderableList(_draggable, _displayHeader, _displayAddButton, _displayRemoveButton);
                _container.onGUIHandler = DrawList;
                _listName = new GUIContent(_arrayProperty.displayName);
            }

            _shouldDraw = true;
        }

        private void CreateReorderableList(bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton)
        {
            _reorderable = new ReorderableList(_arrayProperty.serializedObject, _arrayProperty, draggable, displayHeader, displayAddButton, displayRemoveButton);
            _reorderable.drawHeaderCallback = DrawHeaderCallback;
            _reorderable.drawElementCallback = DrawElementCallback;
            _reorderable.elementHeightCallback = ElementHeightCallback;
            _reorderable.onChangedCallback = OnChangedCallback;
            _reorderable.onAddDropdownCallback = AddDropdownCallback;
            _reorderable.onAddCallback = OnAddCallback;
        }

        private void OnAddCallback(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoAddButton(list);
            OnItemAdd?.Invoke();
        }

        private void DrawHeaderCallback(Rect rect1)
        {
            EditorGUI.LabelField(rect1, _listName);
        }

        private void OnChangedCallback(ReorderableList list)
        {
            OnDataUpdate?.Invoke();
        }

        private float ElementHeightCallback(int index)
        {
            return EditorGUI.GetPropertyHeight(_arrayProperty.GetArrayElementAtIndex(index));
        }

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty indexProp = _arrayProperty.GetArrayElementAtIndex(index);

            rect.height = EditorGUI.GetPropertyHeight(indexProp);
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(rect, indexProp, true);

            if (EditorGUI.EndChangeCheck())
            {
                _arrayProperty.serializedObject.ApplyModifiedProperties();
                OnDataUpdate?.Invoke();
            }
        }

        private void DrawList()
        {
            if (!_shouldDraw)
            {
                return;
            }

            _reorderable.DoLayoutList();
        }
    }
}
