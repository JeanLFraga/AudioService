using NUnit.Framework;
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.TestTools;

namespace JeanLF.AudioService.Tests
{
    [TestFixture(TestOf = typeof(AudioService))]
    public class ServiceTests : IPrebuildSetup, IPostBuildCleanup
    {
        private IAudioService _audioService;

        public void Setup()
        {
            AudioServiceSettings settings = Resources.Load<AudioServiceSettings>(TestUtility.AudioSettingsFile);
            TestUtility.WriteEntryEnums(settings.Database);
        }

        public void Cleanup()
        {
            AudioServiceSettings settings = Resources.Load<AudioServiceSettings>(AudioServiceSettings.FileName);
            TestUtility.WriteEntryEnums(settings.Database);
        }

        [OneTimeSetUp]
        public void SetupService()
        {
            AudioServiceSettings settings = Resources.Load<AudioServiceSettings>(TestUtility.AudioSettingsFile);
            _audioService = new AudioService(settings);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _audioService.Dispose();
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
}
