using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

namespace JeanLF.AudioService.Editor
{
    public class AudioServiceWindow : EditorWindow
    {
        private static readonly Vector2 MinWindowSize = new Vector2(350f, 300);
        private readonly Regex _enumMemberRegex = new Regex("(^[^A-Za-z]+)|([^a-zA-Z0-9])");

        private AudioConfig _audioConfig;
        private SerializedObject _audioConfigSerialized;

        private ReorderableArray _entriesList;
        private ReorderableArray _groupList;
        private PropertyField _mixerField;

        public static void OpenConfigurationWindow(AudioConfig config)
        {
            AudioServiceWindow wnd = GetWindow<AudioServiceWindow>("Audio Service Window");
            wnd.Initialize(config);
            wnd.minSize = MinWindowSize;
        }

        [MenuItem("Debug/Close Window")]
        public static void CloseWindow()
        {
            AudioServiceWindow wnd = GetWindow<AudioServiceWindow>("Audio Service Window");
            wnd.Close();
        }

        internal void Initialize(AudioConfig config)
        {
            _audioConfig = config;
            _audioConfigSerialized = new SerializedObject(_audioConfig);
            BindControls();
            GenerateEnums();
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            VisualTreeAsset visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{AudioServiceEditorUtils.EditorUIPath}/AudioServiceWindow.uxml");
            VisualElement treeAsset = visualTree.CloneTree();

            root.Add(treeAsset);

            _entriesList = root.Q<ReorderableArray>("entries");
            _entriesList.OnDataUpdate += OnEntriesUpdate;

            _groupList = root.Q<ReorderableArray>("groups");
            _groupList.OnDataUpdate += OnGroupsUpdate;

            _mixerField = root.Q<PropertyField>("mixer");

            if (_audioConfig != null)
            {
                Initialize(_audioConfig);
            }
        }

        private void BindControls()
        {
            _entriesList.BindProperty(_audioConfigSerialized.FindProperty(AudioConfig.EntriesPropertyPath));
            _groupList.BindProperty(_audioConfigSerialized.FindProperty(AudioConfig.GroupPropertyPath));
            _mixerField.BindProperty(_audioConfigSerialized.FindProperty(AudioConfig.MixerProperty));
        }

        private void CleanupIdStrings(string arrayPath, string stringPath)
        {
            SerializedProperty arrayProp = _audioConfigSerialized.FindProperty(arrayPath);

            for (int i = 0; i < arrayProp.arraySize; i++)
            {
                SerializedProperty idProp = arrayProp.GetArrayElementAtIndex(i).FindPropertyRelative(stringPath);
                idProp.stringValue = CapitalizeFirstLetter(_enumMemberRegex.Replace(idProp.stringValue, ""));
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
                        if (string.IsNullOrWhiteSpace(member))
                        {
                            continue;
                        }
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
