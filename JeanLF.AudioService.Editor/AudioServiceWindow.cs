using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace JeanLF.AudioService.Editor
{
    public class AudioServiceWindow : EditorWindow
    {
        [Serializable]
        private struct SavableList
        {
            public List<string> list;
        }

        private const string AudioConfigKey = "AudioService_AudioConfig";
        private const string FoldersKey = "AudioService_Folders";
        private const string ConfigFieldName = "configField";
        private const string FolderListName = "folderList";
        private static readonly Vector2 _minWindowSize = new Vector2(350f, 300);

        [SerializeField]
        private AudioConfig _audioConfig;
        [SerializeField]
        private DefaultAsset[] _folders;

        private SerializedObject _windowSerialized;
        private SerializedProperty _foldersProperty;
        private SerializedObject _audioConfigSerialized;
        private ObjectField _configField;
        private ReorderableArray _folderList;
        private InspectorElement _inspectorElement;

        [MenuItem("Tools/JeanLF/AudioServiceWindow")]
        public static void OpenWindow()
        {
            AudioServiceWindow wnd = GetWindow<AudioServiceWindow>("Audio Service Window");
            wnd.minSize = _minWindowSize;
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
            _windowSerialized = new SerializedObject(this);
            _foldersProperty = _windowSerialized.FindProperty(nameof(_folders));
            Load();
            _windowSerialized.Update();
        }

        private void OnDisable()
        {
            if (_folderList != null)
            {
                _folderList.OnDataUpdate -= Save;
            }
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            VisualTreeAsset visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{AudioServiceEditorUtils.EditorUIPath}/AudioServiceWindow.uxml");

            VisualElement treeAsset = visualTree.CloneTree();

            root.Add(treeAsset);

            _configField = root.Q<ObjectField>(ConfigFieldName);
            _configField.RegisterValueChangedCallback(OnConfigSelected);
            _configField.Bind(_audioConfigSerialized);
            _configField.SetValueWithoutNotify(_audioConfig);

            _folderList = root.Q<ReorderableArray>(FolderListName);
            _folderList.visible = _audioConfig != null;
            _folderList.BindProperty(_foldersProperty);
            _folderList.OnDataUpdate += Save;

            root.Q<Button>("generate").clicked += Save;
            root.Q<ScrollView>("scroll").Add(_inspectorElement);
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

            string json = EditorPrefs.GetString($"{configGuid}-{FoldersKey}", "");

            if (!string.IsNullOrEmpty(json))
            {
                SavableList saveList = JsonUtility.FromJson<SavableList>(json);
                _folders = new DefaultAsset[saveList.list.Count];

                for (int i = 0; i < saveList.list.Count; i++)
                {
                    _folders[i] = AssetDatabase.LoadAssetAtPath<DefaultAsset>(AssetDatabase.GUIDToAssetPath(saveList.list[i]));
                }
            }
        }

        private void Save()
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(_audioConfigSerialized.targetObject, out string configGuid, out long _);
            EditorPrefs.SetString(AudioConfigKey, configGuid);

            SavableList saveList = new SavableList();

            saveList.list = new List<string>();

            foreach (DefaultAsset item in _folders)
            {
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(item, out string guid, out long _);
                saveList.list.Add(guid);
            }

            string json = JsonUtility.ToJson(saveList);
            EditorPrefs.SetString($"{configGuid}-{FoldersKey}", json);
        }

        private void ClearSave()
        {
            _audioConfig = null;

            if (_audioConfigSerialized != null)
            {
                _audioConfigSerialized.Dispose();
                _audioConfigSerialized = null;
            }

            _folders = Array.Empty<DefaultAsset>();
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
            bool hasConfig = _audioConfig != null;
            _inspectorElement.visible = hasConfig;
            _folderList.style.display = new StyleEnum<DisplayStyle>(hasConfig ? DisplayStyle.Flex : DisplayStyle.None);
            _inspectorElement.Unbind();
            _inspectorElement.Bind(_audioConfigSerialized);
        }
    }
}
