using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace JeanLF.AudioService.Editor
{
    internal static class AudioServiceSettingsEditor
    {
        private static Button _editButton;
        private static SerializedObject _settings;
        private static ObjectField _configField;

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            SettingsProvider provider = new SettingsProvider("Project/JeanLF/Audio Service", SettingsScope.Project);
            provider.activateHandler = DrawSettings;
            provider.keywords = new HashSet<string>()
            {
                "Audio Configuration Asset",
                "Pool Size",
                "Filtered Sources"
            };

            return provider;
        }

        internal static AudioServiceSettings GetOrCreateSettings()
        {
            AudioServiceSettings settings = AssetDatabase.LoadAssetAtPath<AudioServiceSettings>(AudioServiceEditorUtils.SettingsAssetPath);

            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<AudioServiceSettings>();

                Directory.CreateDirectory(Path.GetDirectoryName(AudioServiceEditorUtils.SettingsAssetPath));
                AssetDatabase.CreateAsset(settings,  AudioServiceEditorUtils.SettingsAssetPath);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }

        private static void DrawSettings(string searchContext, VisualElement rootElement)
        {
            _settings = new SerializedObject(GetOrCreateSettings());
            SerializedProperty configProp = _settings.FindProperty(AudioServiceSettings.ConfigMemberPath);

            VisualTreeAsset visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{AudioServiceEditorUtils.EditorUIPath}/AudioServiceSettings.uxml");
            VisualElement treeAsset = visualTree.CloneTree();

            rootElement.Add(treeAsset);

            rootElement.Q<Button>("createButton").clicked += OnAddClick;

            _editButton = rootElement.Q<Button>("editButton");
            _editButton.clicked += OnEditClick;
            _editButton.SetEnabled(configProp.objectReferenceValue);

            _configField = rootElement.Q<ObjectField>("configObject");
            _configField.BindProperty(configProp);
            _configField.RegisterValueChangedCallback(OnConfigChange);

            PropertyField poolField = rootElement.Q<PropertyField>("poolSize");
            poolField.BindProperty(_settings.FindProperty(AudioServiceSettings.PoolSizeMemberPath));

            PropertyField filterField = rootElement.Q<PropertyField>("filtersPoolSize");
            filterField.BindProperty(_settings.FindProperty(AudioServiceSettings.FilterCountMemberPath));
        }

        private static void OnConfigChange(ChangeEvent<UnityEngine.Object> evt)
        {
            _editButton.SetEnabled(evt.newValue);
        }

        private static void OnEditClick()
        {
            AudioServiceWindow.OpenConfigurationWindow((AudioConfig)_configField.value);
        }

        private static void OnAddClick()
        {
            const string defaultName = "AudioConfiguration";
            const string panelTitle = "Create audio configuration asset";
            const string message = "Please select a folder";
            const string extension = "asset";

            string path = EditorUtility.SaveFilePanelInProject(panelTitle, defaultName, extension, message);
            AudioConfig config = ScriptableObject.CreateInstance<AudioConfig>();
            AssetDatabase.CreateAsset(config, path);

            _configField.value = config;
        }
    }
}
