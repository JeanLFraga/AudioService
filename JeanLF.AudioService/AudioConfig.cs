using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

namespace JeanLF.AudioService
{
    [CreateAssetMenu(menuName = "JeanLF Audio Service/Audio Configuration", fileName = "AudioConfig")]
    public class AudioConfig : ScriptableObject
    {
#if UNITY_EDITOR
        public static readonly string MixerProperty = nameof(_mixer);
        public static readonly string EntriesProperty = nameof(_audioEntries);
        public static readonly string GroupProperty = nameof(_audioGroups);
#endif

        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private List<AudioEntry> _audioEntries = new List<AudioEntry>();
        [SerializeField] private List<AudioGroup> _audioGroups = new List<AudioGroup>();

        public AudioClip GetAudioClip(string audioId)
        {
            for (int i = 0; i < _audioEntries.Count; i++)
            {
                if (audioId == _audioEntries[i].ID)
                {
                    return  GetRandomAssetReference(_audioEntries[i]).LoadAssetAsync<AudioClip>().WaitForCompletion();
                }
            }
            return null;
        }

        private AssetReference GetRandomAssetReference(AudioEntry entry)
        {
            return entry.Clips[UnityEngine.Random.Range(0, entry.Clips.Length)];
        }
    }
}
