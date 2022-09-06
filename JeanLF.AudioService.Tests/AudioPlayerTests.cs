using Cysharp.Threading.Tasks;
using JeanLF.AudioService;
using JeanLF.AudioService.Tests;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace JeanLF.AudioService.Tests
{
    [TestFixture(TestOf = typeof(AudioPlayer))]
    public class AudioPlayerTests : IPrebuildSetup, IPostBuildCleanup
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
            Debug.Log(_audioService);
            _audioService.Dispose();
        }

        [Test]
        public void PlayAudioSuccessfully()
        {
            EntryId entryId = (EntryId)Enum.GetValues(typeof(EntryId)).GetValue(1);
            GroupId groupId = (GroupId)Enum.GetValues(typeof(GroupId)).GetValue(1);
            Assert.DoesNotThrow(() => _audioService.Play(entryId, groupId));
        }

        [UnityTest]
        public IEnumerator PlayAwaitSuccessful()
        {
            return UniTask.ToCoroutine(async () =>
            {
                EntryId entryId = (EntryId)Enum.GetValues(typeof(EntryId)).GetValue(1);
                GroupId groupId = (GroupId)Enum.GetValues(typeof(GroupId)).GetValue(1);
                float time = Time.realtimeSinceStartup;
                await _audioService.Play(entryId, groupId);
                Assert.Greater(Time.realtimeSinceStartup, time);
            });
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
            AudioPlayer audioPlayer = _audioService.Play(entryId, groupId).LocalPosition(new Vector3(1, 1, 1));
            audioPlayer.Attach(gameObject.transform);
            Assert.AreEqual(Vector3.zero, audioPlayer.transform.localPosition);
        }
    }
}
