using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using JeanLF.AudioService.Editor;
using UnityEditor;
#endif

namespace JeanLF.AudioService.Tests
{
    public static class TestUtility
    {
        public const string AudioSettingsFile = "JeanLF_TestSettings";
        public static void WriteEntryEnums(AudioDatabase database)
        {
#if UNITY_EDITOR
            CodeWriter.WriteEnum(AudioServiceEditorUtils.EntriesFilePath,
                                 nameof(EntryId),
                                 database.AudioEntries.Select(x => x.Id).Prepend("Invalid"));

            CodeWriter.WriteEnum(AudioServiceEditorUtils.GroupFilePath,
                                 nameof(GroupId),
                                 database.AudioGroups.Select(x => x.Id).Prepend("Invalid"));

            AssetDatabase.Refresh();
#endif
        }
    }
}
