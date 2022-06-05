using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace JeanLF.AudioService.Editor
{
    internal static class AudioServiceSettingsEditor
    {
        internal const string SettingsPath = "Assets/AudioService/JeanLF.AudioService/Resources/JeanLF_AS_Settings.asset";

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
            JeanLF.AudioService.AudioServiceSettings settings = AssetDatabase.LoadAssetAtPath<JeanLF.AudioService.AudioServiceSettings>(SettingsPath);

            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<JeanLF.AudioService.AudioServiceSettings>();

                AssetDatabase.CreateAsset(settings, SettingsPath);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        private static void DrawSettings(string searchContext, VisualElement rootElement)
        {
            SerializedObject settings = GetSerializedSettings();
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/AudioService/JeanLF.AudioService.Editor/UI/AudioServiceSettings.uss");
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
