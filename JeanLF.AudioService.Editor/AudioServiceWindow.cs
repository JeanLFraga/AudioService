using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

namespace JeanLF.AudioService.Editor
{
    public class AudioServiceWindow : EditorWindow
    {
        private const string AudioConfigKey = "AudioService_AudioConfig";
        private const string ConfigFieldName = "configField";
        private static readonly Vector2 MinWindowSize = new Vector2(350f, 300);
        private readonly Regex _enumMemberRegex = new Regex("(\b[0-9]+)|([^a-zA-Z0-9])");
        private readonly TextInfo _textInfo = new CultureInfo("en-US").TextInfo;

        [SerializeField]
        private AudioConfig _audioConfig;

        private SerializedObject _audioConfigSerialized;
        private InspectorElement _inspectorElement;

        [MenuItem("Tools/JeanLF/AudioServiceWindow")]
        public static void OpenWindow()
        {
            AudioServiceWindow wnd = GetWindow<AudioServiceWindow>("Audio Service Window");
            wnd.minSize = MinWindowSize;
            wnd.Show();
        }

        [MenuItem("Debug/Close Window")]
        public static void CloseWindow()
        {
            AudioServiceWindow wnd = GetWindow<AudioServiceWindow>("Audio Service Window");
            wnd.Close();
        }

        private void OnEnable()
        {
            _inspectorElement = new InspectorElement();
            Load();
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            VisualTreeAsset visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{AudioServiceEditorUtils.EditorUIPath}/AudioServiceWindow.uxml");

            VisualElement treeAsset = visualTree.CloneTree();

            root.Add(treeAsset);

            ObjectField configField = root.Q<ObjectField>(ConfigFieldName);
            configField.RegisterValueChangedCallback(OnConfigSelected);
            configField.Bind(_audioConfigSerialized);
            configField.SetValueWithoutNotify(_audioConfig);

            root.Q<Button>("generateButton").clicked += GenerateEnums;// += Save;

            ReorderableArray entries = root.Q<ReorderableArray>("entries");
            entries.BindProperty(_audioConfigSerialized.FindProperty(AudioConfig.EntriesPropertyPath));
            entries.OnDataUpdate += OnEntriesUpdate;

            ReorderableArray groups = root.Q<ReorderableArray>("groups");
            groups.BindProperty(_audioConfigSerialized.FindProperty(AudioConfig.GroupPropertyPath));
            groups.OnDataUpdate += OnGroupsUpdate;
        }

        private void CleanupIdStrings(string arrayPath, string stringPath)
        {
            SerializedProperty arrayProp = _audioConfigSerialized.FindProperty(arrayPath);

            for (int i = 0; i < arrayProp.arraySize; i++)
            {
                SerializedProperty idProp = arrayProp.GetArrayElementAtIndex(i).FindPropertyRelative(stringPath);
                idProp.stringValue = _enumMemberRegex.Replace(CapitalizeFirstLetter(idProp.stringValue), "");
            }

            arrayProp.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private void OnGroupsUpdate()
        {
            CleanupIdStrings(AudioConfig.GroupPropertyPath, AudioGroup.IdPropertyPath);

            GenerateEnums();
        }

        private void OnEntriesUpdate()
        {
            CleanupIdStrings(AudioConfig.EntriesPropertyPath, AudioEntry.IdPropertyPath);

            GenerateEnums();
        }

        private async void Load()
        {
            string configGuid = EditorPrefs.GetString(AudioConfigKey, "");

            if (string.IsNullOrEmpty(configGuid))
            {
                return;
            }

            _audioConfig = AssetDatabase.LoadAssetAtPath<AudioConfig>(AssetDatabase.GUIDToAssetPath(configGuid));
            _audioConfigSerialized = new SerializedObject(_audioConfig);

            try
            {
                _inspectorElement.Bind(_audioConfigSerialized);
            }
            catch (NullReferenceException) //HACK InspectorElement throws an internal error on recompile
            {
                await Task.Delay(5);
                _inspectorElement.Bind(_audioConfigSerialized);
            }
        }

        private void Save()
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(_audioConfigSerialized.targetObject, out string configGuid, out long _);
            EditorPrefs.SetString(AudioConfigKey, configGuid);
            //TODO change to user settings.
        }

        private void ClearSave()
        {
            _audioConfig = null;

            if (_audioConfigSerialized != null)
            {
                _audioConfigSerialized.Dispose();
                _audioConfigSerialized = null;
            }

            EditorPrefs.DeleteKey(AudioConfigKey);
        }

        private void OnConfigSelected(ChangeEvent<UnityEngine.Object> evt)
        {
            if (evt.newValue == null)
            {
                ClearSave();
                RefreshWindow();

                return;
            }

            _audioConfig = (AudioConfig)evt.newValue;
            _audioConfigSerialized = new SerializedObject(_audioConfig);

            Save();
            RefreshWindow();
        }

        private void RefreshWindow()
        {
            _inspectorElement.visible = _audioConfig != null;
            _inspectorElement.Unbind();
            _inspectorElement.Bind(_audioConfigSerialized);
        }

        private string CodifyString(string text)
        {
            return _enumMemberRegex.Replace(_textInfo.ToTitleCase(text), "");
        }

        private void GenerateEnums()
        {
            List<string> RemoveDuplicates(IEnumerable<string> items)
            {
                HashSet<string> hashSet = new HashSet<string>(items);
                List<string> list = hashSet.ToList();
                list.Insert(0,"Invalid");

                return list;
            }

            StreamWriter file = new StreamWriter(AudioServiceEditorUtils.EntriesFilePath,false);
            SourceFile source = WriteEnum(nameof(EntryId), RemoveDuplicates(_audioConfig.AudioEntries.Select(x => x.ID)));
            file.Write(source.ToString());
            file.Close();

            file = new StreamWriter(AudioServiceEditorUtils.GroupFilePath, false);
            source = WriteEnum(nameof(GroupId), RemoveDuplicates(_audioConfig.AudioGroups.Select(x => x.ID)));
            file.Write(source.ToString());
            file.Close();

            AssetDatabase.Refresh();
        }

        private SourceFile WriteEnum(string enumName, IEnumerable<string> members)
        {
            SourceFile sourceFile = new SourceFile();
            using (new NamespaceScope(sourceFile, AudioServiceEditorUtils.AudioServiceNamespace))
            {
                sourceFile.AppendLine($"public enum {enumName}");

                using (new BracesScope(sourceFile))
                {
                    foreach (string member in members)
                    {
                        sourceFile.AppendLine($"{member},");
                    }
                }
            }

            return sourceFile;
        }

        private static string CapitalizeFirstLetter(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            string original = text;
            return text.Substring(1).Insert(0, char.ToUpper(original[0]).ToString());
        }
    }
}
