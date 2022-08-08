using UnityEngine;

namespace JeanLF.AudioService.Editor
{
    public static class AudioServiceEditorUtils
    {
        public const string PackageName = "com.jeanlf.audioservice";

        public const string BaseServicePath = "Assets/AudioService";
        public const string RuntimeAssetsPath = BaseServicePath + "/JeanLF.AudioService";
        public const string EditorUIPath = BaseServicePath + "/JeanLF.AudioService.Editor/UI";

        public const string GeneratedAssetsPath = BaseServicePath + "/JeanLF.AudioService/Generated";
        public const string EntriesFilePath = GeneratedAssetsPath + "/EntryId.cs";
        public const string GroupFilePath = GeneratedAssetsPath + "/GroupId.cs";

        public const string BaseNamespace = "JeanLF";
        public const string AudioServiceNamespace = BaseNamespace + ".AudioService";
    }
}
