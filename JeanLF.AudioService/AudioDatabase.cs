using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

namespace JeanLF.AudioService
{
    [CreateAssetMenu(menuName = "JeanLF Audio Service/Audio Configuration", fileName = "AudioConfig")]
    public sealed class AudioDatabase : ScriptableObject
    {
#if UNITY_EDITOR
        internal static readonly string MixerProperty = nameof(_mixer);
        internal static readonly string EntriesPropertyPath = nameof(_audioEntries);
        internal static readonly string GroupPropertyPath = nameof(_audioGroups);
#endif

        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private List<AudioEntry> _audioEntries = new List<AudioEntry>();
        [SerializeField] private List<AudioGroup> _audioGroups = new List<AudioGroup>();

        internal IReadOnlyList<AudioEntry> AudioEntries => _audioEntries;
        internal IReadOnlyList<AudioGroup> AudioGroups => _audioGroups;
    }
}
