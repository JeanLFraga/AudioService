using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

namespace JeanLF.AudioService.Editor
{
    // ReSharper disable once AccessToStaticMemberViaDerivedType
    [CustomPropertyDrawer(typeof(AudioReference))]
    public class AudioReferencePropertyDrawer : PropertyDrawer
    {
        private class ListAdvancedDropdown : AdvancedDropdown
        {
            private event Action<string> onSelection;

            private readonly IReadOnlyList<string> _sourceItems;
            private readonly string _rootLabel;

            private string currentlySelected;

            public event Action<string> OnSelection
            {
                add
                {
                    onSelection += value;
                }
                remove
                {
                    onSelection -= value;
                }
            }

            public ListAdvancedDropdown(AdvancedDropdownState state)
                : base(state) { }

            public ListAdvancedDropdown(AdvancedDropdownState state, string rootLabel, IReadOnlyList<string> items, string selected = null)
                : base(state)
            {
                _sourceItems = items ?? throw new ArgumentNullException("Items cannot be null");
                _rootLabel = rootLabel;
                currentlySelected = selected;
            }

            protected override AdvancedDropdownItem BuildRoot()
            {
                AdvancedDropdownItem root = new AdvancedDropdownItem(_rootLabel);

                for (int i = 0; i < _sourceItems.Count; i++)
                {
                    AdvancedDropdownItem item = new AdvancedDropdownItem(_sourceItems[i])
                    {
                        enabled = _sourceItems[i] != currentlySelected,
                        icon = _sourceItems[i] == currentlySelected ? (Texture2D)EditorGUIUtility.IconContent("TestPassed").image : null
                    };

                    root.AddChild(item);
                }

                return root;
            }

            protected override void ItemSelected(AdvancedDropdownItem item)
            {
                base.ItemSelected(item);
                onSelection?.Invoke(item.name);
            }
        }

        private readonly GUIStyle _labelStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter
        };
        private AudioServiceSettings _settings;

        private AudioServiceSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = AssetDatabase.LoadAssetAtPath<AudioServiceSettings>(AudioService.SettingsAssetPath);
                }

                return _settings;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);

            int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);

            Rect controlRect = EditorGUI.PrefixLabel(position, controlID, label);

            if (Settings != null && !Settings.HasConfiguration)
            {
                EditorGUI.HelpBox(controlRect, "A configuration asset must be set in the Project Settings - Audio Service Settings", MessageType.Error);
                return;
            }

            const int middleLabelWidth = 30;
            Rect labelRect = new Rect(controlRect)
            {
                width = middleLabelWidth,
                x = controlRect.center.x - middleLabelWidth / 2f,
            };

            Rect groupRect = new Rect(controlRect)
            {
                width = controlRect.width / 2 - middleLabelWidth / 2f
            };

            Rect entryRect = new Rect(controlRect)
            {
                width = controlRect.width / 2 -  middleLabelWidth / 2f,
                x = labelRect.xMax
            };


            EditorGUI.LabelField(labelRect, "/", _labelStyle);

            SerializedProperty groupStringProperty = property.FindPropertyRelative(AudioReference.GroupStringPath);
            SerializedProperty groupEnumProperty = property.FindPropertyRelative(AudioReference.GroupEnumPath);

            SerializedProperty entryStringProperty = property.FindPropertyRelative(AudioReference.EntryStringPath);
            SerializedProperty entryEnumProperty = property.FindPropertyRelative(AudioReference.EntryEnumPath);

            DrawIdDropdown(groupRect, groupStringProperty, groupEnumProperty, GetGroupsIds());
            DrawIdDropdown(entryRect, entryStringProperty, entryEnumProperty, GetEntriesIds());

            EditorGUI.EndProperty();
        }

        private IReadOnlyList<string> GetGroupsIds()
        {
            return Settings.Database.AudioGroups.Select(x => x.Id).ToList();
        }

        private IReadOnlyList<string> GetEntriesIds()
        {
            return Settings.Database.AudioEntries.Select(x => x.Id).ToList();
        }

        private void DrawIdDropdown(Rect groupRect, SerializedProperty stringProperty, SerializedProperty enumProperty, IReadOnlyList<string> items)
        {
            string displayName = enumProperty.enumValueIndex == 0 ? " Invalid" : ObjectNames.NicifyVariableName(stringProperty.stringValue);
            Texture icon = enumProperty.enumValueIndex == 0 ? EditorGUIUtility.IconContent("CollabError").image : null;
            if (EditorGUI.DropdownButton(groupRect, new GUIContent(displayName, image: icon), FocusType.Keyboard))
            {
                ListAdvancedDropdown dropdown = new ListAdvancedDropdown(new AdvancedDropdownState(), stringProperty.displayName, items, stringProperty.stringValue);
                dropdown.Show(groupRect);
                dropdown.OnSelection += delegate(string selected)
                {
                    stringProperty.stringValue = selected;
                    stringProperty.serializedObject.ApplyModifiedProperties();
                };
            }
        }
    }
}
