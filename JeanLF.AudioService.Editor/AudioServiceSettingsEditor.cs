using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace JeanLF.AudioService.Editor
{
    [InitializeOnLoad]
    internal static class AudioServiceSettingsEditor
    {
        private static Button _editButton;
        private static SerializedObject _settings;
        private static ObjectField _configField;

        static AudioServiceSettingsEditor()
        {
            AudioServiceSettings settings = GetOrCreateSettings();

            if (settings.Database == null)
            {
                return;
            }

            GenerateCode(settings.Database);
        }

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

        internal static void GenerateCode(AudioDatabase database)
        {
            CodeWriter.WriteEnum(AudioServiceEditorUtils.EntriesFilePath,
                                 nameof(EntryId),
                                 database.AudioEntries.Select(x => x.Id).Prepend("Invalid"));

            CodeWriter.WriteEnum(AudioServiceEditorUtils.GroupFilePath,
                                 nameof(GroupId),
                                 database.AudioGroups.Select(x => x.Id).Prepend("Invalid"));

            AssetDatabase.Refresh();
        }

        private static void DrawSettings(string searchContext, VisualElement rootElement)
        {
            _settings = new SerializedObject(GetOrCreateSettings());
            SerializedProperty configProp = _settings.FindProperty(AudioServiceSettings.DatabaseName);

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

            SerializedProperty poolProp = _settings.FindProperty(AudioServiceSettings.PoolSettingsName);

            PropertyField poolField = rootElement.Q<PropertyField>("poolSize");
            poolField.BindProperty(poolProp.FindPropertyRelative(PoolSettings.PlayerPoolCountName));

            PropertyField filterField = rootElement.Q<PropertyField>("filtersPoolSize");
            filterField.BindProperty(poolProp.FindPropertyRelative(PoolSettings.FilterPlayerCountName));

            PropertyField expandField = rootElement.Q<PropertyField>("expandCount");
            expandField.BindProperty(poolProp.FindPropertyRelative(PoolSettings.ExpandCountName));

            PropertyField shrinkField = rootElement.Q<PropertyField>("shrinkCount");
            shrinkField.BindProperty(poolProp.FindPropertyRelative(PoolSettings.ShrinkCountName));
        }

        private static void OnConfigChange(ChangeEvent<UnityEngine.Object> evt)
        {
            AudioDatabase database = (AudioDatabase)evt.newValue;
            _editButton.SetEnabled(database);

            if (evt.newValue == null)
            {
                return;
            }

            GenerateCode(database);
        }

        private static void OnEditClick()
        {
            AudioServiceWindow.OpenConfigurationWindow((AudioDatabase)_configField.value);
        }

        private static void OnAddClick()
        {
            const string defaultName = "AudioConfiguration";
            const string panelTitle = "Create audio configuration asset";
            const string message = "Please select a folder";
            const string extension = "asset";

            string path = EditorUtility.SaveFilePanelInProject(panelTitle, defaultName, extension, message);
            AudioDatabase database = ScriptableObject.CreateInstance<AudioDatabase>();
            AssetDatabase.CreateAsset(database, path);

            _configField.value = database;
        }
    }
}
