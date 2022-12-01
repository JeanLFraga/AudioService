using UnityEngine;

namespace JeanLF.AudioService.Editor
{
    public static class AudioServiceEditorUtils
    {
        public const string PackageName = "com.jeanlfraga.audioservice";

        public const string BaseServicePath = "Packages/" + PackageName;
        public const string EditorUIPath = BaseServicePath + "/JeanLF.AudioService.Editor/UI";
        public const string RuntimeAssetsPath = "Assets/Plugins/JeanLF.AudioService";
        public const string SettingsAssetPath = RuntimeAssetsPath + "/Resources/JeanLF_AS_Settings.asset";

        public const string GeneratedAssetsPath = BaseServicePath + "/JeanLF.AudioService/Generated";
        public const string EntriesFilePath = GeneratedAssetsPath + "/EntryId.cs";
        public const string GroupFilePath = GeneratedAssetsPath + "/GroupId.cs";

        public const string BaseNamespace = "JeanLF";
        public const string CoreNamespace = BaseNamespace + ".AudioService";
    }
}
