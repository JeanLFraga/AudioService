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
        internal static readonly AudioPlayerProperties AudioPropertyDefaultValues = new AudioPlayerProperties(volume: 1f);

#if UNITY_EDITOR
        internal static readonly string IdPropertyPath = nameof(_id);
        internal static readonly string FilterPropertyName = nameof(_filters);
        internal static readonly string AudioPropertyName = nameof(_audioProperties);
#endif

        internal enum PlayMode
        {
            Random,
            Sequential,
            SequentialLoop
        }

        [Delayed] [SerializeField] private string _id;
        [SerializeField] private PlayMode _playMode;
        [SerializeField] private AssetReferenceT<AudioClip>[] _clips;
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
        public PlayMode Mode => _playMode;
        public AssetReferenceT<AudioClip>[] Clips => _clips;
        public AudioPlayerProperties AudioProperties => _audioProperties;
        public IFilterProperty[] Filters => _filters;
        public bool HasFilters => _filters.Length > 0;
        public void SetDefaultValues()
        {
            _audioProperties = AudioPlayerProperties.DefaultValues;
        }

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
