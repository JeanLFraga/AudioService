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

        internal static JeanLF.AudioService.AudioServiceSettings GetOrCreateSettings()
        {
            JeanLF.AudioService.AudioServiceSettings settings = AssetDatabase.LoadAssetAtPath<JeanLF.AudioService.AudioServiceSettings>(AudioServiceEditorUtils.SettingsAssetPath);

            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<JeanLF.AudioService.AudioServiceSettings>();

                Directory.CreateDirectory(Path.GetDirectoryName(AudioServiceEditorUtils.SettingsAssetPath));
                AssetDatabase.CreateAsset(settings,  AudioServiceEditorUtils.SettingsAssetPath);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }

        private static void DrawSettings(string searchContext, VisualElement rootElement)
        {
            SerializedObject settings = new SerializedObject(GetOrCreateSettings());
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AudioServiceEditorUtils.EditorUIPath + "/AudioServiceSettings.uss");
            rootElement.styleSheets.Add(styleSheet);
            VisualElement container = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Column
                }
            };

            container.AddToClassList("settings-container");
            rootElement.Add(container);

            Label title = new Label()
            {
                text = "Audio Service"
            };

            title.AddToClassList("settings-header");
            container.Add(title);

            container.Add(CreatePropertyField(settings, AudioServiceSettings.ConfigMemberPath, "Audio Configuration Asset"));
            container.Add(CreatePropertyField(settings, AudioServiceSettings.SaveFolderMemberPath));
            container.Add(CreatePropertyField(settings, AudioServiceSettings.PoolSizeMemberPath));
            container.Add(CreatePropertyField(settings, AudioServiceSettings.FilterCountMemberPath));
        }

        private static PropertyField CreatePropertyField(SerializedObject serializedObject, string propertyPath, string displayName = null)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyPath);
            PropertyField field = new PropertyField(property, displayName ?? property.displayName)
            {
                tooltip = property.tooltip,
            };

            field.Bind(serializedObject);

            return field;
        }
    }
}
