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
        private const float LenghtEpsilon = 0.015f;

        private IAudioService _audioService;
        private AudioListener _listener;
        private AudioReference _validReference;
        private float _exampleClipLenght;

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
            _listener = new GameObject().AddComponent<AudioListener>();

            AudioServiceSettings settings = Resources.Load<AudioServiceSettings>(TestUtility.AudioSettingsFile);
            _audioService = new AudioService(settings);

            EntryId entryId = (EntryId)Enum.GetValues(typeof(EntryId)).GetValue(1);
            GroupId groupId = (GroupId)Enum.GetValues(typeof(GroupId)).GetValue(1);
            _validReference = new AudioReference(entryId, groupId);

#if UNITY_EDITOR
            _exampleClipLenght = settings.Database.AudioEntries[0].Clips[0].editorAsset.length;
#endif
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Debug.Log(_audioService);
            _audioService.Dispose();
            UnityEngine.Object.Destroy(_listener);
        }

        [Test]
        public void PlayAudioSuccessfully()
        {
            Assert.DoesNotThrow(() => _audioService.Play(_validReference));
        }

        [UnityTest]
        [Repeat(5)]
        public IEnumerator PlayAwaitSuccessful()
        {
            return UniTask.ToCoroutine(async () =>
            {
                float time = Time.realtimeSinceStartup;
                await _audioService.Play(_validReference);

                Assert.Greater(Time.realtimeSinceStartup + LenghtEpsilon, time + _exampleClipLenght);
            });
        }

        [Test]
        public void PlayerAttachesToTransform()
        {
            GameObject gameObject = new GameObject();
            AudioPlayer audioPlayer = _audioService.Play(_validReference).Attach(gameObject.transform);

            Assert.True(audioPlayer.transform.IsChildOf(gameObject.transform));
            UnityEngine.Object.Destroy(gameObject);
        }

        [Test]
        public void PlayerAttachResetOrigin()
        {
            GameObject gameObject = new GameObject();
            AudioPlayer audioPlayer = _audioService.Play(_validReference).LocalPosition(new Vector3(1, 1, 1));
            audioPlayer.Attach(gameObject.transform);

            Assert.AreEqual(Vector3.zero, audioPlayer.transform.localPosition);
        }

        [TestCase(12f,15f,20f)]
        [TestCase(8f,5f,16f)]
        [TestCase(2f,0f,10f)]
        public void PlayerAttachSetWorldPosition(float x, float y, float z)
        {
            GameObject gameObject = new GameObject();
            gameObject.transform.position = new Vector3(10f, 10f, 10f);
            AudioPlayer player = _audioService.Play(_validReference).Attach(gameObject.transform).Position(new Vector3(x,y,z));

            Assert.AreEqual(new Vector3(x, y, z), player.transform.position);
        }

        [TestCase(12f,15f,20f)]
        [TestCase(8f,5f,16f)]
        [TestCase(2f,0f,10f)]
        public void PlayerAttachSetLocalPosition(float x, float y, float z)
        {
            GameObject gameObject = new GameObject();
            gameObject.transform.position = new Vector3(10f, 10f, 10f);
            AudioPlayer player = _audioService.Play(_validReference).Attach(gameObject.transform).LocalPosition(new Vector3(x,y,z));

            Assert.AreEqual(new Vector3(x, y, z), player.transform.localPosition);
        }

        [UnityTest]
        [Repeat(5)]
        public IEnumerator PlayerYieldSuccessfully()
        {
            float time = Time.realtimeSinceStartup;
            yield return _audioService.Play(_validReference).Yield();

            Assert.Greater(Time.realtimeSinceStartup + LenghtEpsilon, time + _exampleClipLenght);
        }
    }
}
