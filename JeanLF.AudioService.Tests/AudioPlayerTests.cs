using JeanLF.AudioService;
using NUnit.Framework;
using System;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

#if UNITY_EDITOR
using JeanLF.AudioService.Editor;
using UnityEditor;
#endif

public class AudioPlayerTests : IPrebuildSetup, IPostBuildCleanup
{
    private IAudioService _audioService;
    private static AudioDatabase _originalDatabase;

    public void Setup()
    {
        AudioDatabase database = Resources.Load<AudioDatabase>("JeanLF_TestConfig");
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

    public void Cleanup()
    {
#if UNITY_EDITOR
        CodeWriter.WriteEnum(AudioServiceEditorUtils.EntriesFilePath,
                             nameof(EntryId),
                             _originalDatabase.AudioEntries.Select(x => x.Id).Prepend("Invalid"));

        CodeWriter.WriteEnum(AudioServiceEditorUtils.GroupFilePath,
                             nameof(GroupId),
                             _originalDatabase.AudioGroups.Select(x => x.Id).Prepend("Invalid"));

        AssetDatabase.Refresh();
#endif
        _originalDatabase = null;
    }

    [OneTimeSetUp]
    public void SetupAudioService()
    {
        AudioServiceSettings settings = Resources.Load<AudioServiceSettings>(AudioServiceSettings.FileName);
        AudioDatabase database = Resources.Load<AudioDatabase>("JeanLF_TestConfig");
        _originalDatabase = settings.Configuration;
        settings.OverrideConfiguration(database);
        _audioService = new AudioService();
    }

    [OneTimeTearDown]
    public void Teardown()
    {
        _audioService.Dispose();
        AudioServiceSettings settings = Resources.Load<AudioServiceSettings>(AudioServiceSettings.FileName);
        settings.OverrideConfiguration(_originalDatabase);
    }

    [Test]
    public void PlayAudioSuccessfully()
    {
        EntryId entryId = (EntryId)Enum.GetValues(typeof(EntryId)).GetValue(1);
        GroupId groupId = (GroupId)Enum.GetValues(typeof(GroupId)).GetValue(1);
        Assert.DoesNotThrow( () => _audioService.Play(entryId, groupId));
    }

    [Test]
    public void PlayerAttachesToTransform()
    {
        EntryId entryId = (EntryId)Enum.GetValues(typeof(EntryId)).GetValue(1);
        GroupId groupId = (GroupId)Enum.GetValues(typeof(GroupId)).GetValue(1);
        GameObject gameObject = new GameObject();
        AudioPlayer audioPlayer = _audioService.Play(entryId, groupId).Attach(gameObject.transform);
        Assert.True(audioPlayer.transform.IsChildOf(gameObject.transform));
        UnityEngine.Object.Destroy(gameObject);
    }

    [Test]
    public void PlayerAttachResetOrigin()
    {
        EntryId entryId = (EntryId)Enum.GetValues(typeof(EntryId)).GetValue(1);
        GroupId groupId = (GroupId)Enum.GetValues(typeof(GroupId)).GetValue(1);
        GameObject gameObject = new GameObject();
        AudioPlayer audioPlayer = _audioService.Play(entryId, groupId).LocalPosition(new Vector3(1,1,1));
        audioPlayer.Attach(gameObject.transform);
        Assert.AreEqual(Vector3.zero,audioPlayer.transform.localPosition);
    }

    [Test]
    public void PlayBlockInvalidEntry()
    {
        EntryId entryId = (EntryId)Enum.GetValues(typeof(EntryId)).GetValue(0);
        GroupId groupId = (GroupId)Enum.GetValues(typeof(GroupId)).GetValue(1);
        Assert.Throws<InvalidEnumArgumentException>( () => _audioService.Play(entryId, groupId));
    }

    [Test]
    public void PlayBlockInvalidGroup()
    {
        EntryId entryId = (EntryId)Enum.GetValues(typeof(EntryId)).GetValue(1);
        GroupId groupId = (GroupId)Enum.GetValues(typeof(GroupId)).GetValue(0);
        Assert.Throws<InvalidEnumArgumentException>( () => _audioService.Play(entryId, groupId));
    }
}
