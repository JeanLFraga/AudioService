using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using JeanLF.AudioService.Filters;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace JeanLF.AudioService
{
    [System.Serializable]
    internal struct AudioEntry
    {
#if UNITY_EDITOR
        public static readonly string IdPropertyPath = nameof(_id);
        public static readonly string FilterPropertyName = nameof(_filters);
#endif

        internal enum PlayMode
        {
            Random,
            Sequential,
            SequentialLoop
        }

        public AudioEntry(float volume = 1f)
        {
            _id = string.Empty;
            _clips = Array.Empty<AssetReference>();
            _audioProperties = new AudioPlayerProperties(volume:1f);
            _filters = Array.Empty<IFilterProperty>();
            _cachedId = EntryId.Invalid;
            _mode = PlayMode.Random;
        }

        public AudioEntry(
            string id,
            AssetReference[] audioClips,
            bool bypassListenerEffects,
            bool loop,
            float volume,
            float minPitch,
            float maxPitch,
            float spatialBlend,
            float stereoPan,
            float reverbZoneMix,
            float dopplerLevel,
            float spread,
            AudioRolloffMode volumeRolloff,
            float minRolloff,
            float maxRolloff,
            int priority)
        {
            _id = id;
            _clips = audioClips;
            _audioProperties = new AudioPlayerProperties(bypassListenerEffects, loop, priority, volume, minPitch,
                maxPitch, spatialBlend, stereoPan, reverbZoneMix, dopplerLevel, spread, volumeRolloff, minRolloff,
                maxRolloff);
            _filters = Array.Empty<IFilterProperty>();
            _cachedId = EntryId.Invalid;
            _mode = PlayMode.Random;
        }

        [Delayed] [SerializeField] private string _id;
        [SerializeField] private AssetReference[] _clips;
        [SerializeField] private PlayMode _mode;
        [SerializeField] private AudioPlayerProperties _audioProperties;
        [HideInInspector] [SerializeReference] private IFilterProperty[] _filters;
        private EntryId _cachedId;

        public string Id => _id;
        public EntryId ConvertedId
        {
            get
            {
                if (_cachedId == EntryId.Invalid)
                {
                    if (!EntryId.TryParse(_id, out _cachedId))
                    {
                        _cachedId = EntryId.Invalid;
                    }
                }

                return _cachedId;
            }
        }
        public PlayMode Mode => _mode;
        public AssetReference[] Clips => _clips;
        public AudioPlayerProperties AudioProperties => _audioProperties;
        public IFilterProperty[] Filters => _filters;

        //TODO Remove.
        [ContextMenu("Debug Filters")]
        private void DebugFilters()
        {
            for (int i = 0; i < _filters.Length; i++)
            {
                Debug.Log(_filters[i]);
            }
        }
    }
}
