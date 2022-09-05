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

        private AudioDatabase _audioDatabase;
        private SerializedObject _audioConfigSerialized;

        private ReorderableArray _entriesList;
        private ReorderableArray _groupList;
        private PropertyField _mixerField;

        public static void OpenConfigurationWindow(AudioDatabase database)
        {
            AudioServiceWindow wnd = GetWindow<AudioServiceWindow>("Audio Service Window");
            wnd.Initialize(database);
            wnd.minSize = MinWindowSize;
        }

        [MenuItem("Debug/Close Window")]
        public static void CloseWindow()
        {
            AudioServiceWindow wnd = GetWindow<AudioServiceWindow>("Audio Service Window");
            wnd.Close();
        }

        internal void Initialize(AudioDatabase database)
        {
            _audioDatabase = database;
            _audioConfigSerialized = new SerializedObject(_audioDatabase);
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
            _entriesList.OnItemAdd += OnEntryAdd;

            _groupList = root.Q<ReorderableArray>("groups");
            _groupList.OnDataUpdate += OnGroupsUpdate;

            _mixerField = root.Q<PropertyField>("mixer");

            if (_audioDatabase != null)
            {
                Initialize(_audioDatabase);
            }
        }

        private void OnEntryAdd()
        {
            SerializedProperty arrayProp = _audioConfigSerialized.FindProperty(AudioDatabase.EntriesPropertyPath);
            SerializedProperty itemProp = arrayProp.GetArrayElementAtIndex(arrayProp.arraySize - 1);
            AudioEntry entry = (AudioEntry)itemProp.GetValue();
            entry.SetDefaultValues();
            itemProp.SetValueNoRecord(entry);
            itemProp.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private void BindControls()
        {
            _entriesList.BindProperty(_audioConfigSerialized.FindProperty(AudioDatabase.EntriesPropertyPath));
            _groupList.BindProperty(_audioConfigSerialized.FindProperty(AudioDatabase.GroupPropertyPath));
            _mixerField.BindProperty(_audioConfigSerialized.FindProperty(AudioDatabase.MixerProperty));
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
            CleanupIdStrings(AudioDatabase.GroupPropertyPath, AudioGroup.IdPropertyPath);

            GenerateEnums();
        }

        private void OnEntriesUpdate()
        {
            CleanupIdStrings(AudioDatabase.EntriesPropertyPath, AudioEntry.IdPropertyPath);

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

            CodeWriter.WriteEnum(AudioServiceEditorUtils.EntriesFilePath,
                                  nameof(EntryId),
                                  RemoveDuplicates(_audioDatabase.AudioEntries.Select(x => x.Id)));

            CodeWriter.WriteEnum(AudioServiceEditorUtils.GroupFilePath,
                                  nameof(GroupId),
                                  RemoveDuplicates(_audioDatabase.AudioGroups.Select(x => x.Id)));

            AssetDatabase.Refresh();
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
